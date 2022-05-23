using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Modes
{
    public class AssemblyInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public IReadOnlyList<ResourceInfo> AddedResources { get; set; }
    }
}
