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

namespace Aviator.Core.EditorData.Documents
{
    public class Document : INotifyPropertyChanged
    {
        public string FilePath { get; set; } = "";

        public string FileName
        {
            get => RawFileName + (true ? " *" : "");
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

        public string RawFileName { get; set; }
        public int FileHash { get; } = 0;

        public DocumentCollection Parent;

        public static Document NewByType(string extension, int maxHash, string name, string path, bool supressMessage = false)
        {
            if (extension == ".avtrproj")
            {
                return new ProjectDocument(maxHash, supressMessage)
                {
                    FileName = name,
                    FilePath = path,
                    IsSelected = true
                };
            }
            else
            {
                return new PlainDocument(maxHash, supressMessage)
                {
                    FileName = name,
                    FilePath = path,
                    IsSelected = true
                };
            }
        }

        public WorkTree TreeNodes { get; set; } = [];
        
        public Document(int hash, bool suppressMessage)
        {
            FileHash = hash;
        }

        public bool Save(IAppSettings appSettings, bool saveAs = false)
        {
            string path = "";
            if (string.IsNullOrEmpty(FilePath) || saveAs)
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Aviator Project (*.avtrproj)|*.avtrproj|Aviator File (*.avtr)|*.avtr",
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

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
