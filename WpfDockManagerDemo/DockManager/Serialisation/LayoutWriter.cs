using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Windows;
using System.Windows.Controls;

namespace WpfDockManagerDemo.DockManager.Serialisation
{
    internal static class LayoutWriter
    {
        private static XmlElement SaveToolPaneGroupNode(XmlDocument xmlDocument, XmlNode xmlParentNode, ToolPaneGroup toolPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(toolPaneGroup != null);

            XmlElement xmlToolPaneGroup = xmlDocument.CreateElement("ToolPaneGroup");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Guid");
            xmlAttribute.Value = toolPaneGroup.Tag.ToString();
            xmlToolPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Row");
            xmlAttribute.Value = Grid.GetRow(toolPaneGroup).ToString();
            xmlToolPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Column");
            xmlAttribute.Value = Grid.GetColumn(toolPaneGroup).ToString();
            xmlToolPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Width");
            xmlAttribute.Value = toolPaneGroup.ActualWidth.ToString();
            xmlToolPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Height");
            xmlAttribute.Value = toolPaneGroup.ActualHeight.ToString();
            xmlToolPaneGroup.Attributes.Append(xmlAttribute);

            xmlParentNode.AppendChild(xmlToolPaneGroup);

            int count = toolPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Tool pane group has no tools");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = toolPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                SaveToolNode(xmlDocument, xmlToolPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlToolPaneGroup;
        }

