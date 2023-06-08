// ----------------------------------------------------------------------
// <copyright file="IExtensionOptionsContainer.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace BlazorLocalizer.Core
{
    /// <summary>
    /// Json localization extension options container.
    /// </summary>
    public interface IExtensionOptionsContainer
    {
        /// <summary>
        /// Get the extension options type.
        /// </summary>
        Type ExtensionOptionsType { get; }

        /// <summary>
        /// Get the extension service type.
        /// </summary>
        Type ExtensionServiceType { get; }

        /// <summary>
        /// Gets the generic options.
        /// </summary>
        AExtensionOptions Options { get; }
    }
}
