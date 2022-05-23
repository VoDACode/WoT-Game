using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Enums;

namespace WoTCore.Modes
{
    public class ResourceInfo
    {
        public ResourceType ResourceType { get; set; }
        public string Name { get; set; }

        public ResourceInfo(ResourceType resourceType, string name)
        {
            ResourceType = resourceType;
            Name = name;
        }

    }
}
