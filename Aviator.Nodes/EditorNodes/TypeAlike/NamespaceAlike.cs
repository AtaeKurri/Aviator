using Aviator.Nodes.EditorNodes.Chambersite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodes.TypeAlike
{
    public class NamespaceAlike : IEnumerable<Type>
    {
        private static readonly Type[] Types =
        {
            typeof(NamespaceDefinition),
            typeof(RootNamespaceDefinition)
        };

        public IEnumerator<Type> GetEnumerator()
        {
            foreach (Type t in Types)
            {
                yield return t;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
