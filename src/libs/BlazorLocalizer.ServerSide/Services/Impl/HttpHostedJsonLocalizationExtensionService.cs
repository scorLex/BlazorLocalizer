// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionService.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using BlazorLocalizer.Services.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using BlazorLocalizer.Helpers;
using BlazorLocalizer.Services;

namespace BlazorLocalizer.ServerSide.Services.Impl
{
    /// <summary>
    /// Http hosted Json Localization extension service.
    /// </summary>
    public class HttpHostedJsonLocalizationExtensionService : AHttpHostedJsonLocalizationExtensionService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<HttpHostedJsonLocalizationExtensionService> logger;

        /// <summary>
        /// Setup with the IWebHostEnvironment.
        /// </summary>
        /// <param name="webHostEnvironment">The IWebHostEnvironment.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="httpCacheService">Http loading task cache service.</param>
        public HttpHostedJsonLocalizationExtensionService(
            IWebHostEnvironment webHostEnvironment,
            ILogger<HttpHostedJsonLocalizationExtensionService> logger,
            IHttpCacheService httpCacheService)
            : base(logger, httpCacheService)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        ///<inheritdoc/>
        protected override async Task<IReadOnlyDictionary<string, string>?> TryLoadFromUriAsync<TOptions>(TOptions options, Uri uri, JsonSerializerOptions? jsonSerializerOptions)
        {
            var option = options as HttpHostedJsonLocalizationOptions;
            if (option == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!option.DisableLogs)
            {
                this.logger.LoadingLocalizationDataFromHost(uri);
            }

            var fileInfo = this.webHostEnvironment.WebRootFileProvider.GetFileInfo(uri.OriginalString);

            if (!fileInfo.Exists)
            {
                if (!option.DisableLogs)
                {
                    this.logger.FileDoesNotExist(uri);
                }

                return null;
            }

            if (!option.DisableLogs)
            {
                this.logger.LoadingFile(uri);
            }

            using var stream = fileInfo.CreateReadStream();

            var map = await JsonHelper
                .DeserializeAsync(stream, jsonSerializerOptions)
                .ConfigureAwait(false);

            return map ?? throw new FileLoadException("Null resources");
        }
    }
}
