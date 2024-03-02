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
        protected TreeNode Source;
        protected TreeNode ToInsert;

        public InsertCommand(TreeNode source, TreeNode toInsert)
        {
            this.Source = source;
            this.ToInsert = toInsert;
        }
    }
}
