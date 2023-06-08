// ----------------------------------------------------------------------
// <copyright file="AJsonMapData.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BlazorLocalizer.Helpers.Impl
{
    [JsonConverter(typeof(JsonMapDataConverter))]
    internal abstract class AJsonMapData
    {
        public abstract void FillIn(string? prefix, Dictionary<string, string> map);
    }
}
