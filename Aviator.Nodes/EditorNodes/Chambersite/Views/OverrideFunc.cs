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

namespace Aviator.Nodes.EditorNodes.Chambersite.Views
{
    [CompileTarget(ECompileTarget.Chambersite), NodeIcon("Func.png")]
    [RequireParent(typeof(GameObjectAlike))]
    public class OverrideFunc : TreeNode
    {
        [JsonConstructor]
        private OverrideFunc() : base() { }
        public OverrideFunc(Document workspace) : this(workspace, "Initialize") { }
        public OverrideFunc(Document workspace, string method) : base(workspace)
        {
            MethodOverride = method;
        }

        [JsonIgnore, NodeAttribute, DefaultValue("Initialize")]
        public string MethodOverride
        {
            get => CheckAttr("Method", "CBSMethods").attrValue;
            set => CheckAttr("Method", "CBSMethods").attrValue = value;
        }

        public override string ToString()
        {
            return $"{MethodOverride}()";
        }

        public override IEnumerable<string> ToChambersite(int spacing)
        {
            string sp = Indent(spacing);
            yield return sp + $"public override void {MethodOverride}()\n";
            yield return sp + "{\n";
            foreach (var a in base.ToChambersite(spacing + 1))
            {
                yield return a;
            }
            yield return sp + "}\n";
        }

        public override object Clone()
        {
            OverrideFunc cloned = new(ParentWorkspace);
            cloned.CopyData(this);
            return cloned;
        }
    }
}
