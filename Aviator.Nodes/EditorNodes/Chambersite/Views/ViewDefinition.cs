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
using System.Xml.Linq;

namespace Aviator.Nodes.EditorNodes.Chambersite.Views
{
    [CompileTarget(ECompileTarget.Chambersite)]
    [DefineClass, NodeIcon("ViewDefinition.png")]
    [RequireParent(typeof(NamespaceAlike))]
    public class ViewDefinition : TreeNode
    {
        [JsonConstructor]
        private ViewDefinition() : base() { }
        public ViewDefinition(Document workspace) : this(workspace, "", "ViewType.Interface") { }
        public ViewDefinition(Document workspace, string internalName, string viewType) : base(workspace)
        {
            InternalName = internalName;
            ViewType = viewType;
        }

        [JsonIgnore, NodeAttribute, DefaultValue("")]
        public string InternalName
        {
            get => CheckAttr().attrValue;
            set => CheckAttr().attrValue = value;
        }

        [JsonIgnore, NodeAttribute, DefaultValue("")]
        public string ViewType
        {
            get => CheckAttr("ViewType", "ViewType").attrValue;
            set => CheckAttr("ViewType", "ViewType").attrValue = value;
        }

        public override string ToString()
        {
            return $"Define View of type '{ViewType}' and name '{InternalName}'";
        }

        public override IEnumerable<string> ToChambersite(int spacing)
        {
            string sp = Indent(spacing);
            yield return sp + $"[InternalName(\"{InternalName}\"), ViewType({ViewType})]\n";
            yield return sp + CBS_NewView(InternalName);
            yield return sp + "{\n";
            foreach (var a in base.ToChambersite(spacing + 1))
            {
                yield return a;
            }
            yield return sp + "}\n";
        }

        public override object Clone()
        {
            ViewDefinition cloned = new(ParentWorkspace);
            cloned.CopyData(this);
            return cloned;
        }
    }
}
