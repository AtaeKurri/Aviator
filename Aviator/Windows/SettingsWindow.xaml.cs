using Aviator.Core.EditorData.Documents;
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

        private MainWindow parentWindow;

        public bool IsProjectLoaded => parentWindow.CurrentWorkspace != null;

        public Array CompileTargetArray
        {
            get => Enum.GetValues(typeof(ECompileTarget))
                .OfType<ECompileTarget>()
                .Where(m => (int)m == 0 || (int)m == 1)
                .ToArray();
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

        private string projectInternalName;
        public string ProjectInternalName
        {
            get => projectInternalName;
            set
            {
                projectInternalName = value;
                RaisePropertyChanged("ProjectInternalName");
            }
        }

        #endregion
        #region VirtualSettings

        public ECompileTarget CompileTargetVirtual
        {
            get => parentWindow.CurrentWorkspace.Configuration.CompileTarget;
            set => parentWindow.CurrentWorkspace.Configuration.CompileTarget = value;
        }
        public string LuaSTGExecPathVirtual
        {
            get => parentWindow.CurrentWorkspace.Configuration.LuaSTGExecutablePath;
            set => parentWindow.CurrentWorkspace.Configuration.LuaSTGExecutablePath = value;
        }
        public string ProjectInternalNameVirtual
        {
            get => parentWindow.CurrentWorkspace.Configuration.ProjectInternalName;
            set => parentWindow.CurrentWorkspace.Configuration.ProjectInternalName = value.Replace(" ", "");
        }

        #endregion

        public SettingsWindow(MainWindow mainWindow)
        {
            parentWindow = mainWindow;
            GetSettings();
            InitializeComponent();
        }

        public SettingsWindow(MainWindow mainWindow, int tabIndex)
            : this(mainWindow)
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
            if (IsProjectLoaded)
            {
                CompileTargetVirtual = CompileTarget;
                LuaSTGExecPathVirtual = LuaSTGExecPath;
                ProjectInternalNameVirtual = ProjectInternalName;

                parentWindow.CurrentWorkspace.Configuration.WriteConfiguration();
                parentWindow.ResetNodePickers();
            }

            Properties.Settings.Default.Save();
        }

        private void GetSettings()
        {
            if (IsProjectLoaded)
            {
                CompileTarget = CompileTargetVirtual;
                LuaSTGExecPath = LuaSTGExecPathVirtual;
                ProjectInternalName = ProjectInternalNameVirtual;
            }
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
            GetSettings();
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
