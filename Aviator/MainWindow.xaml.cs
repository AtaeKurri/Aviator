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
using Aviator.Nodes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Aviator.Core.EditorData.EditorTraces;
using Aviator.Commands;
using Aviator.Core.EditorData.Nodes.Attributes;
using Aviator.Nodes.EditorNodePicker;
using Aviator.Core.EditorData.Commands;

namespace Aviator
{
    /// <summary>
    /// Reprents the mode of insert of the nodes.
    /// </summary>
    public enum EInsertState
    {
        Before,
        Child,
        After
    }

    public partial class MainWindow : Window, IMainWindow, INotifyPropertyChanged
    {
        public DocumentCollection Documents { get; } = [];
        private AbstractNodePicker nodePickerBox = null;
        public AbstractNodePicker NodePickerBox
        {
            get => nodePickerBox;
            set
            {
                nodePickerBox = value;
                RaisePropertyChanged("NodePickerBox");
            }
        }

        private string debugLog = "";

        public ObservableCollection<EditorTrace> Traces { get => EditorTraceContainer.Traces; }

        private EInsertState insertState = EInsertState.Child;
        public EInsertState InsertState
        {
            get => insertState;
            set
            {
                insertState = value;
                RaiseInsertStateChanged();
            }
        }
        public bool IsBeforeState => InsertState == EInsertState.Before;
        public bool IsChildState => InsertState == EInsertState.Child;
        public bool IsAfterState => InsertState == EInsertState.After;

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
            //NodePickerBox = new CBSNodePicker(this);
            NodePickerBox = null;
            EditorCommands cmds = new();
            InitializeComponent();
            WorkspacesTab.ItemsSource = Documents;
            EditorTraces.ItemsSource = Traces;
        }

        #region Exec

        /// <summary>
        /// Creates a new document from an existing template.
        /// </summary>
        private void NewFile()
        {
            NewFileWindow newFileWindow = new();
            if (newFileWindow.ShowDialog() == true)
            {
                string pathToTemplate = Path.GetFullPath(newFileWindow.SelectedPath);
                CloneTemplate(newFileWindow.FileName, pathToTemplate);
            }
        }

