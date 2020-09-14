using Newtonsoft.Json;

namespace Magnifier
{
    internal class PluginSettings
    {
        public static PluginSettings CreateDefaultSettings()
        {
            var instance = new PluginSettings { ZoomLevel = 2 };
            return instance;
        }

        [JsonProperty(PropertyName = "zoomLevel")]
        public string PIZoomLevel { get; set; }

        public int ZoomLevel { get; set; }
    }
}
