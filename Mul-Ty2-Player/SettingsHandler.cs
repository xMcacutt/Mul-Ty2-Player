using Newtonsoft.Json;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT2PClient
{
    public static class Extensions
    {
        public static string RemoveWhiteSpaces(this string str)
        {
            return string.Concat(str.Where(c => !char.IsWhiteSpace(c)));
        }
    }

    internal class SettingsHandler
    {
        public static Settings Settings { get; private set; }

        public static Dictionary<string, bool> SyncSettings;

        public static void Setup()
        {
            string json = File.ReadAllText("./ServerSettings.json");
            Settings = JsonConvert.DeserializeObject<Settings>(json);
        }
    }
}
