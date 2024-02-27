using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Nodes
{
    [Serializable]
    public class NodeAttribute : INotifyPropertyChanged, ICloneable
    {
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string value;

        [JsonIgnore]
        public virtual string Value
        {
            get => this.value;
            set
            {
                this.value = value;
                RaisePropertyChanged("Value");
                ParentNode?.RaisePropertyChanged("DisplayString");
            }
        }

        [JsonIgnore]
        public TreeNode ParentNode;

        private string editWindow;
        [JsonProperty]
        public string EditWindow
        {
            get => editWindow;
            set
            {
                editWindow = value;
                RaisePropertyChanged("EditWindow");
            }
        }

        [JsonConstructor]
        public NodeAttribute() { }

        public NodeAttribute(string name, TreeNode parent)
        {
            Name = name;
            ParentNode = parent;
            Value = "";
        }

        public NodeAttribute(string name, string value = "", string editWindow = "")
            : this()
        {
            Name = name;
            Value = value;
            EditWindow = editWindow;
        }

        public NodeAttribute(string name, TreeNode parent, string editWindow)
            : this(name, parent)
        {
            EditWindow = EditWindow;
        }

        public NodeAttribute(string name, string value, TreeNode parent)
            : this(name, parent)
        {
            Value = value;
        }

        public NodeAttribute(string name, TreeNode parent, string editWindow, string value)
            : this(name, value, parent)
        {
            EditWindow = editWindow;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public virtual object Clone()
        {
            return new NodeAttribute(Name, ParentNode) { Value = this.Value, EditWindow = this.EditWindow };
        }
    }
}
