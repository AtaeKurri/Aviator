﻿using Aviator.Core;
using Aviator.Properties;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Aviator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IAppSettings
    {
        public string AuthorName
        {
            get => Settings.Default.AuthorName;
            set { Settings.Default.AuthorName = value; }
        }

        public string LastUsedPath
        {
            get => Settings.Default.LastUsedPath;
            set { Settings.Default.LastUsedPath = value; }
        }

        public int CompileTarget
        {
            get => Settings.Default.CompileTarget;
            set { Settings.Default.CompileTarget = value; }
        }

        public string LSTGExecutablePath
        {
            get => Settings.Default.LSTGExecutablePath;
            set { Settings.Default.LSTGExecutablePath = value; }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
            Current.Shutdown();
        }
    }
}
