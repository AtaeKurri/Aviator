using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Aviator.Commands
{
    public class EditorCommands
    {
        #region Routed commands

        public static RoutedUICommand OpenSettings { get; set; } = new RoutedUICommand("Open Settings", "OpenSettings", typeof(EditorCommands));
        public static RoutedUICommand RunProject { get; set; }
            = new RoutedUICommand("Run Project", "RunProject", typeof(EditorCommands), CreateGesture(new KeyGesture(Key.F5)));

        #endregion

        public static InputGestureCollection CreateGesture(KeyGesture gesture)
        {
            return [gesture];
        }
    }
}
