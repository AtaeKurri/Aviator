using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Nodes
{
    public struct OnCreateEventArgs
    {
        public TreeNode Parent;
    }

    public struct OnRemoveEventArgs
    {
        public TreeNode Parent;
    }

    public delegate void OnCreateNodeHandler(OnCreateEventArgs e);
    public delegate void OnRemoveNodeHandler(OnRemoveEventArgs e);
}
