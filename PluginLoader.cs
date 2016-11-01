using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin;
using System.IO;
using System.Reflection;

namespace Sharkfuscator
{
    class PluginLoader
    {
        public string Plugin_directory { get; set; }
        public PluginLoader(string directory)
        {
            Plugin_directory = directory;
        }

        public void LoadPlugins(List<iProtection> plugins)
        {
            Type plugin_type = typeof(iProtection);
            foreach (string file in Directory.GetFiles(Plugin_directory, "*.dll"))
            {
                Assembly assembly = Assembly.Load(file);
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }
                    else
                    {
                        if (type.GetInterface(plugin_type.FullName) != null)
                        {
                            plugins.Add((iProtection)Activator.CreateInstance(type));
                        }
                    }
                }
            }
        }
    }
}