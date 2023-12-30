using Newtonsoft.Json;

namespace Magnifier
{
    internal class MagnifierSettings
    {
        public static MagnifierSettings CreateDefaultSettings()
        {
            var instance = new MagnifierSettings { ZoomLevel = 2, RefreshRate = 100 };
            return instance;
        }

        [JsonProperty(PropertyName = "zoomLevel")]
        public string PIZoomLevel { get; set; }

        [JsonProperty(PropertyName = "useCrosshair")]
        public bool UseCrosshair { get; set; }

        public int ZoomLevel { get; set; }

        [JsonProperty(PropertyName = "refreshRate")]
        public int RefreshRate { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "isLocked")]
        public bool IsLocked { get; set; }
    }
}
