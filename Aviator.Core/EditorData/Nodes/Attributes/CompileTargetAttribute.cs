using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Nodes.Attributes
{
    public enum ECompileTarget
    {
        LuaSTG,
        Chambersite,
        All
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CompileTargetAttribute(ECompileTarget compileTarget) : Attribute
    {
        private readonly ECompileTarget compileTarget = compileTarget;
        public virtual ECompileTarget CompileTarget { get => compileTarget; }
    }
}
