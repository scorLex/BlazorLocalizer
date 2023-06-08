// ----------------------------------------------------------------------
// <copyright file="ICultureInfoService.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Globalization;

namespace BlazorLocalizer.Services
{
    /// <summary>
    /// CultureInfo service to provide abstraction.
    /// </summary>
    public interface ICultureInfoService
    {
        /// <summary>
        /// Get the current UI cultureInfo.
        /// </summary>
        CultureInfo CurrentUICulture { get; }
    }
}
