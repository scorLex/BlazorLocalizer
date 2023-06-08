// ----------------------------------------------------------------------
// <copyright file="NameHelper.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace BlazorLocalizer.Helpers
{
    internal static class NameHelper
    {
        internal static string GetBaseName(this Type type)
        {
            return type.FullName ?? type.Name;
        }
    }
}
