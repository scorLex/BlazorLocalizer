// ----------------------------------------------------------------------
// <copyright file="HttpHostedJsonLocalizationOptions.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using BlazorLocalizer.Core;
using System;
using System.Reflection;

namespace BlazorLocalizer
{
    /// <summary>
    /// JsonLocalizer Http Hosted extension options
    /// </summary>
    public class HttpHostedJsonLocalizationOptions : AJsonExtensionOptions
    {
        /// <summary>
        /// Naming policy handler used to compute the actual Http hosted Json file to fetch.
        /// </summary>
        /// <param name="basePath">The base resource path of the json file.</param>
        /// <param name="cultureName">The culture name (if any).</param>
        /// <returns>The actual Uri to load.</returns>
        public delegate Uri NamingPolicyHandler(string basePath, string cultureName);

        /// <summary>
        /// Gets/Sets Path where to get the resources.
        /// </summary>
        public string ResourcesPath { get; set; } = string.Empty;

        /// <summary>
        /// Set the delegate for custom file naming.
        /// For example:
        /// (basePath, cultureName) => new Uri($"{basePath}{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"-{cultureName}")}.json", UriKind.Relative);
        /// </summary>
        public NamingPolicyHandler? NamingPolicy { get; set; }

        /// <summary>
        /// Gets/Sets the application assembly.
        /// </summary>
        public Assembly? ApplicationAssembly { get; set; }

        /// <summary>
        /// Set base address for hosted localization
        /// </summary>
        public string? HttpClientName { get; set; }
    }
}
