using Aviator.Core.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Nodes
{
    public class TreeNodeMeta
    {
        public bool IsFolder { get; set; }

        /// <summary>
        /// This node can't have children.
        /// </summary>
        public bool IsInfertile { get; set; }

        /// <summary>
        /// The compile target of this node. <see cref="ECompileTarget"/>
        /// </summary>
        public ECompileTarget CompileTarget { get; set; }

        /// <summary>
        /// This node can't be deleted nor banned.
        /// </summary>
        public bool CannotBeDeleted { get; set; }

        /// <summary>
        /// The (internal) path to the icon displayed on both the nodepickerbox and on the treeview.
        /// </summary>
        public string Icon { get; set; }

        public Type[] RequireParent { get; set; } = null;
        public Type[][] RequireAncestor { get; set; } = null;

        public TreeNodeMeta(TreeNode node)
        {
            Type type = node.GetType();

            IsInfertile = type.IsDefined(typeof(IsInfertileAttribute), false);
            IsFolder = type.IsDefined(typeof(IsFolderAttribute), false);
            CompileTarget = type.GetCustomAttribute<CompileTargetAttribute>().CompileTarget;
            CannotBeDeleted = type.IsDefined(typeof(CannotBeDeletedAttribute), false);
            string pathToImage = type.GetAttributeValue((NodeIconAttribute img) => img.Path);
            Icon = $"/Aviator;component/Images/{(string.IsNullOrEmpty(pathToImage) ? "Unknown.png": pathToImage)}";
            RequireParent = GetTypes(type.GetCustomAttribute<RequireParentAttribute>()?.RequiredTypes);

            var attrs = type.GetCustomAttributes<RequireAncestorAttribute>();
            if (attrs.Count() != 0)
            {
                RequireAncestor = (from RequireAncestorAttribute at in attrs
                                        select GetTypes(at.RequiredTypes)).ToArray();
            }
        }

        public static Type[] GetTypes(Type[] src)
        {
            if (src != null)
            {
                LinkedList<Type> types = new LinkedList<Type>();
                Type it = typeof(IEnumerable<Type>);
                foreach (Type t in src)
                {
                    if (it.IsAssignableFrom(t))
                    {
                        IEnumerable<Type> o = t.GetConstructor(Type.EmptyTypes).Invoke([]) as IEnumerable<Type>;
                        foreach (Type ty in o)
                        {
                            types.AddLast(ty);
                        }
                    }
                    else
                    {
                        types.AddLast(t);
                    }
                }
                return types.ToArray();
            }
            return null;
        }
    }

    public static class AttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(
            this Type type,
            Func<TAttribute, TValue> valueSelector)
            where TAttribute : Attribute
        {
            try
            {
                var att = type.GetCustomAttributes(
                    typeof(TAttribute), true
                ).FirstOrDefault() as TAttribute;
                if (att != null)
                {
                    return valueSelector(att);
                }
                return default(TValue);
            }
            catch { return default(TValue); }
        }
    }
}
