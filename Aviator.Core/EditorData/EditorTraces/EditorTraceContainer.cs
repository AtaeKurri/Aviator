using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.EditorTraces
{
    public static class EditorTraceContainer
    {
        public static ObservableCollection<EditorTrace> Traces { get; set; } = new ObservableCollection<EditorTrace>();

        public static void UpdateTraces(ITraceThrowable source)
        {
            var traces = from EditorTrace editorTrace in Traces
                         where editorTrace.Source == source
                         select editorTrace;
            List<EditorTrace> editorTraces = new(traces);
            foreach (EditorTrace et in editorTraces)
                Traces.Remove(et);
            foreach (EditorTrace et in source.Traces)
                Traces.Add(et);
        }

        public static bool ContainSeverity(TraceSeverity severity)
        {
            foreach (EditorTrace trace in Traces)
                if (trace.TraceSeverity == severity) return true;
            return false;
        }
    }
}
