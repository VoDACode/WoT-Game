using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using WoTCore.Models;
using WoTCore.Modes;
using WoTCore.Modes.Interfaces;
using WoTCore.Modes.Resources;

namespace ModesLoader
{
    public static class ModesLoader
    {
        static ModesLoader()
        {
            if (!Directory.Exists("modes"))
                Directory.CreateDirectory("modes");
        }
        public static List<ModeContent> Load()
        {
            List<ModeContent> objects = new List<ModeContent>();

            var files = Directory.GetFiles("modes");
            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetFileName(files[i]) == "WoTCore.dll")
                    continue;
                var dll = Assembly.LoadFile($"{AppContext.BaseDirectory}{files[i]}");
                var dllRootName = Path.GetFileNameWithoutExtension(dll.ManifestModule.Name);
                Type assemblyInfoType = dll.GetType($"{dllRootName}.AssemblyInfo");
                if (assemblyInfoType == null)
                    throw new DllNotFoundException($"Not found 'AssemblyInfo' in '{dllRootName}'.");

                object o = Activator.CreateInstance(assemblyInfoType);
                AssemblyInfo assemblyInfo = JsonConvert.DeserializeObject<AssemblyInfo>(JsonConvert.SerializeObject(o));
                // copy data to assemblyInfo
                foreach (var m in assemblyInfoType.GetFields())
                {
                    var p = assemblyInfo.GetType().GetProperty(m.Name);
                    p.SetValue(assemblyInfo, m.GetValue(o));
                }
                // validation data
                foreach (var f in assemblyInfo.GetType().GetFields())
                {
                    if (f.GetValue(assemblyInfo) == null)
                        throw new DllNotFoundException($"{dllRootName}: AssemblyInfo, not found '{f.Name}'.");
                }
                // loading mode resources
                var modeContect = new ModeContent(assemblyInfo);
                foreach (var resource in assemblyInfo.AddedResources)
                {
                    var type = dll.GetType($"{dllRootName}.{resource.ResourceType}.{resource.Name}");
                    if (type == null)
                        throw new DllNotFoundException($"\"{dllRootName}\": resource '{dllRootName}.{resource.ResourceType}.{resource.Name}' not found.");
                    var iComponent = Activator.CreateInstance(type);

                    if (resource.ResourceType == WoTCore.Enums.ResourceType.Blocks)
                    {
                        var elemet = toType<BlockResource>(iComponent);
                        modeContect.Blocks.Add(elemet);
                        elemet.Setup();
                    }
                    else if (resource.ResourceType == WoTCore.Enums.ResourceType.Structures)
                    {
                        var elemet = toType<StructureResource>(iComponent);
                        modeContect.Structures.Add(elemet);
                        elemet.Setup();
                    }
                }
                objects.Add(modeContect);
            }

            return objects;
        }

        private static T toType<T>(object input) where T : IType, new()
        {
            T resulr = new T();
            var iType = input.GetType();
            var iObj = Activator.CreateInstance(iType);
            foreach (var m in iType.GetProperties())
            {
                var p = resulr.GetType().GetProperty(m.Name);
                if (p == null)
                    continue;
                p.SetValue(resulr, m.GetValue(iObj));
            }
            resulr.ItemType = iType;
            return resulr;
        }

    }
}
