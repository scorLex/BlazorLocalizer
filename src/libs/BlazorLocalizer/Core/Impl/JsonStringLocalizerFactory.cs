﻿// ----------------------------------------------------------------------
// <copyright file="JsonStringLocalizerFactory.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BlazorLocalizer.Helpers;
using BlazorLocalizer.Services;

namespace BlazorLocalizer.Core.Impl
{
    /// <summary>
    /// IStringLocalizerFactory implementation that will create the IStringLocalizer instance
    /// from the extension configured in the JsonLocalization options.
    /// </summary>
    public sealed class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private class FactoryInternal : IJsonStringLocalizerFactoryInternal
        {
            private readonly string baseName;
            private readonly Assembly assembly;
            private readonly IEnumerable<(string baseName, Assembly assembly)> parents;
            private readonly Func<string, Assembly, CultureInfo, IJsonStringLocalizerFactoryInternal, IStringLocalizerInternal> createHandler;

            public FactoryInternal(string baseName, Assembly assembly,
                IEnumerable<(string baseName, Assembly assembly)> parents,
                Func<string, Assembly, CultureInfo, IJsonStringLocalizerFactoryInternal, IStringLocalizerInternal> createHandler)
            {
                this.baseName = baseName;
                this.assembly = assembly;
                this.parents = parents;
                this.createHandler = createHandler;
            }

            public IStringLocalizerInternal CreateStringLocalizer(CultureInfo cultureInfo)
            {
                return this.createHandler(this.baseName, this.assembly, cultureInfo, this);
            }

            public LocalizedString? ProcessThroughStringLocalizerHierarchy(CultureInfo cultureInfo, Func<IStringLocalizerInternal, LocalizedString?> forward)
            {
                foreach (var item in this.parents)
                {
                    var factory = new FactoryInternal(item.baseName, item.assembly, Enumerable.Empty<(string baseName, Assembly assembly)>(), this.createHandler);

                    var itemStringLocalizer = this.createHandler(item.baseName, item.assembly, cultureInfo, factory);

                    var localizedString = forward(itemStringLocalizer);
                    if (localizedString != null)
                    {
                        return localizedString;
                    }
                }

                return null;
            }
        }

        private readonly JsonLocalizationOptions options;
        private readonly IExtensionResolverService extensionResolverService;
        private readonly ICultureInfoService cultureInfoService;
        private readonly ICacheService cacheService;
        private readonly ILogger<JsonStringLocalizerFactory> logger;
        private readonly ILogger<StringLocalizerProxy> loggerForStringLocalizerProxy;
        public HttpHostedJsonLocalizationOptions HostedJsonLocalizationOptions
        {
            get
            {
                var hostedJsonLocalizationOptions = this.options.ExtensionOptions?.FirstOrDefault()?.Options as HttpHostedJsonLocalizationOptions;
                if (hostedJsonLocalizationOptions != null)
                {
                    return hostedJsonLocalizationOptions;
                }
                else
                {
                    return new HttpHostedJsonLocalizationOptions();
                }
            }
        }

        /// <summary>
        /// Setup the factory.
        /// </summary>
        /// <param name="options">The JsonLocalization options.</param>
        /// <param name="cultureInfoService">Service providing the current culture.</param>
        /// <param name="extensionResolverService">The service resolver to get the JsonLocalization
        /// extension service.</param>
        /// <param name="cacheService">The service to cache the loaded data.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="loggerForStringLocalizerProxy">Logger to pass when creating a <see cref="StringLocalizerProxy"/>.</param>
        public JsonStringLocalizerFactory(
            IOptions<JsonLocalizationOptions> options,
            ICultureInfoService cultureInfoService,
            IExtensionResolverService extensionResolverService,
            ICacheService cacheService,
            ILogger<JsonStringLocalizerFactory> logger,
            ILogger<StringLocalizerProxy> loggerForStringLocalizerProxy)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.logger = logger;
            this.loggerForStringLocalizerProxy = loggerForStringLocalizerProxy;
            this.options = options.Value;
            this.extensionResolverService = extensionResolverService;
            this.cultureInfoService = cultureInfoService;
            this.cacheService = cacheService;
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            var assembly = resourceSource.Assembly;
            var baseName = resourceSource.GetBaseName();

