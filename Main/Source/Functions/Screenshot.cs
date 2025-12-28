using Main.Source.Utilities;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Main.Source.Functions
{
    public class Screenshot
    {
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private static float _scale = -0.0f;

        public async static Task<MemoryStream?> TakeScreenshot(float? scale = null)
        {
            int x = GetSystemMetrics(0);
            int y = GetSystemMetrics(1);

            if (x == 0 || y == 0)
            {
                return null;
            }

            float theScale = scale ?? (_scale != -0.0f ? _scale : GetScale());

            var bitmap = new Bitmap((int)(x * theScale), (int)(y * theScale));

            try
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                }

                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;

                return stream;
            }
            catch (Exception ex)
            {
                Logger.Error($"TakeScreenshot: [{ex.GetType()}]: {ex.Message} - {ex.StackTrace}");
                return null;
            }
            finally
            {
                bitmap.Dispose();
            }
        }

        private static float GetScale()
        {
            var metrics = Registry.CurrentUser.OpenSubKey(
                "Control Panel\\Desktop\\WindowMetrics",
                false
            );

            if (metrics == null)
            {
                Logger.Warn("GetScale: failed to open WindowMetrics key");
                return 1.0f;
            }

            var dip = metrics.GetValue("AppliedDPI");
            if (dip == null)
            {
                Logger.Warn("GetScale: failed to get AppliedDPI value");
                return 1.0f;
            }

            _scale = (int)dip / 96f;

            return _scale;
        }
    }
}
