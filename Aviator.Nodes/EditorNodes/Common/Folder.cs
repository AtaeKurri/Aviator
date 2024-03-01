using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodes.Common
{
    internal class Folder : TreeNode
    {
        [JsonConstructor]
        private Folder() : base() { }

        public Folder(Document workspace) : this(workspace, "Folder") { }
        public Folder(Document workspace, string name) : base(workspace)
        {
            Name = name;
        }

        [JsonIgnore, NodeAttribute("Folder")]
        public string Name
        {
            get => CheckAttr().attrValue;
            set => CheckAttr().attrValue = value;
        }

        public override string ToString()
        {
            return Name;
        }

        public override IEnumerable<string> ToLua(int spacing)
        {
            yield return $"-- Folder: {Name}";
        }
        
        public override IEnumerable<string> ToChambersite(int spacing)
        {
            yield return $"// Folder: {Name}";
        }

        public override object Clone()
        {
            Folder folder = new Folder(ParentWorkspace);
            folder.CopyData(this);
            return folder;
        }
    }
}
