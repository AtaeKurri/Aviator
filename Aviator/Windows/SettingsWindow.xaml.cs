using Aviator.Core.EditorData.Nodes.Attributes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Aviator.Windows
{
    /// <summary>
    /// Logique d'interaction pour SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        private App main = Application.Current as App;

        public Array CompileTargetArray
        {
            get => Enum.GetValues(typeof(ECompileTarget));
        }

        #region VirtualSettingsProperties

        private ECompileTarget compileTarget;
        public ECompileTarget CompileTarget
        {
            get => compileTarget;
            set
            {
                compileTarget = value;
                RaisePropertyChanged("CompileTarget");
            }
        }

        private string luaSTGExecPath;
        public string LuaSTGExecPath
        {
            get => luaSTGExecPath;
            set
            {
                luaSTGExecPath = value;
                RaisePropertyChanged("LuaSTGExecPath");
            }
        }

        #endregion
        #region VirtualSettings

        public ECompileTarget CompileTargetVirtual
        {
            get => main.CompileTarget;
            set => main.CompileTarget = value;
        }
        public string LuaSTGExecPathVirtual
        {
            get => main.LSTGExecutablePath;
            set => main.LSTGExecutablePath = value;
        }

        #endregion

        public SettingsWindow()
        {
            GetSettings();
            InitializeComponent();
        }

        public SettingsWindow(int tabIndex)
            : this()
        {
            switch (tabIndex)
            {
                case 0:
                    GeneralTab.IsSelected = true;
                    break;
                case 1:
                    CompileTab.IsSelected = true;
                    break;
                case 2:
                    ProjectTab.IsSelected = true;
                    break;
                default:
                    GeneralTab.IsSelected = true;
                    break;
            }
        }

        #region Settings Handlers

        private void ApplySettings()
        {
            CompileTargetVirtual = CompileTarget;
            LuaSTGExecPathVirtual = LuaSTGExecPath;
            Properties.Settings.Default.Save();
        }

        private void GetSettings()
        {
            CompileTarget = CompileTargetVirtual;
            LuaSTGExecPath = LuaSTGExecPathVirtual;
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #region Commands and Clicks

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LuaSTGExecPath_Clicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "LuaSTG Executable (*.exe)|*.exe|LuaSTG dev Executable (*.dev.exe)|*.dev.exe"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                LuaSTGExecPath = openFileDialog.FileName;
            }
        }

        #endregion
    }
}
