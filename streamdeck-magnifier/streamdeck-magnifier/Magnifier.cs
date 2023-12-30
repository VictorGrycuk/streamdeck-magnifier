using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using Magnifier.Helpers;
using System;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;

namespace Magnifier
{
    [PluginActionId("victorgrycuk.streamdeck.magnifier")]
    public class Magnifier : PluginBase
    {
        private readonly MagnifierSettings settings;
        private readonly Timer Timer;
        private bool isFixed;
        private Point mouseLocation;
        private DateTime dateTime;

        public Magnifier(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            settings = payload.Settings == null || payload.Settings.Count == 0
                ? MagnifierSettings.CreateDefaultSettings()
                : payload.Settings.ToObject<MagnifierSettings>();

            ParseLocation();
            isFixed = settings.IsLocked;
            Timer = new Timer(settings.RefreshRate);
            Timer.Elapsed += new ElapsedEventHandler(UpdateKey);
            Timer.Enabled = false;
            Timer.Start();
        }

        private void UpdateKey(object sender, ElapsedEventArgs e)
        {
            if (Timer.Enabled)
            {
                var location = isFixed ? mouseLocation : ScreenHelper.GetMouseLocation();
                var img = ImageHelper.CopyFromScreen(settings.ZoomLevel, location);
                img = ImageHelper.ResizeImage(img, 144, 144);

                if (settings.UseCrosshair)
                {
                    ImageHelper.DrawCrosshair(img);
                }

                Connection.SetImageAsync(img);
            }
        }

        public override void Dispose()
        {
            Timer.Stop();
            Timer.Dispose();
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Timer.Enabled = !Timer.Enabled;
            dateTime = DateTime.Now;
        }

        public override void KeyReleased(KeyPayload payload)
        {
            if ((DateTime.Now - dateTime).TotalSeconds > 2)
            {
                mouseLocation = ScreenHelper.GetMouseLocation();
                isFixed = !isFixed;
                Timer.Enabled = true;
                settings.Location = $"{mouseLocation.X};{mouseLocation.Y}";
                Logger.Instance.LogMessage(TracingLevel.DEBUG, settings.Location);
                settings.IsLocked = isFixed;
                SaveSettings();
            }
        }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            ParseZoomLevel();
            UpdateRefreshRate();
            ParseLocation();
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

        private void UpdateRefreshRate()
        {
            Timer.Interval = settings.RefreshRate;
        }

        private void ParseLocation()
        {
            if (settings.Location == null) return;
            var points = settings.Location.Split(';');
            if (points.Length < 2) return;
            mouseLocation = new Point(int.Parse(points[0]), int.Parse(points[1]));
        }
    }
}