            var parents = new List<(string baseName, Assembly assembly)>();

            var baseType = resourceSource.BaseType;
            if (baseType != null && baseType != typeof(object))
            {
                parents.Add((baseType.GetBaseName(), baseType.Assembly));
            }

            parents.AddRange(resourceSource.GetInterfaces().Select(x => (x.GetBaseName(), x.Assembly)));

            parents.AddRange(this.options.Fallbacks);

            return CreateStringLocalizerProxy(baseName, assembly, parents);
        }

        ///<inheritdoc/>
        public IStringLocalizer Create(string baseName, string location)
        {
            var assembly = Assembly.Load(location);

            return CreateStringLocalizerProxy(baseName, assembly, this.options.Fallbacks);
        }

        private IStringLocalizer CreateStringLocalizerProxy(string baseName, Assembly assembly, IEnumerable<(string baseName, Assembly assembly)> parents)
        {
            // First use the cache.
            var localizer = this.cacheService.Match(assembly, baseName, null);
            if (localizer != null)
            {
                return localizer.AsStringLocalizer;
            }

            localizer = new StringLocalizerProxy(
                this.loggerForStringLocalizerProxy,
                this.cultureInfoService,
                new FactoryInternal(baseName, assembly, parents, CreateStringLocalizer));

            this.cacheService.Cache(assembly, baseName, null, localizer);

            return localizer.AsStringLocalizer;
        }

        private IStringLocalizerInternal CreateStringLocalizer(string baseName, Assembly assembly, CultureInfo cultureInfo, IJsonStringLocalizerFactoryInternal factoryInternal)
        {
            // First use the cache.
            var localizer = this.cacheService.Match(assembly, baseName, cultureInfo);
            if (localizer != null)
            {
                return localizer;
            }

            var task = LoadStringLocalizerAsync(assembly, baseName, cultureInfo);

            if (task.Status == TaskStatus.RanToCompletion)
            {
                var map = task.Result;
                if (map != null)
                {
                    localizer = new JsonStringLocalizer(map, cultureInfo, factoryInternal);
                    this.cacheService.Cache(assembly, baseName, cultureInfo, localizer);
                }
                else
                {
                    localizer = new NullStringLocalizer(cultureInfo, factoryInternal, true);
                }
            }
            else
            {
                IStringLocalizerInternal loadingLocalizer = this.options.IsDisplayKeysWhileLoadingAsynchronouslyEnabled
                    ? new NullStringLocalizer(cultureInfo, factoryInternal, false)
                    : new ConstStringLocalizer("...", factoryInternal);

                localizer = new JsonStringLocalizerAsync(task, cultureInfo, factoryInternal, loadingLocalizer);
                this.cacheService.Cache(assembly, baseName, cultureInfo, localizer);
            }

            return localizer;
        }

        private async Task<IReadOnlyDictionary<string, string>?> LoadStringLocalizerAsync(Assembly assembly, string baseName, CultureInfo cultureInfo)
        {
            var hadErrors = false;
            foreach (var extensionOptionsContainer in this.options.ExtensionOptions)
            {
                try
                {
                    var options = extensionOptionsContainer.Options;

                    var noAssambliesToMatch = !options.Assemblies.Any();

                    if (noAssambliesToMatch || options.Assemblies.Contains(assembly))
                    {
                        var extensionService = this.extensionResolverService.GetExtensionService(extensionOptionsContainer);

                        var map = await extensionService.TryLoadAsync(options, assembly, baseName, cultureInfo).ConfigureAwait(false);
                        if (map != null)
                        {
                            return map;
                        }
                    }
                }
                catch (FileLoadException)
                {

                }
#pragma warning disable CA1031 // Ne pas intercepter les types d'exception générale
                catch (Exception)
#pragma warning restore CA1031 // Ne pas intercepter les types d'exception générale
                {
                    hadErrors = true;
                }
            }

            if (!this.HostedJsonLocalizationOptions.DisableLogs)
            {
                this.logger.UnableToLoadLocalizationData(baseName, assembly, cultureInfo);
            }

            if (hadErrors)
            {
                this.cacheService.Reset(assembly, baseName, cultureInfo);
            }

            return null;
        }
    }
}
