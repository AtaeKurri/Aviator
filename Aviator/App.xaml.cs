using Aviator.Core;
using Aviator.Core.EditorData.Documents;
using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.InputWindows;
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
            set => Settings.Default.AuthorName = value;
        }

        public string LastUsedPath
        {
            get => Settings.Default.LastUsedPath;
            set => Settings.Default.LastUsedPath = value;
        }

        public string CurrentTheme
        {
            get => Settings.Default.CurrentTheme;
            set => Settings.Default.CurrentTheme = value;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            InputWindowSelector.Register();
            InputWindowSelector.AfterRegister();
            SetTheme(CurrentTheme);
        }

        public void SetTheme(string themeName)
        {
            Resources.MergedDictionaries[0].Source = new($"Themes/ColourDictionaries/{themeName}.xaml", UriKind.Relative);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
            Current.Shutdown();
        }
    }
}
