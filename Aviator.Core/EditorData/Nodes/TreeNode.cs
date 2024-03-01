using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using Aviator.Core.EditorData.Documents;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Aviator.Core.EditorData.EditorTraces;

namespace Aviator.Core.EditorData.Nodes
{
    [Serializable]
    public abstract class TreeNode : INotifyPropertyChanged, ICloneable, ITraceThrowable
    {
        #region Properties

        [JsonIgnore]
        public ObservableCollection<NodeAttribute> attributes = new ObservableCollection<NodeAttribute>();

        public ObservableCollection<NodeAttribute> Attributes
        {
            get => attributes;
            set
            {
                if (value == null)
                {
                    attributes = new ObservableCollection<NodeAttribute>();
                    attributes.CollectionChanged += new NotifyCollectionChangedEventHandler(AttributesChanged);
                }
                else
                {
                    throw new InvalidOperationException("Cannot add a null attribute to a TreeNode.");
                }
            }
        }

        [JsonIgnore]
        public int AttributeCount { get => attributes.Count; }

        [JsonIgnore]
        private bool isExpanded;

        [JsonProperty, DefaultValue(true)]
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        [JsonIgnore]
        public bool isSelected;
        [JsonProperty, DefaultValue(false)]
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        [JsonIgnore]
        public bool isBanned;
        [JsonProperty, DefaultValue(false)]
        public bool IsBanned
        {
            get => isBanned;
            set
            {
                isBanned = value;
                RaisePropertyChanged("IsBanned");
            }
        }

        [JsonIgnore]
        public bool CanBeDeleted = false;
        [JsonIgnore]
        public bool CanBeBanned = false;

        [JsonIgnore]
        public string DisplayString { get => ToString(); }

        [JsonIgnore]
        private TreeNode parent = null;

        [JsonIgnore]
        public TreeNode Parent { get => parent; }

        [JsonIgnore]
        public Document ParentWorkspace = null;

        /// <summary>
        /// Indicate that a node cannot have children.
        /// </summary>
        public bool IsInfertile = false;

        [JsonIgnore]
        private ObservableCollection<TreeNode> children;

        [JsonIgnore]
        public ObservableCollection<TreeNode> Children
        {
            get => children;
            set
            {
                if (value == null)
                {
                    children = [];
                    children.CollectionChanged += new NotifyCollectionChangedEventHandler(ChildrenChanged);
                }
                else
                {
                    throw new InvalidOperationException("Cannot set a Children as a null value.");
                }
            }
        }

        [JsonIgnore]
        public ObservableCollection<EditorTrace> Traces { get; private set; } = [];

        #endregion
        #region Events

        private event OnCreateNodeHandler OnCreate;
        private event OnCreateNodeHandler OnVirtualCreate;
        private event OnRemoveNodeHandler OnRemove;
        private event OnRemoveNodeHandler OnVirtualRemove;

        public void RaiseCreate(OnCreateEventArgs e)
        {
            if (e.Parent == null)
            {
                if (!isBanned)
                    OnVirtualCreate?.Invoke(e);
                OnCreate?.Invoke(e);
                OnCreateEventArgs args = new() { Parent = this };
                foreach (TreeNode node in Children)
                    node.RaiseCreate(args);
            }
        }

        public void RaiseVirtuallyCreate(OnCreateEventArgs e)
        {
            if (!isBanned)
            {
                OnVirtualCreate?.Invoke(e);
                foreach (TreeNode node in Children)
                    node.RaiseVirtuallyCreate(e);
            }
        }

        #endregion

        protected TreeNode()
        {
            Children = null;
            Attributes = null;
            isExpanded = true;
        }

        public TreeNode(Document workspace)
            : this()
        {
            ParentWorkspace = workspace;
        }

        public override string ToString()
        {
            return $"No DisplayString set. Don't forget to override the 'ToString()' method, dumbass. Node: {this.GetType()}";
        }

        #region ToCompiler

        public virtual IEnumerable<string> ToLua(int spacing)
        {
            return ToLua(spacing, children);
        }

        protected IEnumerable<string> ToLua(int spacing, IEnumerable<TreeNode> children)
        {
            return null;
        }

        public virtual IEnumerable<string> ToChambersite(int spacing)
        {
            return ToChambersite(spacing, children);
        }

        protected IEnumerable<string> ToChambersite(int spacing, IEnumerable<TreeNode> children)
        {
            return null;
        }

        #endregion
        #region Event Handlers

        private void ChildrenChanged(object o, NotifyCollectionChangedEventArgs e)
        {

        }

        private void AttributesChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            NodeAttribute attr;
            if (e.NewItems != null)
            {
                foreach (NodeAttribute na in e.NewItems)
                {
                    attr = na;
                    if (attr != null)
                        attr.ParentNode = this;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
        #region Serialization

        public void SerializeToFile(StreamWriter sw, int level)
        {
            sw.WriteLine($"{level},{TreeSerializer.SerializeTreeNode(this)}");
            string dfs = $"{level},{TreeSerializer.SerializeTreeNode(this)}";
            foreach (TreeNode node in Children)
            {
                node.SerializeToFile(sw, level + 1);
            }
        }

        #endregion
        #region Attributes

        public NodeAttribute? GetAttr(string name)
        {
            var attrs = from NodeAttribute na in attributes
                        where na != null && !string.IsNullOrEmpty(na.AttrName) && na.AttrName == name
                        select na;
            if (attrs != null && attrs.Count() > 0) return attrs.First();
            return null;
        }

        public NodeAttribute CheckAttr([CallerMemberName] string name = "", string editWindow = "")
        {
            NodeAttribute na = GetAttr(name);
            if (na == null)
            {
                na = new NodeAttribute(name, this, editWindow);
                attributes.Add(na);
            }
            na.EditWindow = editWindow;
            return na;
        }

        #endregion
        #region Data Handle

        public void CopyData(TreeNode source)
        {
            var attributes = from NodeAttribute attr in source.attributes select (NodeAttribute)attr.Clone();
            var childrens = from TreeNode childn in source.children select (TreeNode)childn.Clone();
            Attributes = null;
            foreach (NodeAttribute na in attributes)
                this.attributes.Add(na);
            Children = null;
            foreach (TreeNode tn in childrens)
                this.children.Add(tn);
            parent = source.Parent;
            isExpanded = source.isExpanded;
        }

        public void AddChild(TreeNode node)
        {
            Children.Add(node);
        }

        public abstract object Clone();

        #endregion
        #region Traces

        public void CheckTrace(object sender, PropertyChangedEventArgs e)
        {
            List<EditorTrace> traces = [];
            GetTraces(); // TODO: Check if isBanned
            Traces.Clear();
            foreach (EditorTrace trace in traces)
                Traces.Add(trace);
            EditorTraceContainer.UpdateTraces(this);
        }

        public List<EditorTrace> GetTraces()
        {
            return [];
        }

        #endregion
    }
}
