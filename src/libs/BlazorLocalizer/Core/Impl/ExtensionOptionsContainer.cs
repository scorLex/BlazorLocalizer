// ----------------------------------------------------------------------
// <copyright file="ExtensionOptionsContainer.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using BlazorLocalizer.Services;
using System;

namespace BlazorLocalizer.Core.Impl
{
    /// <summary>
    /// Typed JsonLocalization extension options.
    /// </summary>
    /// <typeparam name="TOptions">The extension options type.</typeparam>
    public sealed class ExtensionOptionsContainer<TOptions> : IExtensionOptionsContainer
        where TOptions : AExtensionOptions
    {
        /// <summary>
        /// Setup the ExtensionOptionsContainer with the given options.
        /// </summary>
        /// <param name="options">The extension options</param>
        public ExtensionOptionsContainer(TOptions options)
        {
            Options = options;
        }

        ///<inheritdoc/>
        public Type ExtensionOptionsType => typeof(TOptions);

        ///<inheritdoc/>
        public Type ExtensionServiceType => typeof(IJsonLocalizationExtensionService<TOptions>);

        /// <summary>
        /// Get the typed extension options.
        /// </summary>
        public TOptions Options { get; }

        ///<inheritdoc/>
        AExtensionOptions IExtensionOptionsContainer.Options => Options;
    }
}
