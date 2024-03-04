using Aviator.Core.EditorData.Nodes;
using Aviator.Core.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.EditorTraces.Traces
{
    public class WrongCompileTargetTrace : EditorTrace
    {
        public ECompileTarget ExpectedCompileTarget { get; set; }
        public ECompileTarget ActualCompileTarget { get; set; }

        public WrongCompileTargetTrace(ITraceThrowable source, ECompileTarget actualTarget, ECompileTarget expectedTarget)
            : base(TraceSeverity.Error, source)
        {
            ExpectedCompileTarget = expectedTarget;
            ActualCompileTarget = actualTarget;
        }

        public override object Clone()
        {
            return new WrongCompileTargetTrace(Source, ActualCompileTarget, ExpectedCompileTarget);
        }

        public override void Invoke()
        {
            return;
        }

        public override string ToString()
        {
            return $"Cannot compile to the selected target. Expected: '{ExpectedCompileTarget}' ; Got '{ActualCompileTarget}'";
        }
    }
}
