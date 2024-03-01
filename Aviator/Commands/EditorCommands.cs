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

        public static RoutedUICommand ToBeforeState { get; set; }
            = new RoutedUICommand("Switch to Before insert mode", "ToBeforeState", typeof(EditorCommands), CreateGesture(new KeyGesture(Key.Up, ModifierKeys.Alt, "Alt+Up")));
        public static RoutedUICommand ToChildState { get; set; }
            = new RoutedUICommand("Switch to Child insert mode", "ToChildState", typeof(EditorCommands), CreateGesture(new KeyGesture(Key.Right, ModifierKeys.Alt, "Alt+Right")));
        public static RoutedUICommand ToAfterState { get; set; }
            = new RoutedUICommand("Switch to After insert mode", "ToAfterState", typeof(EditorCommands), CreateGesture(new KeyGesture(Key.Down, ModifierKeys.Alt, "Alt+Down")));

        public static RoutedUICommand NodeAttrMore { get; set; }
            = new RoutedUICommand("Adjust Node Attributes", "NodeAttrMore", typeof(EditorCommands));

        #endregion

        public static InputGestureCollection CreateGesture(KeyGesture gesture)
        {
            return [gesture];
        }
    }
}
