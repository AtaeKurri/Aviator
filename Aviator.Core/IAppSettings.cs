using Aviator.Core.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core
{
    public interface IAppSettings
    {
        public string AuthorName { get; }
        public string LastUsedPath { get; set; }
        public ECompileTarget CompileTarget { get; }
        public string LSTGExecutablePath { get; }
    }
}
