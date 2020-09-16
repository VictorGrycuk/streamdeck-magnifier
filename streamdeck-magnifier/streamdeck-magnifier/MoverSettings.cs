using Newtonsoft.Json;

namespace Magnifier
{
    internal class MoverSettings
    {
        internal enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public static MoverSettings CreateDefaultSettings()
        {
            return new MoverSettings();
        }

        [JsonProperty(PropertyName = "direction")]
        public string PIDirection { get; set; }

        public Direction CurrentDirection { get; set; }
    }
}