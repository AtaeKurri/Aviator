using Aviator.Core.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.EditorTraces.Traces
{
    public class LuaSTGPathNotSetTrace : EditorTrace
    {
        public LuaSTGPathNotSetTrace(ITraceThrowable source)
            : base(TraceSeverity.Error, source)
        {
        }

        public override object Clone()
        {
            return new LuaSTGPathNotSetTrace(Source);
        }

        public override void Invoke()
        {
            return;
        }

        public override string ToString()
        {
            return $"LuaSTG Executable Path not set. Select your executable in the Project Settings.";
        }
    }
}
