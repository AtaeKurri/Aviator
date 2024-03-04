using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes;
using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.Nodes.EditorNodes.Common;
using Aviator.Nodes.EditorNodes.TypeAlike;
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
    [CompileTarget(ECompileTarget.Chambersite), NodeIcon("NamespaceDef.png")]
    [RequireParent(typeof(NamespaceAlike))]
    public class NamespaceDefinition : TreeNode, INamespaceDefinition
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
            return $"Define a new Namespace as '{Name}'";
        }

        public override IEnumerable<string> ToChambersite(int spacing)
        {
            // Revoir ce code, c'est pas censé avoir des enfants directement en code, mais plus un truc qui est ajouté à chaque classe, revoir ça.
            string sp = Indent(spacing);
            yield return sp + CBS_NewNamespace(Name);
            yield return sp + "{\n";
            foreach (var a in base.ToChambersite(spacing + 1))
            {
                yield return a;
            }
            yield return sp + "}\n";
        }

        public string CBS_NewNamespace(string name)
        {
            string constructedNamespace = "";
            TreeNode checkParents = this.Parent;
            while (checkParents != null)
            {
                if (CheckIfNodeIsNamespace(checkParents))
                    constructedNamespace += $"{(checkParents as INamespaceDefinition).Name}.";
                checkParents = checkParents.Parent;
            }
            return $"namespace {constructedNamespace}{name}\n";
        }

        private bool CheckIfNodeIsNamespace(TreeNode node)
        {
            if (GetType().Equals(typeof(NamespaceDefinition)) || GetType().Equals(typeof(RootNamespaceDefinition)))
                    return true;
            return false;
        }

        public override object Clone()
        {
            NamespaceDefinition namespaceDefinition = new NamespaceDefinition(ParentWorkspace);
            namespaceDefinition.CopyData(this);
            return namespaceDefinition;
        }
    }
}
