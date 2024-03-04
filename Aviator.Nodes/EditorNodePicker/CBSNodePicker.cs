using Aviator.Core.EditorData.Nodes;
using Aviator.Nodes.EditorNodes.Chambersite;
using Aviator.Nodes.EditorNodes.Chambersite.Views;
using Aviator.Nodes.EditorNodes.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator.Nodes.EditorNodePicker
{
    public class CBSNodePicker(IMainWindow parentWindow) : AbstractNodePicker(parentWindow)
    {
        public override void Initialize()
        {
            NodePickerTab generalTab = new("General");
            NodePickerTab dataTab = new("Data");
            NodePickerTab taskingTab = new("Tasking");
            NodePickerTab enemiesTab = new("Enemies");
            NodePickerTab bulletsTab = new("Bullets");
            NodePickerTab objectsTab = new("Game Objects");
            NodePickerTab graphicsTab = new("Graphics");
            NodePickerTab renderingTab = new("Rendering");

            #region General

            generalTab.AddNode(new NodePickerItem("folder", "Folder.png", "Folder", new AddNode(AddNode_Folder)));
            generalTab.AddNode(new NodePickerItem("folderred", "FolderRed.png", "Red Folder", new AddNode(AddNode_FolderRed)));
            generalTab.AddNode(new NodePickerItem("foldergreen", "FolderGreen.png", "Green Folder", new AddNode(AddNode_FolderGreen)));
            generalTab.AddNode(new NodePickerItem("folderblue", "FolderBlue.png", "Blue Folder", new AddNode(AddNode_FolderBlue)));
            generalTab.AddNode(new NodePickerItem("code", "Code.png", "Code", new AddNode(AddNode_Code)));
            generalTab.AddNode(new NodePickerItem("comment", "Comment.png", "Comment", new AddNode(AddNode_Comment)));
            generalTab.AddNode(new NodePickerItem(true));
            generalTab.AddNode(new NodePickerItem("namespace", "NamespaceDef.png", "Define Namespace", new AddNode(AddNode_Namespace)));
            generalTab.AddNode(new NodePickerItem("view", "ViewDefinition.png", "Define View", new AddNode(AddNode_ViewDefinition)));

            #endregion
            #region Data



            #endregion
            #region Game Objects

            objectsTab.AddNode(new NodePickerItem("overridefunc", "Func.png", "Override Method", new AddNode(AddNode_OverrideFunc)));

            #endregion

            NodePickerTabs.Add(generalTab);
            NodePickerTabs.Add(dataTab);
            NodePickerTabs.Add(taskingTab);
            NodePickerTabs.Add(enemiesTab);
            NodePickerTabs.Add(bulletsTab);
            NodePickerTabs.Add(objectsTab);
            NodePickerTabs.Add(graphicsTab);
            NodePickerTabs.Add(renderingTab);
        }

        #region General

        private void AddNode_Folder()
        {
            ParentWindow.Insert(new Folder(ParentWindow.CurrentWorkspace));
        }

        private void AddNode_FolderRed()
        {
            ParentWindow.Insert(new FolderRed(ParentWindow.CurrentWorkspace));
        }

        private void AddNode_FolderGreen()
        {
            ParentWindow.Insert(new FolderGreen(ParentWindow.CurrentWorkspace));
        }

        private void AddNode_FolderBlue()
        {
            ParentWindow.Insert(new FolderBlue(ParentWindow.CurrentWorkspace));
        }

        private void AddNode_Code()
        {
            ParentWindow.Insert(new Code(ParentWindow.CurrentWorkspace));
        }

        private void AddNode_Comment()
        {
            ParentWindow.Insert(new Comment(ParentWindow.CurrentWorkspace));
        }

        private void AddNode_Namespace()
        {
            ParentWindow.Insert(new NamespaceDefinition(ParentWindow.CurrentWorkspace));
        }

        private void AddNode_ViewDefinition()
        {
            TreeNode ViewDef = new ViewDefinition(ParentWindow.CurrentWorkspace);
            ViewDef.AddChild(new OverrideFunc(ParentWindow.CurrentWorkspace, "Initialize"));
            ParentWindow.Insert(ViewDef);
        }

        #endregion
        #region Game Objects

        private void AddNode_OverrideFunc()
        {
            ParentWindow.Insert(new OverrideFunc(ParentWindow.CurrentWorkspace));
        }

        #endregion
    }
}
