using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Modes;

namespace DefaultMode
{
    public class AssemblyInfo
    {

        public IReadOnlyList<ResourceInfo> AddedResources => new List<ResourceInfo> {
            new ResourceInfo(WoTCore.Enums.ResourceType.Blocks, "Stone"),
            new ResourceInfo(WoTCore.Enums.ResourceType.Blocks, "Bush"),
            new ResourceInfo(WoTCore.Enums.ResourceType.Blocks, "Water"),
            new ResourceInfo(WoTCore.Enums.ResourceType.Blocks, "Sand")
        };

        public const string Name = "DefaultMode";

        public const string Description = "This is default mode.";

        public const string Author = "VoDA";

        public const string Version = "0.1";
    }
}
