using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes;
using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.Nodes.EditorNodes.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aviator.Nodes.EditorNodes.Chambersite
{
    [CompileTarget(ECompileTarget.Chambersite)]
    public class NamespaceDefinition : TreeNode
    {
        [JsonConstructor]
        private NamespaceDefinition() : base() { }

        public NamespaceDefinition(Document workspace) : this(workspace, Path.GetFileNameWithoutExtension(workspace.FilePath)) { }
        public NamespaceDefinition(Document workspace, string name) : base(workspace)
        {
            Name = name;
        }

        [JsonIgnore, DefaultValue("Namespace")]
        public string Name
        {
            get => CheckAttr().attrValue;
            set => CheckAttr().attrValue = value;
        }

        public override string ToString()
        {
            return $"Define a '{Name}' Namespace";
        }

        public override IEnumerable<string> ToChambersite(int spacing)
        {
            CBS_NewNamespace(Name);
            yield return $"// Namespace Definition: {Name}";
        }

        public override object Clone()
        {
            NamespaceDefinition namespaceDefinition = new NamespaceDefinition(ParentWorkspace);
            namespaceDefinition.CopyData(this);
            return namespaceDefinition;
        }
    }
}
