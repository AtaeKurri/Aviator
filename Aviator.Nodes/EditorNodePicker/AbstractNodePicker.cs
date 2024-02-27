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

        public AbstractNodePicker(IMainWindow parentWindow)
        {
            ParentWindow = parentWindow;
            Initialize();
        }

        public abstract void Initialize();

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
