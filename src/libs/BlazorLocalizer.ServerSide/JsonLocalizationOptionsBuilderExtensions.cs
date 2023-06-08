﻿// ----------------------------------------------------------------------
// <copyright file="JsonLocalizationOptionsBuilderExtensions.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace BlazorLocalizer.ServerSide
{
    /// <summary>
    /// Extension methods to setup Http hosted Json file support.
    /// </summary>
    public static class JsonLocalizationOptionsBuilderExtensions
    {
        /// <summary>
        /// Setup default Http hosted Json support.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <returns>The given builder updated with the Json Http hosted options.</returns>
        public static JsonLocalizationOptionsBuilder UseHttpHostedJson(
            this JsonLocalizationOptionsBuilder builder)
            => Http.JsonLocalizationOptionsBuilderExtensions.UseHttpHostedJson(builder);

        /// <summary>
        /// Setup Http hosted Json support with the given setup action.
        /// </summary>
        /// <param name="builder">The builder to setup.</param>
        /// <param name="setup">The setup action.</param>
        /// <returns>The given builder updated with the Json http hosted options.</returns>
        public static JsonLocalizationOptionsBuilder UseHttpHostedJson(
            this JsonLocalizationOptionsBuilder builder,
            Action<HttpHostedJsonLocalizationOptions>? setup)
            => Http.JsonLocalizationOptionsBuilderExtensions.UseHttpHostedJson(builder, setup);
    }
}