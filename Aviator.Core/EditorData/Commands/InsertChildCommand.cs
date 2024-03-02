using Aviator.Core.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Commands
{
    public class InsertChildCommand : InsertCommand
    {
        public InsertChildCommand(TreeNode source, TreeNode node)
            : base(source, node) { }

        public override void Execute()
        {
            Source.AddChild(ToInsert);
        }

        public override void Undo()
        {
            Source.RemoveChild(ToInsert);
        }
    }
}
