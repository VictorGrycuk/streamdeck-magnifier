using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Magnifier.Helpers;
using System;
using System.Threading.Tasks;

namespace Magnifier
{
    [PluginActionId("victorgrycuk.streamdeck.magnifier")]
    public class Magnifier : PluginBase
    {
        private readonly MagnifierSettings settings;
        private bool isRunning;

        public Magnifier(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            settings = payload.Settings == null || payload.Settings.Count == 0
                ? MagnifierSettings.CreateDefaultSettings()
                : payload.Settings.ToObject<MagnifierSettings>();
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public override void KeyPressed(KeyPayload payload)
        {
            isRunning = !isRunning;
        }

        public override void KeyReleased(KeyPayload payload) { }

        public override void OnTick()
        {
            if (isRunning)
            {
                var img = ImageHelper.CopyFromScreen(settings.ZoomLevel);
                img = ImageHelper.ResizeImage(img, 144, 144);
                Connection.SetImageAsync(img);
            }
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            ParseZoomLevel();
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        private void ParseZoomLevel()
        {
            if (int.TryParse(settings.PIZoomLevel, out int value))
            {
                settings.ZoomLevel = value;
            }
            else
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Cannot parse: " + settings.PIZoomLevel);
            }
        }
    }
}