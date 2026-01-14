using Main.Source.Utilities;
using System.Runtime.InteropServices;

namespace Main.Source.Functions
{
    public class ScreenRotate
    {
        [DllImport("user32.dll")]
        private static extern int EnumDisplaySettings(
            int lpszDeviceName,
            int iModeNum,
            ref DeviceModeA lpDevMode
        );

        [DllImport("user32.dll")]
        private static extern int ChangeDisplaySettings(
            ref DeviceModeA lpDevMode,
            uint dwFlags
        );

        [StructLayout(LayoutKind.Sequential)]
        private struct DeviceModeA
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;

            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }

        public static bool Rotate(int by)
        {
            try
            {
                var device = new DeviceModeA();
                device.dmSize = (short)Marshal.SizeOf(typeof(DeviceModeA));

                if (EnumDisplaySettings(0, -1, ref device) == 0)
                {
                    return false;
                }

                if ((device.dmDisplayOrientation + by) % 2 == 1)
                {
                    int height = device.dmPelsHeight;
                    device.dmPelsHeight = device.dmPelsWidth;
                    device.dmPelsWidth = height;
                }

                device.dmDisplayOrientation = by;
                device.dmFields = 0x80 | 0x80000 | 0x100000;

                int rotated = ChangeDisplaySettings(ref device, 0x01);
                return rotated == 0;
            }
            catch (Exception ex)
            {
                Logger.Error($"Rotate: [{ex.GetType()}]: {ex.Message} - {ex.StackTrace}");
                return false;
            }
        }
    }
}
