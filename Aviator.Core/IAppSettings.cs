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
        public int CompileTarget { get; }
        public string LSTGExecutablePath { get; }
    }
}
