using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodePicker
{
    public abstract class AbstractNodePicker : IEnumerable<NodePickerTab>
    {
        public delegate void AddNode();

        public readonly IMainWindow ParentWindow;

        public ObservableCollection<NodePickerTab> NodePickerTabs = [];

        public Dictionary<string, AddNode> NodeFuncs = [];

        public AbstractNodePicker(IMainWindow parentWindow)
        {
            ParentWindow = parentWindow;
            Initialize();
            InitializeData();
        }

        public abstract void Initialize();

        private void InitializeData()
        {
            NodeFuncs = [];
            foreach (NodePickerTab nodeTab in NodePickerTabs)
            {
                foreach (NodePickerItem nodeItem in nodeTab)
                {
                    if (!nodeItem.IsSeparator)
                    {
                        NodeFuncs.Add(nodeItem.Tag, nodeItem.AddNodeMethod);
                    }
                }
            }
        }

        public IEnumerator<NodePickerTab> GetEnumerator()
        {
            return NodePickerTabs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
