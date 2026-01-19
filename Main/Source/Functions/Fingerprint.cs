using Main.Source.Utilities;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace Main.Source.Functions
{
    public class Fingerprint
    {
        public static string GenFingerprint()
        {
            if (ConfigMisc.DEBUG_MODE)
            {
                return "debug";
            }

            var crypto = Registry.LocalMachine.OpenSubKey(
                "SOFTWARE\\Microsoft\\Cryptography",
                false
            );

            string guid = "";

            if (crypto == null)
            {
                Logger.Warn("GenFingerprint: failed to open Cryptography key");
                guid = Environment.OSVersion.ToString() + Environment.ProcessorCount.ToString();
            }
            else
            {
                guid = crypto.GetValue("MachineGuid") as string ?? DateTime.Now.ToString();
            }

            byte[] joined = Encoding.UTF8.GetBytes($"{guid?.ToString()}-{Environment.MachineName}");
            byte[] hashed = MD5.HashData(joined);

            return Convert.ToHexString(hashed).ToUpper();
        }
    }
}
