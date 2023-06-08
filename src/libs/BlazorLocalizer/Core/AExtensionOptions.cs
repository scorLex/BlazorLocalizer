﻿// ----------------------------------------------------------------------
// <copyright file="AExtensionOptions.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace BlazorLocalizer.Core
{
    /// <summary>
    /// Base class for extension options.
    /// </summary>
    public abstract class AExtensionOptions
    {
        /// <summary>
        /// Assemblies to match when the extension is loading resources. The extension won't apply
        /// on other assemblies except if the list is empty.
        /// </summary>
        public IEnumerable<Assembly> Assemblies { get; set; } = Array.Empty<Assembly>();
    }
}
