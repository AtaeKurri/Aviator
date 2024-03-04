using Aviator.Core.EditorData.Nodes;
using Aviator.InputWindows.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.InputWindows
{
    public static class InputWindowSelectorRegister
    {
        public static void RegisterComboBoxText(Dictionary<string, string[]> target)
        {
            target.Add("ViewType",
                ["ViewType.Background", "ViewType.Stage", "ViewType.Interface", "ViewType.Menu"]);
            target.Add("CBSMethods",
                ["Initialize", "LoadResources", "BeforeUpdate", "Update", "AfterUpdate", "Draw"]);
        }

        public static void RegisterInputWindow(Dictionary<string, Func<NodeAttribute, string, IInputWindow>> target)
        {
            target.Add("ViewType", (src, tar) => new Selector(tar, InputWindowSelector.SelectComboBox("ViewType"), "Input View Type"));
            target.Add("CBSMethods", (src, tar) => new Selector(tar, InputWindowSelector.SelectComboBox("CBSMethods"), "Method Override"));
        }
    }
}
