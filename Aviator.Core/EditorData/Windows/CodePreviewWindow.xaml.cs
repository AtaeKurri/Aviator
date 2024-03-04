using Aviator.Core.EditorData.Nodes.Attributes;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Aviator.Core.EditorData.Windows
{
    /// <summary>
    /// Logique d'interaction pour CodePreviewWindow.xaml
    /// </summary>
    public partial class CodePreviewWindow : Window
    {
        public CodePreviewWindow(string code, ECompileTarget compileTarget)
        {
            InitializeComponent();
            codeText.Text = code;
            codeText.SyntaxHighlighting = (compileTarget == ECompileTarget.LuaSTG)
                ? ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("Lua")
                : ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#");
        }
    }
}
