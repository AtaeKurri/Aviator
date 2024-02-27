using System;
using System.IO;
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

namespace Aviator.Windows
{
    /// <summary>
    /// Logique d'interaction pour NewFileWindow.xaml
    /// </summary>
    public partial class NewFileWindow : Window, INotifyPropertyChanged
    {
        private string fileName = "Untitled";
        private string author = (Application.Current as App).AuthorName; // TODO: A remplacer par le paramètre de base de l'application.
        private int modVersion = 4096;

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                RaisePropertyChanged("FileName");
            }
        }

        public string Author
        {
            get => author;
            set
            {
                author = value;
                RaisePropertyChanged("Author");
            }
        }

        public int ModVersion
        {
            get => modVersion;
            set
            {
                modVersion = value;
                RaisePropertyChanged("ModVersion");
            }
        }


        public string SelectedPath { get; set; }

        class FileTemplate
        {
            public string Name { get; set; } = "";
            public string FilePath { get; set; } = "";
            public string Icon { get; set; } = "..\\Images\\Icon.png";
        }

        List<FileTemplate> Templates { get; set; }

        public NewFileWindow()
        {
            string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates\\"));
            DirectoryInfo directory = new DirectoryInfo(path);
            List<FileInfo> fileInfos = new List<FileInfo>(directory.GetFiles("*.avtr"));
            fileInfos.AddRange(directory.GetFiles("*.avtrproj"));
            Templates = new List<FileTemplate>(
                from FileInfo fi in fileInfos
                select new FileTemplate
                {
                    Name = Path.GetFileNameWithoutExtension(fi.Name),
                    FilePath = fi.FullName
                }
            );
            InitializeComponent();
            TemplateList.ItemsSource = Templates;
            try { TemplateList.SelectedIndex = 0; } catch { }
        }

        private void TemplateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FileTemplate template = TemplateList.SelectedItem as FileTemplate;
            try
            {
                string filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Templates", template.Name + ".txt"));
                using (StreamReader  sr = new StreamReader(filePath))
                {
                    TextDescription.Text = sr.ReadLine();
                }
            }
            catch { }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            SelectedPath = (TemplateList.SelectedItem as FileTemplate)?.FilePath;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
