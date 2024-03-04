using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Nodes.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeIconAttribute(string imagePath) : Attribute
    {
        private readonly string path = imagePath;

        [DefaultValue("unknown.png")]
        public virtual string Path { get => path; }
    }
}
