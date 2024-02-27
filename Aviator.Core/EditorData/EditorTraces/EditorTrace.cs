using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.EditorTraces
{
    /// <summary>
    /// Indicate the severity of the message trace. Can be filtered by the datagrid.
    /// </summary>
    public enum TraceSeverity
    {
        Info, // Infos from nodes, allows to build or do anything else. 
        Warning, // Warning from nodes, will allow to build but display a message asking if the user wants to continue.
        Error, // Fatal errors, argument mismatch or node specific errors (missing name, ...). Will NOT allow to build.
    }

    public abstract class EditorTrace : ICloneable
    {
        public ITraceThrowable Source { get; private set; }
        public string SourceName { get; private set; }
        public EditorTrace This => this;
        public string Trace => ToString();
        public TraceSeverity TraceSeverity { get; private set; }
        public string Icon { get; }

        public EditorTrace(TraceSeverity severity, ITraceThrowable source)
        {
            TraceSeverity = severity;
            Source = source;
        }

        public EditorTrace(TraceSeverity severity, ITraceThrowable source, string sourceName)
            : this(severity, source)
        {
            SourceName = sourceName;
        }

        public abstract new string ToString();

        public abstract object Clone();
        public abstract void Invoke();
    }
}
