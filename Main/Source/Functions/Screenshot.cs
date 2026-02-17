using Main.Source.Utilities;
using Microsoft.Win32;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Main.Source.Functions
{
    public class Screenshot
    {
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern bool GetCursorInfo(out CursorInfo pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(
            IntPtr hDC,
            int X,
            int Y,
            IntPtr hIcon
        );

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CursorInfo
        {
            public int cbSize;
            public uint flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }

        private static float _scale = -0.0f;

        public async static Task<MemoryStream?> TakeScreenshot(
            float? scale = null,
            bool cursor = false
        )
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

                    if (!cursor)
                    {
                        goto noCursor;
                    }
                        
                    var info = new CursorInfo();
                    info.cbSize = Marshal.SizeOf(typeof(CursorInfo));

                    if (!GetCursorInfo(out info))
                    {
                        goto noCursor;
                    }

                    nint hdc = graphics.GetHdc();

                    try
                    {
                        DrawIcon(
                            hdc,
                            (int)(info.ptScreenPos.x * theScale),
                            (int)(info.ptScreenPos.y * theScale),
                            info.hCursor
                        );
                    }
                    finally
                    {
                        graphics.ReleaseHdc(hdc);
                    }

                noCursor:;
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
