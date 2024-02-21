using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;


namespace PluginLoader
{
    [ApiVersion(2, 1)]
    public class PluginLoader : TerrariaPlugin
    {
        private List<PluginContainer> Loaded = new List<PluginContainer>();

        private Command cmd;

        private Main main;

        public override string Author => "Leader，肝帝熙恩更新";

        public override string Description => "A sample plugin to reload the plugins when server is running";

        public override string Name => "Plugin Loader";


        public override Version Version => new Version(2, 3, 0, 1);

        public PluginLoader(Main game)
            : base(game)
        {
            main = game;
        }

        public override void Initialize()
        {
            cmd = new Command("pluginloader.admin", new CommandDelegate(loader), new string[1] { "load" });
            Loaded.AddRange(ServerApi.Plugins);
            Config.GetConfig();
            Commands.ChatCommands.Add(cmd);
        }

        private List<PluginContainer> GetPlugins()
        {
            List<PluginContainer> list = new List<PluginContainer>();
            string[] files = Directory.GetFiles("ServerPlugins\\");
            foreach (string path in files)
            {
                try
                {
                    Assembly assembly = Assembly.Load(File.ReadAllBytes(path));
                    Type[] exportedTypes = assembly.GetExportedTypes();
                    foreach (Type type in exportedTypes)
                    {
                        if (type.IsSubclassOf(typeof(TerrariaPlugin)) && type.IsPublic && !type.IsAbstract)
                        {
                            object[] customAttributes = type.GetCustomAttributes(typeof(ApiVersionAttribute), inherit: false);
                            if (customAttributes.Length != 0)
                            {
                                TerrariaPlugin val = (TerrariaPlugin)Activator.CreateInstance(type, main);
                                list.Add(new PluginContainer(val));
                            }
                        }
                    }
                }
                catch (Exception value)
                {
                    Console.WriteLine(value);
                }
            }
            return list;
        }

        private void UnLoadPlugin(CommandArgs args, string plugin)
        {
            List<PluginContainer> list = new List<PluginContainer>();
            list.AddRange(Loaded);
            try
            {
                PluginContainer val = list.ToList().Find((PluginContainer p) => p.Plugin.Name == plugin);
                val.DeInitialize();
                val.Dispose();
                Loaded.Remove(val);
                args.Player.SendInfoMessage(val.Plugin.Name + "已卸载");
            }
            catch (Exception arg)
            {
                args.Player.SendErrorMessage($"插件:{plugin}在卸载时报错:{arg}");
            }
        }

        private void LoadPlugin(CommandArgs args, string plugin)
        {
            List<PluginContainer> list = new List<PluginContainer>();
            string[] files = Directory.GetFiles("ServerPlugins\\");
            foreach (string path in files)
            {
                try
                {
                    Assembly assembly = Assembly.Load(File.ReadAllBytes(path));
                    Type[] exportedTypes = assembly.GetExportedTypes();
                    foreach (Type type in exportedTypes)
                    {
                        if (type.IsSubclassOf(typeof(TerrariaPlugin)) && type.IsPublic && !type.IsAbstract)
                        {
                            object[] customAttributes = type.GetCustomAttributes(typeof(ApiVersionAttribute), inherit: false);
                            if (customAttributes.Length != 0)
                            {
                                TerrariaPlugin val = (TerrariaPlugin)Activator.CreateInstance(type, main);
                                list.Add(new PluginContainer(val));
                            }
                        }
                    }
                }
                catch (Exception value)
                {
                    Console.WriteLine(value);
                }
            }
            foreach (PluginContainer item in from x in list
                                             orderby x.Plugin.Order, x.Plugin.Name
                                             select x)
            {
                try
                {
                    if (!(item.Plugin.Name != plugin))
                    {
                        item.Initialize();
                        Loaded.Add(item);
                        args.Player.SendInfoMessage($"插件: {item.Plugin.Name} v{item.Plugin.Version} (作者: {item.Plugin.Author}) 已加载。", new object[1] { TraceLevel.Info });
                    }
                }
                catch (Exception arg)
                {
                    args.Player.SendErrorMessage($"Plugin \"{item.Plugin.Name}\" has thrown an exception:{arg} during initialization.");
                }
            }
        }

        private void loader(CommandArgs args)
        {
            if (args.Parameters.Count() == 0 || args.Parameters[0] == "help")
            {
                args.Player.SendInfoMessage("/load unload 插件名,卸载已加载插件");
                args.Player.SendInfoMessage("/load loaded,列出已加载插件");
                args.Player.SendInfoMessage("/load list,列出可加载插件的列表");
                args.Player.SendInfoMessage("/load this 插件名,加载指定插件");
                args.Player.SendInfoMessage("/load all,加载所有插件(PluginLoader.json中设置不加载的插件)");
                return;
            }
            switch (args.Parameters[0])
            {
                case "unload":
                    UnLoadPlugin(args, args.Parameters[1]);
                    break;
                case "loaded":
                    {
                        foreach (PluginContainer item in Loaded)
                        {
                            args.Player.SendInfoMessage("插件：" + item.Plugin.Name + "，作者：" + item.Plugin.Author + "，版本：v" + item.Plugin.Version?.ToString() + "，描述：" + item.Plugin.Description);
                        }
                        break;
                    }
                case "all":
                    {
                        foreach (PluginContainer p2 in GetPlugins())
                        {
                            if (Config.GetConfig().IgnoreReload.ToList().FindAll((string s) => s == p2.Plugin.Name).Count == 0)
                            {
                                UnLoadPlugin(args, p2.Plugin.Name);
                                LoadPlugin(args, p2.Plugin.Name);
                            }
                        }
                        break;
                    }
                case "this":
                    {
                        string plugin = args.Parameters[1];
                        if (GetPlugins().FindAll((PluginContainer p) => p.Plugin.Name == plugin).Count == 0)
                        {
                            args.Player.SendErrorMessage("查无插件:" + plugin);
                            break;
                        }
                        UnLoadPlugin(args, plugin);
                        LoadPlugin(args, plugin);
                        break;
                    }
                case "list":
                    {
                        foreach (PluginContainer plugin2 in GetPlugins())
                        {
                            args.Player.SendInfoMessage("插件：" + plugin2.Plugin.Name + "，作者：" + plugin2.Plugin.Author + "，版本：v" + plugin2.Plugin.Version?.ToString() + "，描述：" + plugin2.Plugin.Description);
                        }
                        break;
                    }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.Remove(cmd);
            }
            base.Dispose(disposing);
        }
    }
}