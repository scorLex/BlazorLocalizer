// ----------------------------------------------------------------------
// <copyright file="AJsonExtensionOptions.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Text.Json;

namespace BlazorLocalizer.Core
{
    /// <summary>
    /// Base class for extension options using Json serialization.
    /// </summary>
    public abstract class AJsonExtensionOptions : AExtensionOptions
    {
        /// <summary>
        /// Gets/Sets JsonSerializer custom options.
        /// </summary>
        public JsonSerializerOptions? JsonSerializerOptions { get; set; }
    }
}
