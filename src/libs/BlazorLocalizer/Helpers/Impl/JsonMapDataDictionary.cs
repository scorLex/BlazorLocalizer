﻿// ----------------------------------------------------------------------
// <copyright file="JsonMapDataDictionary.cs" company="Ömer Çelik">
// Copyright © 2023 Ömer Çelik.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System.Collections.Generic;

namespace BlazorLocalizer.Helpers.Impl
{
    internal class JsonMapDataDictionary : AJsonMapData
    {
        public Dictionary<string, AJsonMapData>? ValueMap { get; set; }

        public override void FillIn(string? prefix, Dictionary<string, string> map)
        {
            foreach (var mapItem in ValueMap)
            {
                var itemPrefix = string.IsNullOrEmpty(prefix) ? mapItem.Key : prefix + Key.Separator + mapItem.Key;

                mapItem.Value.FillIn(itemPrefix, map);
            }
        }
    }
}
