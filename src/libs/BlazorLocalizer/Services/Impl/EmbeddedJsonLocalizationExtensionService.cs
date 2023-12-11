// ----------------------------------------------------------------------
// <copyright file="EmbeddedJsonLocalizationExtensionService.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorLocalizer.Helpers;
using Microsoft.Extensions.Logging;
using BlazorLocalizer.Core.Impl;

namespace BlazorLocalizer.Services.Impl
{
    /// <summary>
    /// Embedded Json Localization extension service.
    /// </summary>
    public class EmbeddedJsonLocalizationExtensionService : IJsonLocalizationExtensionService<EmbeddedJsonLocalizationOptions>
    {
        private readonly ILogger<EmbeddedJsonLocalizationExtensionService> logger;

        /// <summary>
        /// Setup EmbeddedJsonLocalizationExtensionService with the given logger.
        /// </summary>
        /// <param name="logger">Logger where to log processing messages.</param>
        public EmbeddedJsonLocalizationExtensionService(ILogger<EmbeddedJsonLocalizationExtensionService> logger)
        {
            this.logger = logger;
        }

        ///<inheritdoc/>
        public async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadAsync(
            EmbeddedJsonLocalizationOptions options,
            Assembly assembly,
            string baseName,
            CultureInfo cultureInfo)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (cultureInfo == null)
            {
                throw new ArgumentNullException(nameof(cultureInfo));
            }
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var rootNameSpace = options.RootNameSpaceResolver?.Invoke(assembly) ?? assembly.GetName().Name;

            var embeddedFileProviderFactory = GetSatelliteFileProviderFactory(assembly, rootNameSpace);

            var basePath = ResourcePathHelper.ComputeBasePath(assembly, baseName, rootNameSpace);

            return await LoadStringMapAsync(embeddedFileProviderFactory, options.ResourcesPath, basePath, cultureInfo, options)
                .ConfigureAwait(false);
        }

        private async ValueTask<IReadOnlyDictionary<string, string>?> LoadStringMapAsync(
            SatelliteFileProviderFactory fileProviderFactory,
            string resourcesPath,
            string basePath,
            CultureInfo cultureInfo,
            EmbeddedJsonLocalizationOptions options)
        {
            var jsonSerializerOptions = options.JsonSerializerOptions;

            basePath = string.IsNullOrEmpty(resourcesPath)
                ? basePath
                : Path.Combine(resourcesPath, basePath);

            return await CultureInfoHelper.WalkThoughCultureInfoParentsAsync(cultureInfo,
                culture =>
                {
                    var handler = options.NamingPolicy ?? ResourcePathHelper.DefaultEmbeddedJsonNamingPolicy;
                    var path = handler.Invoke(basePath, culture.Name);

                    var fileProvider = fileProviderFactory.GetFileProvider(culture);

                    return TryLoadMapAsync(fileProvider, path, jsonSerializerOptions).AsTask();
                })
                .ConfigureAwait(false);
        }

        private async ValueTask<IReadOnlyDictionary<string, string>?> TryLoadMapAsync(
            IFileProvider fileProvider,
            string path,
            JsonSerializerOptions? jsonSerializerOptions)
        {
            var fileInfo = fileProvider.GetFileInfo(path);

            if (!fileInfo.Exists)
            {
                return null;
            }

            using var stream = fileInfo.CreateReadStream();

            var map = await JsonHelper
                .DeserializeAsync(stream, jsonSerializerOptions)
                .ConfigureAwait(false);

            return map ?? throw new FileLoadException("Null resources");
        }

        private static SatelliteFileProviderFactory GetSatelliteFileProviderFactory(Assembly assembly, string rootNameSpace)
        {
            return new SatelliteFileProviderFactory(assembly, rootNameSpace);
        }
    }
}
