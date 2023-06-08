﻿// ----------------------------------------------------------------------
// <copyright file="IExtensionResolverService.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using BlazorLocalizer.Core;

namespace BlazorLocalizer.Services
{
    /// <summary>
    /// Extension resolver service that resolve the extension service from a given
    /// extension options type.
    /// </summary>
    public interface IExtensionResolverService
    {
        /// <summary>
        /// Get the Json Localization extension service.
        /// </summary>
        /// <param name="optionsContainer">The extension options container to get the associated service
        /// from.</param>
        /// <returns>The Json Localization extension service.</returns>
        IJsonLocalizationExtensionService GetExtensionService(IExtensionOptionsContainer optionsContainer);
    }
}
