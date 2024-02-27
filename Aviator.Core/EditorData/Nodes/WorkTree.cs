using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Nodes
{
    [Serializable]
    public class WorkTree : ObservableCollection<TreeNode>
    {
        public WorkTree() : base() { }
        public WorkTree(IEnumerable<TreeNode> treeNodes) : base(treeNodes) { }
        public WorkTree(List<TreeNode> treeNodes) : base(treeNodes) { }
    }
}
