using Aviator.Core.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Commands
{
    public class InsertBeforeCommand : InsertCommand
    {
        public InsertBeforeCommand(TreeNode source, TreeNode node)
            : base(source, node) { }

        public override void Execute()
        {
            TreeNode parent = Source.Parent;
            parent?.InsertChild(ToInsert, parent.Children.IndexOf(Source));
        }

        public override void Undo()
        {
            Source.Parent?.RemoveChild(ToInsert);
        }
    }
}
