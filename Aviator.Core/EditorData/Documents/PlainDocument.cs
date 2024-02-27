using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData.Documents
{
    public class PlainDocument : Document
    {
        public PlainDocument(int hash, bool suppressMessage = false)
            : base(hash, suppressMessage)
        {

        }
    }
}
