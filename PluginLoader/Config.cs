using System.IO;
using Newtonsoft.Json;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;
using TShockAPI;

namespace PluginLoader
{
    internal class Config
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "PluginLoader.json");

        public string[] IgnoreReload { get; set; }

        public Config(string[] ignoreReload = null)
        {
            IgnoreReload = ((ignoreReload != null) ? ignoreReload : new string[2] { "TShock", "PluginLoader" });
        }

        public static Config GetConfig()
        {
            Config config = new Config();
            if (File.Exists("tshock\\PluginLoader.json"))
            {
                using StreamReader streamReader = new StreamReader("tshock\\PluginLoader.json");
                config = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
            }
            else
            {
                using StreamWriter streamWriter = new StreamWriter("tshock\\PluginLoader.json");
                streamWriter.WriteLine(JsonConvert.SerializeObject((object)config, (Formatting)1));
            }
            return config;
        }
    }
}

