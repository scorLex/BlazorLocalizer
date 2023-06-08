﻿// ----------------------------------------------------------------------
// <copyright file="IHttpCacheService.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorLocalizer.Services
{
    /// <summary>
    /// Http loading task cache to prevent loading several times the same resources.
    /// </summary>
    public interface IHttpCacheService
    {
        /// <summary>
        /// Process loading task unless it is already loading.
        /// </summary>
        /// <param name="uri">Uri to fetch.</param>
        /// <param name="loadingHandler">Loading task.</param>
        /// <returns>The loading task.</returns>
        Task<IReadOnlyDictionary<string, string>?> ProcessLoadingTask(Uri uri, Func<Task<IReadOnlyDictionary<string, string>?>> loadingHandler);
    }
}
