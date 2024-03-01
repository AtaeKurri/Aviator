using Aviator.Core.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Commands
{
    public abstract class InsertCommand : Command
    {
        protected TreeNode source;
        protected TreeNode toInsert;

        public InsertCommand(TreeNode source, TreeNode toInsert)
        {
            this.source = source;
            this.toInsert = toInsert;
        }
    }
}
