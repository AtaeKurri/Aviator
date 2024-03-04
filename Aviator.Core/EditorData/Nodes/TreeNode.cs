﻿using System;
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
using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.Core.EditorData.EditorTraces.Traces;
using System.Xml.Linq;
using System.Windows.Controls;

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
        public string DisplayString { get => ToString(); }

        [JsonIgnore]
        private TreeNode parent = null;

        [JsonIgnore]
        public TreeNode Parent { get => parent; }

        [JsonIgnore]
        public Document ParentWorkspace = null;

        [JsonIgnore]
        protected bool Activated = false;

        [JsonIgnore]
        private TreeNode LinkedPrevious = null;
        [JsonIgnore]
        private TreeNode LinkedNext = null;

        [JsonIgnore]
        public TreeNodeMeta MetaData { get; set; }

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

        public void RaiseRemove(OnRemoveEventArgs e)
        {
            if (e.Parent == null || e.Parent.Activated)
            {
                if (!isBanned)
                {
                    OnVirtualRemove?.Invoke(e);
                }
                OnRemove?.Invoke(e);
                OnCreateEventArgs args = new OnCreateEventArgs { Parent = this };
                foreach (TreeNode node in Children)
                    node.RaiseRemove(e);
            }
        }

        public void RaiseVirtuallyRemove(OnRemoveEventArgs e)
        {
            if (Activated && isBanned)
            {
                OnVirtualRemove?.Invoke(e);
                foreach (TreeNode node in Children)
                    node.RaiseVirtuallyRemove(e);
            }
        }

        #endregion

        protected TreeNode()
        {
            MetaData = new(this);
            Children = null;
            Attributes = null;
            isExpanded = true;
            PropertyChanged += new PropertyChangedEventHandler(CheckTrace);
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
            foreach (TreeNode t in children)
            {
                if (!t.isBanned)
                {
                    foreach (var a in t.ToLua(spacing))
                    {
                        yield return a;
                    }
                }
            }
        }

        public virtual IEnumerable<string> ToChambersite(int spacing)
        {
            return ToChambersite(spacing, children);
        }

        protected IEnumerable<string> ToChambersite(int spacing, IEnumerable<TreeNode> children)
        {
            foreach (TreeNode t in children)
            {
                if (!t.isBanned)
                {
                    foreach (var a in t.ToChambersite(spacing))
                    {
                        yield return a;
                    }
                }
            }
        }

        #endregion
        #region Event Handlers

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

        public void FixParentDocument(Document document)
        {
            ParentWorkspace = document;
            foreach (TreeNode node in Children)
                node.FixParentDocument(document);
        }

        #endregion
        #region Children
        
        public bool ValidateChild(TreeNode sourceNode, TreeNode nodeToValidate)
        {
            if (MetaData.IsFolder)
                return Parent.ValidateChild(sourceNode, nodeToValidate);
            if (sourceNode.MetaData.IsInfertile)
                return false;
            if (!CheckRequiredParentsValidation(nodeToValidate))
                return false;

            Stack<TreeNode> stack = [];
            stack.Push(sourceNode);
            TreeNode cur;
            while (stack.Count != 0)
            {
                cur = stack.Pop();
                if (!cur.CheckRequiredAncestorValidation(cur, sourceNode.Parent, this, null)) return false;
                foreach (TreeNode t in cur.Children)
                    stack.Push(t);
            }
            return true;
        }

        private bool CheckRequiredParentsValidation(TreeNode newNode)
        {
            if (newNode == null)
                return false;
            if (newNode.MetaData.RequireParent == null)
                return true;
            foreach (Type type in newNode.MetaData.RequireParent)
            {
                if (GetType().Equals(type))
                    return true;
            }
            return false;
        }

        private bool CheckRequiredAncestorValidation(TreeNode Beg1, TreeNode End1, TreeNode Beg2, TreeNode End2)
        {
            Type[][] ts = MetaData.RequireAncestor;
            if (ts == null)
                return true;
            List<Type[]> toSatisfiedGroups = ts.ToList();
            List<Type> Satisfied = [];
            List<Type[]> toRemove = [];
            while (Beg1 != End1)
            {
                foreach (Type[] t1 in ts)
                {
                    foreach (Type t2 in t1)
                    {
                        if (Beg1.GetType().Equals(t2))
                            Satisfied.Add(t2);
                    }
                }
                foreach (Type[] t1 in toSatisfiedGroups)
                {
                    foreach (Type t2 in t1)
                    {
                        foreach (Type t3 in Satisfied)
                        {
                            if (t2 == t3 && !toRemove.Contains(t1))
                                toRemove.Add(t1);
                        }
                    }
                }
                foreach (Type[] t1 in toRemove)
                {
                    toSatisfiedGroups.Remove(t1);
                }
                if (toSatisfiedGroups.Count == 0)
                    return true;
                Satisfied.Clear();
                toRemove.Clear();
                Beg1 = Beg1.Parent;
            }
            while (Beg2 != End2)
            {
                foreach (Type[] t1 in ts)
                {
                    foreach (Type t2 in t1)
                    {
                        if (Beg2.GetType().Equals(t2))
                            Satisfied.Add(t2);
                    }
                }
                foreach (Type[] t1 in toSatisfiedGroups)
                {
                    foreach (Type t2 in t1)
                    {
                        foreach (Type t3 in Satisfied)
                        {
                            if (t2 == t3 && !toRemove.Contains(t1))
                                toRemove.Add(t1);
                        }
                    }
                }
                foreach (Type[] t1 in toRemove)
                {
                    toSatisfiedGroups.Remove(t1);
                }
                if (toSatisfiedGroups.Count == 0)
                    return true;
                Satisfied.Clear();
                toRemove.Clear();
                Beg2 = Beg2.Parent;
            }
            return false;
        }

        /// <summary>
        /// Return the first parent that's not a folder.
        /// </summary>
        /// <param name="node">The source node.</param>
        /// <returns>The first non-folder parent; null if not found.</returns>
        public TreeNode GetRealParent(TreeNode node)
        {
            TreeNode p = parent;
            while (p != null && p.MetaData.IsFolder)
            {
                p = p.parent;
            }
            return p;
        }

        public IEnumerable<TreeNode> GetRealChildren()
        {
            foreach (TreeNode n in children)
            {
                if (n.Parent == this)
                {
                    if (n.MetaData.IsFolder)
                    {
                        foreach (TreeNode t in n.GetRealChildren())
                        {
                            yield return t;
                        }
                    }
                    else
                    {
                        yield return n;
                    }
                }
            }
        }

        public bool CanLogicallyDelete()
        {
            if (MetaData.IsFolder)
            {
                foreach (TreeNode t in GetRealChildren())
                {
                    if (t.MetaData.CannotBeDeleted)
                        return false;
                }
                return true;
            }
            else
            {
                return !MetaData.CannotBeDeleted;
            }
        }

        private void ChildrenChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            TreeNode node, previousNode = null;
            if (e.OldItems != null)
            {
                for (int index = 0; index < e.OldItems.Count; index++)
                {
                    node = (TreeNode)e.OldItems[index];
                    node.RaiseRemove(new OnRemoveEventArgs() { Parent = this });
                    if (index + e.OldStartingIndex != 0)
                    {
                        if (previousNode == null)
                            previousNode = Children[index + e.OldStartingIndex - 1];
                        previousNode.LinkedNext = node.LinkedNext;
                        if (node.LinkedNext != null)
                            node.LinkedNext.LinkedPrevious = previousNode;
                    }
                    else
                    {
                        LinkedNext = node.LinkedNext;
                        if (LinkedNext != null)
                            node.LinkedNext.LinkedPrevious = this;
                        previousNode = this;
                    }
                }
            }
            if (e.NewItems != null)
            {
                for (int index = 0; index < e.NewItems.Count; index++)
                {
                    node = (TreeNode)e.NewItems[index];
                    node.RaiseCreate(new OnCreateEventArgs() { Parent = this });
                    node.parent = this;
                    if (index + e.NewStartingIndex != 0)
                    {
                        if (previousNode == null)
                            previousNode = Children[index + e.NewStartingIndex - 1];
                        node.LinkedPrevious = previousNode;
                        node.LinkedNext = previousNode.LinkedNext;
                        if (previousNode.LinkedNext != null)
                            previousNode.LinkedNext.LinkedPrevious = node;
                        previousNode.LinkedNext = node;
                    }
                    else
                    {
                        node.LinkedPrevious = this;
                        node.LinkedNext = LinkedNext;
                        if (LinkedNext != null)
                            LinkedNext.LinkedPrevious = node;
                        LinkedNext = node;
                    }
                    previousNode = node;
                }
            }
        }

        public void AddChild(TreeNode node)
        {
            Children.Add(node);
        }

        public void InsertChild(TreeNode node, int index)
        {
            Children.Insert(index, node);
        }

        public void RemoveChild(TreeNode node)
        {
            Children.Remove(node);
        }

        public void ClearChildSelection()
        {
            isSelected = false;
            foreach (TreeNode child in children)
                child.ClearChildSelection();
        }

        public abstract object Clone();

        public IEnumerator<TreeNode> GetForwardNodes()
        {
            TreeNode node = this;
            while (node.LinkedNext != null)
            {
                yield return node;
                node = node.LinkedNext;
            }
        }

        public IEnumerator<TreeNode> GetBackwardNodes()
        {
            TreeNode node = this;
            while (node.LinkedPrevious != null)
            {
                yield return node;
                node = node.LinkedPrevious;
            }
        }

        public TreeNode GetNearestEdited()
        {
            TreeNode node = parent;
            if (node != null)
            {
                int id = node.children.IndexOf(this) - 1;
                if (id >= 0)
                {
                    node = node.children[id];
                }
                return node;
            }
            else
            {
                return this;
            }
        }

        #endregion
        #region Traces

        public List<EditorTrace> GetTraces()
        {
            return [];
        }

        public void CheckTrace(object sender, PropertyChangedEventArgs e)
        {
            List<EditorTrace> traces = [];
            if (!isBanned) traces = GetTraces();
            Traces.Clear();
             
            foreach (EditorTrace trace in traces)
                Traces.Add(trace);
            EditorTraceContainer.UpdateTraces(this);
        }

        #endregion
        #region Common

        public string Indent(int length)
        {
            return "".PadLeft(4 * length);
        }

        #endregion
        #region ToChambersite

        

        public string CBS_NewView(string name)
        {
            return $"public class {name} : View\n";
        }

        #endregion
        #region ToLua



        #endregion
    }
}