        private static XmlElement SaveToolNode(XmlDocument xmlDocument, XmlNode xmlParentNode, IViewModel iViewModel, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(iViewModel != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Tool");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Title");
            xmlAttribute.Value = iViewModel.Title;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ContentId");
            xmlAttribute.Value = contentId;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlParentNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private static void SaveSplitterPaneNode(XmlDocument xmlDocument, Grid splitterPane, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(splitterPane != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlSplitterPane = xmlDocument.CreateElement("SplitterPane");
            xmlParentNode.AppendChild(xmlSplitterPane);

            GridSplitter gridSplitter = splitterPane.Children.OfType<GridSplitter>().Single();
            System.Diagnostics.Trace.Assert(gridSplitter != null);

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Guid");
            xmlAttribute.Value = splitterPane.Tag.ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            bool isHorizontal = Grid.GetRow(gridSplitter) == 1;
            xmlAttribute = xmlDocument.CreateAttribute("Orientation");
            xmlAttribute.Value = isHorizontal ? "Horizontal" : "Vertical";
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Row");
            xmlAttribute.Value = Grid.GetRow(splitterPane).ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Column");
            xmlAttribute.Value = Grid.GetColumn(splitterPane).ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Width");
            xmlAttribute.Value = splitterPane.ActualWidth.ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Height");
            xmlAttribute.Value = splitterPane.ActualHeight.ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            List<FrameworkElement> children = splitterPane.Children.OfType<FrameworkElement>().Where(n => !(n is GridSplitter)).OrderBy(n => Grid.GetRow(n) + Grid.GetColumn(n)).ToList();
            foreach (var childNode in children)
            {
                SaveNode(xmlDocument, childNode, xmlSplitterPane);
            }
        }

        private static XmlElement SaveDocumentPaneGroupNode(XmlDocument xmlDocument, XmlNode xmlParentNode, DocumentPaneGroup documentPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(documentPaneGroup != null);

            XmlElement xmlDocumentPaneGroup = xmlDocument.CreateElement("DocumentPaneGroup");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Guid");
            xmlAttribute.Value = documentPaneGroup.Tag.ToString();
            xmlDocumentPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Row");
            xmlAttribute.Value = Grid.GetRow(documentPaneGroup).ToString();
            xmlDocumentPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Column");
            xmlAttribute.Value = Grid.GetColumn(documentPaneGroup).ToString();
            xmlDocumentPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Width");
            xmlAttribute.Value = documentPaneGroup.ActualWidth.ToString();
            xmlDocumentPaneGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Height");
            xmlAttribute.Value = documentPaneGroup.ActualHeight.ToString();
            xmlDocumentPaneGroup.Attributes.Append(xmlAttribute);

            xmlParentNode.AppendChild(xmlDocumentPaneGroup);

            int count = documentPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Document pane group has no documents");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = documentPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                SaveDocumentNode(xmlDocument, xmlDocumentPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlDocumentPaneGroup;
        }

        private static XmlElement SaveDocumentNode(XmlDocument xmlDocument, XmlNode xmlParentNode, IViewModel iViewModel, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(iViewModel != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Document");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Title");
            xmlAttribute.Value = iViewModel.Title;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ContentId");
            xmlAttribute.Value = contentId;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlParentNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private static void SaveDocumentPanelNode(XmlDocument xmlDocument, DocumentPanel documentPanel, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(documentPanel != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlDocumentPanel = xmlDocument.CreateElement("DocumentPanel");
            xmlParentNode.AppendChild(xmlDocumentPanel);

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Row");
            xmlAttribute.Value = Grid.GetRow(documentPanel).ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Column");
            xmlAttribute.Value = Grid.GetColumn(documentPanel).ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Width");
            xmlAttribute.Value = documentPanel.ActualWidth.ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Height");
            xmlAttribute.Value = documentPanel.ActualHeight.ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            foreach (var childNode in documentPanel.Children)
            {
                SaveNode(xmlDocument, childNode, xmlDocumentPanel);
            }
        }

        private static void CreateLocationAndSizeAttributes(XmlDocument xmlDocument, XmlNode xmlElement, FloatingPane floatingPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlElement != null);

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Left");
            xmlAttribute.Value = floatingPane.Left.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Top");
            xmlAttribute.Value = floatingPane.Top.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Width");
            xmlAttribute.Value = floatingPane.ActualWidth.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("Height");
            xmlAttribute.Value = floatingPane.ActualHeight.ToString();
            xmlElement.Attributes.Append(xmlAttribute);
        }

        private static XmlElement SaveFloatingToolNode(XmlDocument xmlDocument, XmlNode xmlParentNode, FloatingToolPaneGroup floatingToolPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlFloatingToolPaneGroup = xmlDocument.CreateElement("FloatingToolPaneGroup");
            CreateLocationAndSizeAttributes(xmlDocument, xmlFloatingToolPaneGroup, floatingToolPaneGroup);

            xmlParentNode.AppendChild(xmlFloatingToolPaneGroup);

            int count = floatingToolPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Floating tool pane has no tools");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = floatingToolPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                SaveToolNode(xmlDocument, xmlFloatingToolPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlFloatingToolPaneGroup;
        }

        private static XmlElement SaveFloatingDocumentNode(XmlDocument xmlDocument, XmlNode xmlParentNode, FloatingDocumentPaneGroup floatingDocumentPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlFloatingDocumentPaneGroup = xmlDocument.CreateElement("FloatingDocumentPaneGroup");
            CreateLocationAndSizeAttributes(xmlDocument, xmlFloatingDocumentPaneGroup, floatingDocumentPaneGroup);

            xmlParentNode.AppendChild(xmlFloatingDocumentPaneGroup);

            int count = floatingDocumentPaneGroup.IViewContainer.GetUserControlCount();

            System.Diagnostics.Trace.Assert(count > 0, "Floating document pane has no tools");

            for (int index = 0; index < count; ++index)
            {
                UserControl userControl = floatingDocumentPaneGroup.IViewContainer.GetUserControl(index);
                if (userControl == null)
                {
                    break;
                }
                SaveDocumentNode(xmlDocument, xmlFloatingDocumentPaneGroup, userControl.DataContext as IViewModel, userControl.Name);
            }

            return xmlFloatingDocumentPaneGroup;
        }

        private static void SaveNode(XmlDocument xmlDocument, Object node, XmlNode xmlParentPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(node != null);
            System.Diagnostics.Trace.Assert(xmlParentPane != null);

            if (node is DocumentPanel)
            {
                SaveDocumentPanelNode(xmlDocument, node as DocumentPanel, xmlParentPane);
            }
            else if (node is DocumentPaneGroup)
            {
                SaveDocumentPaneGroupNode(xmlDocument, xmlParentPane, node as DocumentPaneGroup);
            }
            else if (node is ToolPaneGroup)
            {
                SaveToolPaneGroupNode(xmlDocument, xmlParentPane, node as ToolPaneGroup);
            }
            else if (node is Grid)
            {
                SaveSplitterPaneNode(xmlDocument, node as Grid, xmlParentPane);
            }
        }

        public static void SaveLayout(
            XmlDocument xmlDocument, 
            Grid rootGrid, 
            List<FloatingToolPaneGroup> 
            floatingToolPaneGroups, 
            List<FloatingDocumentPaneGroup> floatingDocumentPaneGroups,
            Dictionary<WindowLocation, List<UnpinnedToolData>> dictUnpinnedToolData)
        {
            XmlElement xmlLayoutManager = xmlDocument.CreateElement("LayoutManager");
            xmlDocument.AppendChild(xmlLayoutManager);

            SaveNode(xmlDocument, rootGrid, xmlLayoutManager);

            foreach (FloatingToolPaneGroup floatingTool in floatingToolPaneGroups)
            {
                SaveFloatingToolNode(xmlDocument, xmlLayoutManager, floatingTool);
            }

            foreach (FloatingDocumentPaneGroup floatingDocumentPaneGroup in floatingDocumentPaneGroups)
            {
                SaveFloatingDocumentNode(xmlDocument, xmlLayoutManager, floatingDocumentPaneGroup);
            }

            foreach (var keyValuePair in dictUnpinnedToolData)
            {
                string name = keyValuePair.Key.ToString();
                switch (keyValuePair.Key)
                {
                    case WindowLocation.LeftSide:
                        name = "LeftSide";
                        break;
                    case WindowLocation.TopSide:
                        name = "TopSide";
                        break;
                    case WindowLocation.RightSide:
                        name = "RightSide";
                        break;
                    case WindowLocation.BottomSide:
                        name = "BottomSide";
                        break;
                    default:
                        System.Diagnostics.Trace.Assert(false, "Invalid WindowLocation");
                        break;
                }

                XmlElement xmlElement = xmlDocument.CreateElement(name);
                xmlLayoutManager.AppendChild(xmlElement);

                foreach (var item in keyValuePair.Value)
                {
                    SaveUnpinnedToolDataNode(xmlDocument, xmlElement, item);
                }
            }
        }

        private static XmlElement SaveUnpinnedToolDataNode(XmlDocument xmlDocument, XmlNode xmlParentNode, UnpinnedToolData unpinnedToolData)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);
            System.Diagnostics.Trace.Assert(unpinnedToolData != null);

            XmlElement xmlUnpinnedToolData = xmlDocument.CreateElement("UnpinnedToolData");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Sibling");
            xmlAttribute.Value = unpinnedToolData.Sibling.ToString();
            xmlUnpinnedToolData.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("IsHorizontal");
            xmlAttribute.Value = unpinnedToolData.IsHorizontal.ToString();
            xmlUnpinnedToolData.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("IsFirst");
            xmlAttribute.Value = unpinnedToolData.IsFirst.ToString();
            xmlUnpinnedToolData.Attributes.Append(xmlAttribute);

            SaveToolPaneGroupNode(xmlDocument, xmlUnpinnedToolData, unpinnedToolData.ToolPaneGroup);

            xmlParentNode.AppendChild(xmlUnpinnedToolData);
            return xmlUnpinnedToolData;
        }
    }
}
