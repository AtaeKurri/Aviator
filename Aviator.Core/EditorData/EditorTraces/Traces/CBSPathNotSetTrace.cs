using Aviator.Core.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.EditorTraces.Traces
{
    public class CBSPathNotSetTrace : EditorTrace
    {
        public CBSPathNotSetTrace(ITraceThrowable source)
            : base(TraceSeverity.Error, source)
        {
        }

        public override object Clone()
        {
            return new CBSPathNotSetTrace(Source);
        }

        public override void Invoke()
        {
            return;
        }

        public override string ToString()
        {
            return $"Chambersite output not set. Select a folder in the Project Settings.";
        }
    }
}
