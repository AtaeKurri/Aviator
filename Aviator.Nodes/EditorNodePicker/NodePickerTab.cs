using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodePicker
{
    public class NodePickerTab : IEnumerable<NodePickerItem>
    {
        public NodePickerTab(string header)
        {
            Header = header;
        }

        public string Header { get; set; }
        public ObservableCollection<NodePickerItem> Items { get; set; } = new();

        public void AddNode(NodePickerItem item)
        {
            Items.Add(item);
        }

        public IEnumerator<NodePickerItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
