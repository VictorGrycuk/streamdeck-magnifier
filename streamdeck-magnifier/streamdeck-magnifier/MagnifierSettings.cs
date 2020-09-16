using Newtonsoft.Json;

namespace Magnifier
{
    internal class MagnifierSettings
    {
        public static MagnifierSettings CreateDefaultSettings()
        {
            var instance = new MagnifierSettings { ZoomLevel = 2 };
            return instance;
        }

        [JsonProperty(PropertyName = "zoomLevel")]
        public string PIZoomLevel { get; set; }

        public int ZoomLevel { get; set; }
    }
}
