using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes
{
    public interface IMainWindow
    {
        void Insert(TreeNode node, bool doInvoke = true);
        public static Document CurrentWorkspaceStatic { get; }
        public Document CurrentWorkspace { get; }
    }
}
