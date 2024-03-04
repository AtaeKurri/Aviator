using Aviator.Core.EditorData.Nodes;
using Aviator.InputWindows.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.InputWindows
{
    public static class InputWindowSelector
    {
        public static readonly string[] nullSelection = [];
        public static readonly Func<NodeAttribute, string, InputWindow> nullWindow = (e, s) => new SingleLineInput(s);

        private static readonly Dictionary<string, string[]> comboBox = [];
        private static readonly Dictionary<string, Func<NodeAttribute, string, IInputWindow>> windowGenerator = [];

        public static void Register()
        {
            InputWindowSelectorRegister.RegisterComboBoxText(comboBox);
            InputWindowSelectorRegister.RegisterInputWindow(windowGenerator);
        }

        public static void AfterRegister()
        {
            List<string> vs = new(windowGenerator.Keys);
            vs.Add("");
            vs.Sort();
            comboBox.Add("editWindow", vs.ToArray());
            windowGenerator.Add("editWindow", (src, tar) => new Selector(tar
                 , SelectComboBox("editWindow"), "Input Edit Window"));
        }

        public static string[] SelectComboBox(string editWindow)
        {
            return comboBox.GetOrDefault(editWindow, nullSelection);
        }

        public static IInputWindow SelectInputWindow(NodeAttribute source, string name, string toEdit)
        {
            IInputWindow iw = windowGenerator.GetOrDefault(name, nullWindow)(source, toEdit);
            iw.AppendTitle(source.AttrName);
            return iw;
        }
    }

    public static class DictionaryExtension
    {
        public static U GetOrDefault<T, U>(this Dictionary<T, U> dict, T inValue, U defaultValue)
        {
            U value = defaultValue;
            if (dict.TryGetValue(inValue, out U val)) value = val;
            return value;
        }
    }
}
