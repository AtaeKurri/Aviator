using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes;
using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.Nodes.EditorNodes.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aviator.Nodes.EditorNodes.Chambersite
{
    [CompileTarget(ECompileTarget.Chambersite)]
    [CannotBeDeleted, NodeIcon("NamespaceDef.png")]
    public class RootNamespaceDefinition : TreeNode, INamespaceDefinition
    {
        [JsonIgnore]
        public string Name {
            get => ParentWorkspace.Configuration.ProjectInternalName;
            set => throw new NotImplementedException();
        }

        [JsonConstructor]
        private RootNamespaceDefinition() : base() { }
        public RootNamespaceDefinition(Document workspace) : base(workspace) { }

        public override string ToString()
        {
            return $"Define a new Namespace as '{ParentWorkspace.Configuration.ProjectInternalName}'";
        }

        public override IEnumerable<string> ToChambersite(int spacing)
        {
            string sp = Indent(spacing);
            yield return sp + CBS_NewNamespace(ParentWorkspace.Configuration.ProjectInternalName);
            yield return sp + "{\n";
            foreach (var a in base.ToChambersite(spacing + 1))
            {
                yield return a;
            }
            yield return sp +"}\n";
        }

        public string CBS_NewNamespace(string name)
        {
            return $"namespace {name}\n";
        }

        public override object Clone()
        {
            NamespaceDefinition namespaceDefinition = new NamespaceDefinition(ParentWorkspace);
            namespaceDefinition.CopyData(this);
            return namespaceDefinition;
        }
    }
}
