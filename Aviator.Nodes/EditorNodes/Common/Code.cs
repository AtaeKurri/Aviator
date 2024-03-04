using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.Core.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Aviator.Core.EditorData.Documents;

namespace Aviator.Nodes.EditorNodes.Common
{
    [CompileTarget(ECompileTarget.All), NodeIcon("code.png")]
    [IsInfertile]
    public class Code : TreeNode
    {
        [JsonConstructor]
        private Code() : base() { }

        public Code(Document workSpaceData)
            : this(workSpaceData, "") { }

        public Code(Document workSpaceData, string code) : base(workSpaceData)
        {
            //attributes.Add(new AttrItem("Code", this, "code") { AttrInput = code });
            CodeContent = code;
        }

        [JsonIgnore, NodeAttribute]
        public string CodeContent
        {
            get => CheckAttr("Code").attrValue;
            set => CheckAttr("Code").attrValue = value;
        }

        public override IEnumerable<string> ToLua(int spacing)
        {
            Regex r = new Regex("\\n");
            string sp = Indent(spacing);
            string nsp = "\n" + sp;
            yield return sp + r.Replace(CodeContent, nsp) + "\n";
        }

        public override IEnumerable<string> ToChambersite(int spacing)
        {
            Regex r = new Regex("\\n");
            string sp = Indent(spacing);
            string nsp = "\n" + sp;
            yield return sp + r.Replace(CodeContent, nsp) + "\n";
        }

        /*public override IEnumerable<Tuple<int, TreeNode>> GetLines()
        {
            string s = CodeContent(0);
            int i = 1;
            foreach (char c in s)
            {
                if (c == '\n') i++;
            }
            yield return new Tuple<int, TreeNode>(i, this);
        }*/

        public override string ToString()
        {
            return CodeContent;
        }

        public override object Clone()
        {
            var n = new Code(ParentWorkspace);
            n.CopyData(this);
            return n;
        }
    }
}
