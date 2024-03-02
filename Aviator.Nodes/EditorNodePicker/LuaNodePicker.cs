﻿using Aviator.Nodes.EditorNodes.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aviator.Nodes.EditorNodePicker.AbstractNodePicker;

namespace Aviator.Nodes.EditorNodePicker
{
    public class LuaNodePicker(IMainWindow parentWindow) : AbstractNodePicker(parentWindow)
    {
        public override void Initialize()
        {
            NodePickerTab generalTab = new("General");
            NodePickerTab dataTab = new("Data");
            NodePickerTab taskingTab = new("Tasking");
            NodePickerTab enemiesTab = new("Enemies");
            NodePickerTab bulletsTab = new("Bullets");
            NodePickerTab objectsTab = new("Objects");
            NodePickerTab graphicsTab = new("Graphics");
            NodePickerTab renderingTab = new("Rendering");

            #region General

            generalTab.AddNode(new NodePickerItem("folder", "folder", "Base Folder", new AddNode(AddNode_Folder)));

            #endregion
            #region Data



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

        #region AddNodes Methods

        private void AddNode_Folder()
        {
            ParentWindow.Insert(new Folder(ParentWindow.CurrentWorkspace));
        }

        #endregion
    }
}