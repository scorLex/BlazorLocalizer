// ----------------------------------------------------------------------
// <copyright file="CultureInfoService.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Globalization;

namespace BlazorLocalizer.Services.Impl
{
    /// <summary>
    /// CultureInfo service implementation.
    /// </summary>
    public class CultureInfoService : ICultureInfoService
    {
        ///<inheritdoc/>
        public CultureInfo CurrentUICulture
        {
            get
            {
                return CultureInfo.CurrentUICulture;
            }
        }
    }
}