        /// <summary>
        /// Open an existing file as a <see cref="Document"/>.
        /// </summary>
        /// <seealso cref="OpenDocumentFromPath(string, string)"/>
        /// <seealso cref="CloneTemplate(string, string)"/>
        private void OpenFile()
        {
            string lastUsedPath = (Application.Current as App).LastUsedPath;
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "Aviator Project (*.avtr)|*.avtr",
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

        /// <summary>
        /// Closes the provided <see cref="Document"/>.
        /// </summary>
        /// <param name="document">The document to close.</param>
        /// <returns>True if the file was closed; Otherwise, false.</returns>
        private bool CloseFile(Document document)
        {
            if (document.IsUnsaved)
            {
                switch (MessageBox.Show($"Do you want to save \"{document.RawFileName}\"?",
                    "Aviator Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        if (SaveDocument(document))
                        {
                            Documents.Remove(document);
                            document.OnClosing();
                            if (NodePropertiesData.ItemsSource is ObservableCollection<NodeAttribute> attr)
                            {
                                if (attr.Count > 0 && attr[0].ParentNode.ParentWorkspace == document)
                                {
                                    NodePropertiesData.ItemsSource = null;
                                }
                            }
                        }
                        return true;
                    case MessageBoxResult.No:
                        break;
                    default:
                        return false;
                }
            }
            Documents.Remove(document);
            document.OnClosing();
            if (NodePropertiesData.ItemsSource is ObservableCollection<NodeAttribute> na)
            {
                if (na.Count > 0 && na[0].ParentNode.ParentWorkspace == document)
                {
                    NodePropertiesData.ItemsSource = null;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if a file is already opened as a <see cref="Document"/> in the editor.
        /// </summary>
        /// <param name="path">The path to the file to check.</param>
        /// <returns>True if the file is already open; otherwise, false.</returns>
        private bool IsFileOpened(string path)
        {
            foreach (Document document in Documents)
            {
                if (!string.IsNullOrEmpty(document.FilePath) && Path.GetFullPath(document.FilePath) == Path.GetFullPath(path))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Open and puts a document in memory.
        /// </summary>
        /// <param name="name">The name of the document.</param>
        /// <param name="path">The path to the file.</param>
        public async void OpenDocumentFromPath(string name, string path)
        {
            try
            {
                Document document = new(Documents.MaxHash, false, name, path, true);
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

        /// <summary>
        /// Clone a template and create a document out of it.
        /// </summary>
        /// <param name="name">The name of the document.</param>
        /// <param name="path">The path to the file.</param>
        public async void CloneTemplate(string name, string path)
        {
            try
            {
                Document newDocument = new(Documents.MaxHash, false, name, path, true);
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

        /// <summary>
        /// Saves the active document.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        private bool SaveActiveDocument() => SaveDocument(CurrentWorkspace);

        /// <summary>
        /// Saves the active document as a new file.
        /// </summary>
        /// <returns>True if successful; otherwise, false.</returns>
        private bool SaveActiveDocumentAs() => SaveDocumentAs(CurrentWorkspace);

        /// <summary>
        /// Saves the specified document.
        /// </summary>
        /// <param name="document">The document to save.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        private bool SaveDocument(Document document)
        {
            NodePropertiesData.CommitEdit();
            return document.Save((Application.Current as App));
        }

        /// <summary>
        /// Saves the specified document as a new file.
        /// </summary>
        /// <param name="document">The document to save.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        private bool SaveDocumentAs(Document document)
        {
            NodePropertiesData.CommitEdit();
            return document.Save((Application.Current as App), true);
        }

        /// <summary>
        /// Undoes the last action.
        /// </summary>
        private void Undo()
        {
            CurrentWorkspace.Undo();
        }

        /// <summary>
        /// Redo the last undone action.
        /// </summary>
        private void Redo()
        {
            CurrentWorkspace.Redo();
        }

        #endregion
        #region TreeNode infos

        public void Insert(TreeNode node, bool doInvoke = true)
        {
            try
            {
                if (selectedNode == null) return;
                TreeNode oldSelection = selectedNode;
                Command cmd = null;
                switch (InsertState)
                {
                    case EInsertState.Before:
                        cmd = new InsertBeforeCommand(selectedNode, node);
                        break;
                    case EInsertState.Child:
                        cmd = new InsertChildCommand(selectedNode, node);
                        break;
                    case EInsertState.After:
                        cmd = new InsertAfterCommand(selectedNode, node);
                        break;
                }
                if (!selectedNode.ValidateChild(selectedNode, node))
                    return;
                if (CurrentWorkspace.AddAndExecuteCommand(cmd))
                {
                    RevealNode(node);
                    if (doInvoke)
                    {
                        node.CheckTrace(null, new PropertyChangedEventArgs(""));
                        CreateInvoke(node);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public void RevealNode(TreeNode node)
        {
            if (node == null)
                return;
            TreeNode temp = node.Parent;
            node.ParentWorkspace.IsSelected = true;
            node.ParentWorkspace.TreeNodes[0].ClearChildSelection();
            Stack<TreeNode> stack = [];
            while (temp != null)
            {
                stack.Push(temp);
                temp = temp.Parent;
            }
            while (stack.Count > 0)
                stack.Pop().IsExpanded = true;
            node.IsSelected = true;
        }

        public void CreateInvoke(TreeNode node)
        {
            
        }

        public void ResetNodePickers()
        {
            if (CurrentWorkspace != null)
            {
                switch (CurrentWorkspace.Configuration.CompileTarget)
                {
                    case ECompileTarget.LuaSTG:
                        NodePickerBox = new LuaNodePicker(this);
                        break;
                    case ECompileTarget.Chambersite:
                        NodePickerBox = new CBSNodePicker(this);
                        break;
                    default:
                        NodePickerBox = null;
                        break;
                }
                try { NodePickerTabControl.SelectedItem = NodePickerTabControl.Items[0]; }
                catch { }
            }
            else
            {
                NodePickerBox = null;
            }
        }

        #endregion
        #region Events

        private void ButtonAddNode_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button)?.Tag?.ToString();
            if (string.IsNullOrEmpty(tag))
                return;
            NodePickerBox.NodeFuncs[tag]();
        }

        private void RaiseInsertStateChanged()
        {
            RaisePropertyChanged("IsBeforeState");
            RaisePropertyChanged("IsChildState");
            RaisePropertyChanged("IsAfterState");
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Focus();
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is DataGridCell datacell)
            {
                DataGrid grid = (DataGrid)sender;
                if (!datacell.IsReadOnly) grid.BeginEdit(e);
            }
        }

        private void CloseDocument_Click(object sender, RoutedEventArgs e)
        {
            NodePropertiesData.CommitEdit();
            Button button = sender as Button;
            int buttonHash = Convert.ToInt32(button.Tag?.ToString());
            Document toRemove = Documents.First(doc => doc.FileHash == buttonHash);
            if (toRemove != null)
                CloseFile(toRemove);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NodePropertiesData.CommitEdit();
            while (Documents.Count > 0)
            {
                if (!CloseFile(Documents[0]))
                {
                    e.Cancel = true;
                    break;
                }
            }
            if (!e.Cancel) Application.Current.Shutdown();
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
                e.CanExecute = CurrentWorkspace.IsUnsaved;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentWorkspace != null;
        }

        private void RunProjectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CurrentWorkspace == null)
            {
                e.CanExecute = false;
            }
            else
            {
                if (CurrentWorkspace.Configuration.CompileTarget == ECompileTarget.LuaSTG)
                    e.CanExecute = CurrentWorkspace != null && !string.IsNullOrEmpty(CurrentWorkspace.Configuration.LuaSTGExecutablePath);
                else
                {
                    e.CanExecute = CurrentWorkspace != null && !string.IsNullOrEmpty(CurrentWorkspace.Configuration.ProjectInternalName);
                }
            }
        }

        private void UndoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CurrentWorkspace != null)
                e.CanExecute = CurrentWorkspace.CommandStack.Count > 0;
            else e.CanExecute = false;
        }

        private void RedoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (CurrentWorkspace != null)
                e.CanExecute = CurrentWorkspace.UndoCommandStack.Count > 0;
            else e.CanExecute = false;
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

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveActiveDocumentAs();
        }

        private void UndoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
            var a = NodePropertiesData.ItemsSource;
            NodePropertiesData.ItemsSource = null;
            NodePropertiesData.ItemsSource = a;
        }

        private void RedoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
            var a = NodePropertiesData.ItemsSource;
            NodePropertiesData.ItemsSource = null;
            NodePropertiesData.ItemsSource = a;
        }

        private void OpenSettingsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (int.TryParse(e.Parameter?.ToString(), out int tabIndex))
                new SettingsWindow(this, tabIndex).ShowDialog();
            else
                new SettingsWindow(this).ShowDialog();
        }

        private void RunProjectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WorkSpaceSelectedChanged(object sender, RoutedEventArgs e)
        {
            Workspace = sender as TreeView;
            SelectedNode = Workspace.SelectedItem as TreeNode;
            ResetNodePickers();
            if (selectedNode != null)
                NodePropertiesData.ItemsSource = selectedNode.attributes;
        }

        private void ToBeforeStateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertState = EInsertState.Before;
        }

        private void ToChildStateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertState = EInsertState.Child;
        }

        private void ToAfterStateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertState = EInsertState.After;
        }

        private void AdjustPropCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NodePropertiesData.CommitEdit();
            Button button = sender as Button;
            NodeAttribute attr = button.Tag as NodeAttribute;

            // Do InputWindow shit.
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}