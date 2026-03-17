using System.Collections.Generic;
using System.IO;

namespace VideoScreensaver
{
    class Config
    {
        internal static string ConfigPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\" + "Config.txt";

        private const string PATH_STRING = "PATH:";
        private const string VOLUME_STRING = "VOLUME:";
        private const string STRETCH_STRING = "STRETCH:";
        private const string LOGGING_STRING = "LOGGING:";

        internal struct ConfigOptions
        {
            public List<string> VideoPaths;
            public double Volume;
            public string StretchMode;
            public bool EnableLogging;
        }

        internal static void WriteConfig(List<string> videoPaths, double volume, string stretchMode, bool enableLogging)
        {
            List<string> options = new List<string>();
            foreach (var path in videoPaths) options.Add(PATH_STRING + path);
            options.Add(VOLUME_STRING + volume.ToString());
            options.Add(STRETCH_STRING + stretchMode);
            options.Add(LOGGING_STRING + enableLogging.ToString());
            File.WriteAllLines(ConfigPath, options.ToArray());
        }

        internal static ConfigOptions ReadConfig()
        {
            string[] options = File.Exists(ConfigPath) ? File.ReadAllLines(ConfigPath) : new string[0];
            ConfigOptions co = new ConfigOptions { VideoPaths = new List<string>(), Volume = 1.0, StretchMode = "UniformToFill", EnableLogging = false };

            for (int i = 0; i < options.Length; i++)
            {
                if (options[i].StartsWith(PATH_STRING)) co.VideoPaths.Add(options[i].Substring(PATH_STRING.Length));
                else if (options[i].StartsWith(VOLUME_STRING)) co.Volume = double.Parse(options[i].Substring(VOLUME_STRING.Length));
                else if (options[i].StartsWith(STRETCH_STRING)) co.StretchMode = options[i].Substring(STRETCH_STRING.Length);
                else if (options[i].StartsWith(LOGGING_STRING)) bool.TryParse(options[i].Substring(LOGGING_STRING.Length), out co.EnableLogging);
            }
            return co;
        }

        internal static void CheckConfig()
        {
            if (!File.Exists(ConfigPath)) WriteConfig(new List<string>(), 1.0, "UniformToFill", false);
        }
    }
}
