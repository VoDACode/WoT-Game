﻿using WoTCore.Modes.Resources;
using System.Collections.Generic;
using WoTCore.Modes;

namespace WoTCore.Models
{
    public class ModeContent
    {
        public AssemblyInfo AssemblyInfo { get; }
        public List<BlockResource> Blocks { get; } = new List<BlockResource>();
        public List<StructureResource> Structures { get; } = new List<StructureResource>();
        public ModeContent(AssemblyInfo assembly)
        {
            AssemblyInfo = assembly;
        }
    }
}
