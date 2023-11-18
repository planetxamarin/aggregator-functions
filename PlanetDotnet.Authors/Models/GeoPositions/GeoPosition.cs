// ---------------------------------------------------------------
// Copyright (c) 2023 Planet Dotnet. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using Newtonsoft.Json;

namespace PlanetDotnet.Authors.Models.GeoPositions
{
    public class GeoPosition
    {
        public static GeoPosition Empty = new GeoPosition(-1337, 42);

        [JsonProperty("lat", Required = Required.Always)]
        public double Lat { get; }
        [JsonProperty("lon", Required = Required.Always)]
        public double Lng { get; }

        public GeoPosition(double lat, double lng)
        {
            Lat = lat;
            Lng = lng;
        }
    }
}
