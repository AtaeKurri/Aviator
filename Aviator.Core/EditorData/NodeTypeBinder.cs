using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Core.EditorData
{
    public class NodeTypeBinder : ISerializationBinder
    {
        DefaultSerializationBinder @default = new DefaultSerializationBinder();

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            @default.BindToName(serializedType, out assemblyName, out typeName);
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            return @default.BindToType("Aviator.Nodes", typeName);
        }
    }
}
