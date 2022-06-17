using WoTCore.Modes.Resources;
using System.Collections.Generic;
using WoTCore.Modes;

namespace WoTCore.Models
{
    public class ModeContent
    {
        public AssemblyInfo AssemblyInfo { get; }
        public List<BlockResource> Blocks { get; set; } = new List<BlockResource>();
        public List<StructureResource> Structures { get; set; } = new List<StructureResource>();
        public ModeContent(AssemblyInfo assembly)
        {
            AssemblyInfo = assembly;
        }
    }
}
