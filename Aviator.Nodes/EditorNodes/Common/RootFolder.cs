using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes;
using Aviator.Core.EditorData.Nodes.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodes.Common
{
    [CompileTarget(ECompileTarget.All), NodeIcon("Folder.png")]
    [CannotBeDeleted]
    public class RootFolder : TreeNode
    {
        [JsonConstructor]
        private RootFolder() : base() { }

        public RootFolder(Document workspace) : base(workspace) { }

        public override string ToString()
        {
            return "Root";
        }

        public override object Clone()
        {
            RootFolder folder = new RootFolder(ParentWorkspace);
            folder.CopyData(this);
            return folder;
        }
    }
}
