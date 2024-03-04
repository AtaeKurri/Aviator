using Aviator.Nodes.EditorNodes.Chambersite;
using Aviator.Nodes.EditorNodes.Chambersite.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodes.TypeAlike
{
    public class ViewAlike : IEnumerable<Type>
    {
        private static readonly Type[] Types =
        {
            typeof(ViewDefinition)
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
            return this.GetEnumerator();
        }
    }
}
