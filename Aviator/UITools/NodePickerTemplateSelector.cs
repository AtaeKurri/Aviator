using Aviator.Nodes.EditorNodePicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Aviator.UITools
{
    internal class NodePickerContentTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            NodePickerItem node = (NodePickerItem)item;
            DataTemplate dataTemplate = null;
            if (container is FrameworkElement frameworkElement)
            {
                if (node.IsSeparator)
                    dataTemplate = frameworkElement.FindResource("NodePickerSeparator") as DataTemplate;
                else
                    dataTemplate = frameworkElement.FindResource("NodePickerButton") as DataTemplate;
            }
            return dataTemplate;
        }
    }

    internal class NodePickerTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dataTemplate = null;
            if (container is FrameworkElement frameworkElement)
            {
                if (item is NodePickerTab)
                    dataTemplate = frameworkElement.FindResource("NodePickerContent") as DataTemplate;
            }
            return dataTemplate;
        }
    }
}
