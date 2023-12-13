// ----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using BlazorLocalizer.Services;
using BlazorLocalizer.WebAssembly.Services.Impl;
using System;
using System.Globalization;

namespace BlazorLocalizer.WebAssembly
{
    /// <summary>
    /// Extension methods to setup the WebAssembly Json localization.
    /// </summary>
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="clientName"></param>
        /// <param name="baseAddress"></param>
        /// <returns></returns>
        public static IServiceCollection AddLocalizationHttpClient(this IServiceCollection services, string clientName, string baseAddress)
        {
            return services.AddHttpClient(clientName, client =>
             {
                 client.DefaultRequestHeaders.AcceptLanguage.Clear();
                 client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.Name);
                 client.BaseAddress = new Uri(baseAddress);
             }).Services;
        }

        /// <summary>
        /// Add WebAssembly Json localization services.
        /// </summary>
        /// <param name="services">The service collection to setup.</param>
        /// <param name="setupAction">The action delegate to fine tune the Json localizer behavior
        /// (Use embedded Json resource files if null).</param>
        /// <returns>The given service collection updated with the Json localization services.</returns>
        public static IServiceCollection AddWebAssemblyJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptionsBuilder> setupAction)
        {
            services
                .AddJsonLocalization(setupAction, ServiceLifetime.Scoped);

            services.AddScoped<
                IJsonLocalizationExtensionService<HttpHostedJsonLocalizationOptions>,
                HttpHostedJsonLocalizationExtensionService>();

            return services;
        }
    }
}
