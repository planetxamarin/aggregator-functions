using Newtonsoft.Json;

namespace PlanetXamarinAuthors.Models
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
