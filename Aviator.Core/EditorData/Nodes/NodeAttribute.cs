using Aviator.Core.EditorData.Commands;
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
        protected string attrName;
        [JsonIgnore]
        public string AttrName => attrName;

        [JsonProperty]
        public string attrValue;

        [JsonIgnore]
        public virtual string AttrValue
        {
            get => attrValue;
            set
            {
                attrValue = value;
                RaisePropertyChanged("Value");
                ParentNode?.RaisePropertyChanged("DisplayString");
            }
        }

        [JsonIgnore]
        protected TreeNode parent;

        [JsonIgnore]
        public TreeNode ParentNode
        {
            get => parent;
            set => parent = value;
        }

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

        [JsonIgnore]
        public NodeAttribute This => this;

        [JsonConstructor]
        public NodeAttribute() { }

        public NodeAttribute(string name, TreeNode parent)
        {
            attrName = name;
            this.parent = parent;
            attrValue = "";
        }

        public NodeAttribute(string name, string value = "", string editWindow = "")
            : this()
        {
            attrName = name;
            attrValue = value;
            EditWindow = editWindow;
        }

        public NodeAttribute(string name, TreeNode parent, string editWindow)
            : this(name, parent)
        {
            EditWindow = editWindow;
        }

        public NodeAttribute(string name, string value, TreeNode parent)
            : this(name, parent)
        {
            attrValue = value;
        }

        public NodeAttribute(string name, TreeNode parent, string editWindow, string value)
            : this(name, value, parent)
        {
            EditWindow = editWindow;
        }

        [JsonIgnore]
        public string AttributeInput_InvokeCommand
        {
            get => attrValue;
            set => parent.ParentWorkspace.AddAndExecuteCommand(new EditAttributeCommand(this, attrValue, value));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public virtual object Clone()
        {
            return new NodeAttribute(attrName, parent) { attrValue = this.attrValue, EditWindow = this.EditWindow };
        }
    }
}
