using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.EditorTraces
{
    public interface ITraceThrowable
    {
        ObservableCollection<EditorTrace> Traces { get; }
        void CheckTrace(object sender, PropertyChangedEventArgs e);
        List<EditorTrace> GetTraces();
    }
}
