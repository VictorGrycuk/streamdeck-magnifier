using BarRaider.SdTools;
using Magnifier.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using static Magnifier.MoverSettings;

namespace Magnifier
{
    [PluginActionId("victorgrycuk.streamdeck.mover")]
    public class Mover : PluginBase
    {
        private readonly MoverSettings settings;

        public Mover(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            settings = payload.Settings == null || payload.Settings.Count == 0
                ? CreateDefaultSettings()
                : payload.Settings.ToObject<MoverSettings>();
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public override void KeyPressed(KeyPayload payload)
        {
            var pos = ScreenHelper.GetMouseLocation();

            switch (settings.CurrentDirection)
            {
                case Direction.Left:
                    pos.X -= 1;
                    break;
                case Direction.Right:
                    pos.X += 1;
                    break;
                case Direction.Up:
                    pos.Y -= 1;
                    break;
                case Direction.Down:
                    pos.Y += 1;
                    break;
            }

            ScreenHelper.SetMouseLocation(pos.X, pos.Y);
        }

        public override void KeyReleased(KeyPayload payload) { }

        public override void OnTick()
        {
            
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            UpdateDirection();
            UpdateKeyImage();
            SaveSettings();
        }

        private void UpdateKeyImage()
        {
            try
            {
                var arrowIconPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Images", "arrow.png");
                var keyImage = Image.FromFile(arrowIconPath);
                
                switch (settings.CurrentDirection)
                {
                    case Direction.Left:
                        keyImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case Direction.Right:
                        keyImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case Direction.Down:
                        keyImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    default:
                        break;
                }

                Connection.SetImageAsync(keyImage);

            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, ex.Message);
            }
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        private void UpdateDirection()
        {
            _ = Enum.TryParse(settings.PIDirection, true, out Direction direction);

            settings.CurrentDirection = direction;
        }

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }
    }
}