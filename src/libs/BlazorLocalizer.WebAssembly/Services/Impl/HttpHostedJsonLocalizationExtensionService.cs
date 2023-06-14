// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationExtensionService.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using BlazorLocalizer.Helpers;
using BlazorLocalizer.ServerSide;
using BlazorLocalizer.Services;
using BlazorLocalizer.Services.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorLocalizer.WebAssembly.Services.Impl
{
    /// <summary>
    /// Http hosted Json Localization extension service.
    /// </summary>
    public class HttpHostedJsonLocalizationExtensionService : AHttpHostedJsonLocalizationExtensionService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<HttpHostedJsonLocalizationExtensionService> logger;

        /// <summary>
        /// Setup with the HttpClient.
        /// </summary>
        /// <param name="httpClientFactory">The Host HttpClient.</param>
        /// <param name="logger">Logger where to log processing messages.</param>
        /// <param name="httpCacheService">Http loading task cache service.</param>
        public HttpHostedJsonLocalizationExtensionService(IHttpClientFactory httpClientFactory, ILogger<HttpHostedJsonLocalizationExtensionService> logger, IHttpCacheService httpCacheService)
            : base(logger, httpCacheService)
        {
            this.httpClientFactory = httpClientFactory;
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

            try
            {
                if (!option.DisableLogs)
                {
                    this.logger.LoadingLocalizationDataFromHttpClient(uri);
                }

                using var httpClient = this.httpClientFactory.CreateClient(option.HttpClientName);
                using var stream = await httpClient.GetStreamAsync(uri).ConfigureAwait(false);

                var map = await JsonHelper
                    .DeserializeAsync(stream, jsonSerializerOptions)
                    .ConfigureAwait(false);

                return map ?? throw new FileLoadException("Null resources");

            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                if (!option.DisableLogs)
                {
                    this.logger.HttpFileNotFound(uri, e);
                }

                return null;
            }
        }
    }
}
