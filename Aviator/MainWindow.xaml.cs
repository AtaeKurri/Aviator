using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Microsoft.Win32;
using Aviator.Core.EditorData.Documents;
using Aviator.Windows;
using Newtonsoft.Json;
using Aviator.Core.EditorData.Nodes;
using Aviator.Core;
using Aviator.Nodes.NodeToolbox;
using Aviator.Nodes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Aviator.Core.EditorData.EditorTraces;
using Aviator.Commands;
using Aviator.Core.EditorData.Nodes.Attributes;

namespace Aviator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow, INotifyPropertyChanged
    {
        public DocumentCollection Documents { get; } = [];
        public NodePicker NodePickerBox { get; set; }

        private string debugLog = "";

        public ObservableCollection<EditorTrace> Traces { get => EditorTraceContainer.Traces; }

        public string DebugLog
        {
            get => debugLog;
            set
            {
                debugLog = value;
                RaisePropertyChanged("DebugLog");
            }
        }

        private TreeNode selectedNode = null;
        public TreeNode SelectedNode
        {
            get => selectedNode;
            set
            {
                selectedNode = value;
                RaisePropertyChanged("SelectedNode");
            }
        }

        public Document CurrentWorkspace
        {
            get => WorkspacesTab?.SelectedItem as Document;
        }

        public TreeView Workspace;

        public MainWindow()
        {
            NodePickerBox = new NodePicker(this);
            EditorCommands cmds = new();
            InitializeComponent();
            WorkspacesTab.ItemsSource = Documents;
            EditorTraces.ItemsSource = Traces;
        }

        #region Exec

        private void NewFile()
        {
            NewFileWindow newFileWindow = new();
            if (newFileWindow.ShowDialog() == true)
            {
                string pathToTemplate = Path.GetFullPath(newFileWindow.SelectedPath);
                CloneTemplate(newFileWindow.FileName, pathToTemplate);
            }
        }

        private void OpenFile()
        {
            string lastUsedPath = (Application.Current as App).LastUsedPath;
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "Aviator Project (*.avtrproj)|*.avtrproj|Aviator File (*.avtr)|*.avtr",
                InitialDirectory = string.IsNullOrEmpty(lastUsedPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : lastUsedPath
            };
            if (openFileDialog.ShowDialog() != true) return;
            for (int i = 0; i < openFileDialog.FileNames.Length; i++)
            {
                if (!IsFileOpened(openFileDialog.FileNames[i]))
                    OpenDocumentFromPath(openFileDialog.SafeFileNames[i], openFileDialog.FileNames[i]);
                (Application.Current as App).LastUsedPath = Path.GetDirectoryName(openFileDialog.FileNames[i]);
            }
        }

        private bool IsFileOpened(string path)
        {
            foreach (Document document in Documents)
            {
                if (!string.IsNullOrEmpty(document.FilePath) && Path.GetFullPath(document.FilePath) == Path.GetFullPath(path))
                    return true;
            }
            return false;
        }

        public async void OpenDocumentFromPath(string name, string path)
        {
            try
            {
                Document document = Document.NewByType(Path.GetExtension(path), Documents.MaxHash, name, path);
                Documents.Add(document);
                TreeNode root = await Document.CreateNodesFromFile(path, document);
                document.TreeNodes.Add(root);
            }
            catch (JsonException err)
            {
                MessageBox.Show($"Failed to open file. Check if it's in the same version as the editor.\n{err}",
                    "Aviator Editor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CloneTemplate(string name, string path)
        {
            try
            {
                Document newDocument = Document.NewByType(Path.GetExtension(path), Documents.MaxHash, name, path);
                newDocument.FilePath = "";
                Documents.Add(newDocument);
                TreeNode tree = await Document.CreateNodesFromFile(path, newDocument);
                newDocument.TreeNodes.Add(tree);
            }
            catch (JsonException err)
            {
                MessageBox.Show($"Failed to open file. Check if it's in the same version as the editor.\n{err}",
                    "Aviator Editor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool SaveActiveDocument() => SaveDocument(CurrentWorkspace);

        private static bool SaveDocument(Document document)
        {
            // TODO: Commit edits here
            return document.Save((Application.Current as App));
        }

        #endregion
        #region Events

        private void ButtonAddNode_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
        #region CommandBindings

        #region CanExecute
        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CurrentWorkspace != null)
            {
                e.CanExecute = true; // TODO: Check for if CurrentWorkspace is unsaved, true if unsaved.
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void RunProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((ECompileTarget)(Application.Current as App).CompileTarget == ECompileTarget.Lua)
                e.CanExecute = CurrentWorkspace != null && !string.IsNullOrEmpty((Application.Current as App).LSTGExecutablePath);
        }

        #endregion

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NewFile();
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFile();
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveActiveDocument();
        }

        private void OpenSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsWindow settings = new();
            settings.ShowDialog();
        }

        private void RunProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Insert(TreeNode node, bool doInvoke = true)
        {
            throw new NotImplementedException();
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}