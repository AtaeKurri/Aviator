using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Documents
{
    public class DocumentCollection : ObservableCollection<Document>
    {
        public int MaxHash { get; private set; } = 0;

        public new void Add(Document workspace)
        {
            base.Add(workspace);
            workspace.Parent = this;
            MaxHash++;
        }
    }
}
