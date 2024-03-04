using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes;
using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.Nodes.EditorNodes.TypeAlike;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodes.Common
{
    [CompileTarget(ECompileTarget.All), NodeIcon("Comment.png")]
    [IsInfertile]
    public class Comment : TreeNode
    {
        [JsonConstructor]
        private Comment() : base() { }
        public Comment(Document workspace) : this(workspace, "") { }
        public Comment(Document workspace, string comment) : base(workspace)
        {
            CodeComment = comment;
        }

        [JsonIgnore, NodeAttribute, DefaultValue("")]
        public string CodeComment
        {
            get => CheckAttr("Comment").attrValue;
            set => CheckAttr("Comment").attrValue = value;
        }

        public override string ToString()
        {
            return CodeComment;
        }

        public override IEnumerable<string> ToLua(int spacing)
        {
            string sp = Indent(spacing);
            yield return sp + $"--[[ {CodeComment} ]]\n";
        }

        public override IEnumerable<string> ToChambersite(int spacing)
        {
            string sp = Indent(spacing);
            yield return sp + $"/* {CodeComment} */\n";
        }

        public override object Clone()
        {
            Comment cloned = new(ParentWorkspace);
            cloned.CopyData(this);
            return cloned;
        }
    }
}
