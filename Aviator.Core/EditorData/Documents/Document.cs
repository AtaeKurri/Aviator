using Aviator.Core.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using Aviator.Core.EditorData.Commands;
using Aviator.Core.EditorData.EditorTraces;
using System.Xml.Linq;

namespace Aviator.Core.EditorData.Documents
{
    public class Document : INotifyPropertyChanged
    {
        public ProjectConfiguration Configuration { get; set; }
        public CBSSolutionHandler SolutionHandler { get; set; }

        public string FilePath { get; set; } = "";

        public string FileName
        {
            get => RawFileName + (IsUnsaved ? " *" : "");
            set
            {
                RawFileName = value;
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public bool IsUnsaved
        {
            get
            {
                try { return CommandStack.Peek() != SavedCommand; }
                catch (InvalidOperationException) { return SavedCommand != null; }
            }
        }

        public string RawFileName { get; set; }
        public int FileHash { get; } = 0;

        public DocumentCollection Parent;

        public Stack<Command> CommandStack = [];
        public Stack<Command> UndoCommandStack = [];
        public Command? SavedCommand = null;

        public WorkTree TreeNodes { get; set; } = [];
        
        public Document(int hash, bool suppressMessage, string name, string path, bool _isSelected)
        {
            FileName = name;
            FilePath = path;
            IsSelected = _isSelected;
            FileHash = hash;
            Configuration = new(FilePath);
            SolutionHandler = new("name", "path"); // TODO: Générer un nom de solution (dans les paramètres de projet) et trouver le bon chemin de fichier
        }

        public virtual void OnOpen()
        {

        }

        public virtual void OnEditing(object sender, PropertyChangedEventArgs e)
        {

        }

        public virtual void OnClosing()
        {
            RevertUntilSaved();
            var toRemove = new List<EditorTrace>();
            foreach (EditorTrace trace in EditorTraceContainer.Traces)
            {
                if (trace.Source == this)
                {
                    toRemove.Add(trace);
                }
            }
            foreach (EditorTrace trace in toRemove)
                EditorTraceContainer.Traces.Remove(trace);
        }

        public bool Save(IAppSettings appSettings, bool saveAs = false)
        {
            string path = "";
            if (string.IsNullOrEmpty(FilePath) || saveAs)
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Aviator Project (*.avtr)|*.avtr",
                    InitialDirectory = appSettings.LastUsedPath,
                    FileName = saveAs ? "" : RawFileName
                };
                do
                {
                    if (saveFileDialog.ShowDialog() == false) return false;
                } while (string.IsNullOrEmpty(saveFileDialog.FileName));
                path = saveFileDialog.FileName;
                appSettings.LastUsedPath = Path.GetDirectoryName(path);
                FilePath = path;
                FileName = path[(path.LastIndexOf('\\') + 1)..];
            }
            else path = FilePath;
            
            try
            {
                using (StreamWriter sw = new(path))
                {
                    TreeNodes[0].SerializeToFile(sw, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }

        public static async Task<TreeNode> CreateNodesFromFile(string fileName, Document document)
        {
            TreeNode root = null;
            TreeNode parent = null;
            TreeNode tempNode = null;
            int previousLevel = -1;
            int i;
            int levelGraduation;
            string nodeToDeserialize;
            char[] temp;
            try
            {
                using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
                {
                    while (!sr.EndOfStream)
                    {
                        temp = (await sr.ReadLineAsync()).ToCharArray();
                        i = 0;
                        while (temp[i] != ',') i++;
                        nodeToDeserialize = new string(temp, i + 1, temp.Length - i - 1);
                        if (previousLevel != -1)
                        {
                            levelGraduation = Convert.ToInt32(new string(temp, 0, i)) - previousLevel;
                            if (levelGraduation <= 0)
                            {
                                for (int j = 0; j >= levelGraduation; j--)
                                {
                                    parent = parent.Parent;
                                }
                            }
                            tempNode = TreeSerializer.DeserializeTreeNode(nodeToDeserialize);
                            tempNode.ParentWorkspace = document;
                            parent.AddChild(tempNode);
                            parent = tempNode;
                            previousLevel += levelGraduation;
                        }
                        else
                        {
                            root = TreeSerializer.DeserializeTreeNode(nodeToDeserialize);
                            root.ParentWorkspace = document;
                            parent = root;
                            previousLevel = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return root;
        }

        #region Commands

        public void Undo()
        {
            CommandStack.Peek().Undo();
            UndoCommandStack.Push(CommandStack.Pop());
            RaisePropertyChanged("FileName");
        }

        public void Redo()
        {
            UndoCommandStack.Peek().Execute();
            CommandStack.Push(UndoCommandStack.Pop());
            RaisePropertyChanged("FileName");
        }

        public void PushSavedCommand()
        {
            try { SavedCommand = CommandStack.Peek(); }
            catch (InvalidOperationException) { SavedCommand = null; }
            RaisePropertyChanged("FileName");
        }

        public bool AddAndExecuteCommand(Command command)
        {
            if (command == null)
                return false;
            CommandStack.Push(command);
            CommandStack.Peek().Execute();
            UndoCommandStack = [];

            RaisePropertyChanged("FileName");
            return true;
        }

        #endregion

        private void RevertUntilSaved()
        {
            if (SavedCommand == null || CommandStack.Contains(SavedCommand))
            {
                while (CommandStack.Count != 0 && CommandStack.Peek() != SavedCommand)
                {
                    Undo();
                }
            }
            else
            {
                while (UndoCommandStack.Count != 0 && UndoCommandStack.Peek() != SavedCommand)
                {
                    Redo();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
