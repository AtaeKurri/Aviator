using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodePicker
{
    public class NodePickerItem
    {
        public bool IsSeparator { get; set; }
        public string Tag { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Tooltip { get; set; } = "";

        public AbstractNodePicker.AddNode AddNodeMethod { get; set; }

        public NodePickerItem(bool isSeparator)
        {
            IsSeparator = isSeparator;
        }

        public NodePickerItem(string tag, string image, string tooltip, AbstractNodePicker.AddNode addNodeMethod)
            : this(false)
        {
            Tag = tag;
            Icon = $"/Aviator;component/Images/{(string.IsNullOrEmpty(image) ? "Unknown.png" : image)}";
            Tooltip = tooltip;
            AddNodeMethod = addNodeMethod;
        }
    }
}
