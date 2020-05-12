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
        private static XmlElement SaveToolGroupNode(XmlDocument xmlDocument, XmlNode xmlNode, ToolPaneGroup toolPane)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(toolPane != null);

            XmlElement xmlToolGroup = xmlDocument.CreateElement("ToolGroup");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(toolPane).ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(toolPane).ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = toolPane.ActualWidth.ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = toolPane.ActualHeight.ToString();
            xmlToolGroup.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlToolGroup);
            return xmlToolGroup;
        }

        private static XmlElement SaveToolNode(XmlDocument xmlDocument, XmlNode xmlNode, IViewModel iViewModel, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(iViewModel != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Tool");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Title");
            xmlAttribute.Value = iViewModel.Title;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ID");
            xmlAttribute.Value = iViewModel.ID.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ContentId");
            xmlAttribute.Value = contentId;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlElement);
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

            bool isHorizontal = Grid.GetRow(gridSplitter) == 1;
            var xmlAttribute = xmlDocument.CreateAttribute("Orientation");
            xmlAttribute.Value = isHorizontal ? "Horizontal" : "Vertical";
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(splitterPane).ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(splitterPane).ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = splitterPane.ActualWidth.ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = splitterPane.ActualHeight.ToString();
            xmlSplitterPane.Attributes.Append(xmlAttribute);

            List<FrameworkElement> children = splitterPane.Children.OfType<FrameworkElement>().Where(n => !(n is GridSplitter)).OrderBy(n => Grid.GetRow(n) + Grid.GetColumn(n)).ToList();
            foreach (var childNode in children)
            {
                SaveNode(xmlDocument, childNode, xmlSplitterPane);
            }
        }

        private static XmlElement SaveDocumentGroupNode(XmlDocument xmlDocument, XmlNode xmlNode, DocumentPaneGroup documentPaneGroup)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(documentPaneGroup != null);

            XmlElement xmlDocumentGroup = xmlDocument.CreateElement("DocumentGroup");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(documentPaneGroup).ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(documentPaneGroup).ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = documentPaneGroup.ActualWidth.ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = documentPaneGroup.ActualHeight.ToString();
            xmlDocumentGroup.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlDocumentGroup);
            return xmlDocumentGroup;
        }

        private static XmlElement SaveDocumentNode(XmlDocument xmlDocument, XmlNode xmlNode, IViewModel iViewModel, string contentId)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlNode != null);
            System.Diagnostics.Trace.Assert(iViewModel != null);

            XmlElement xmlElement = xmlDocument.CreateElement("Document");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Title");
            xmlAttribute.Value = iViewModel.Title;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ID");
            xmlAttribute.Value = iViewModel.ID.ToString();
            xmlElement.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("ContentId");
            xmlAttribute.Value = contentId;
            xmlElement.Attributes.Append(xmlAttribute);

            xmlNode.AppendChild(xmlElement);
            return xmlElement;
        }

        private static void SaveDocumentPanelNode(XmlDocument xmlDocument, DocumentPanel documentPanel, XmlNode xmlParentNode)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(documentPanel != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlDocumentPanel = xmlDocument.CreateElement("DocumentPanel");
            xmlParentNode.AppendChild(xmlDocumentPanel);

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("row");
            xmlAttribute.Value = Grid.GetRow(documentPanel).ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("column");
            xmlAttribute.Value = Grid.GetColumn(documentPanel).ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = documentPanel.ActualWidth.ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = documentPanel.ActualHeight.ToString();
            xmlDocumentPanel.Attributes.Append(xmlAttribute);

            foreach (var childNode in documentPanel.Children)
            {
                SaveNode(xmlDocument, childNode, xmlDocumentPanel);
            }
        }

        private static XmlElement SaveFloatingToolNode(XmlDocument xmlDocument, XmlNode xmlParentNode, FloatingTool floatingTool)
        {
            System.Diagnostics.Trace.Assert(xmlDocument != null);
            System.Diagnostics.Trace.Assert(xmlParentNode != null);

            XmlElement xmlFloatingTool = xmlDocument.CreateElement("FloatingTool");

            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("width");
            xmlAttribute.Value = floatingTool.ActualWidth.ToString();
            xmlFloatingTool.Attributes.Append(xmlAttribute);

            xmlAttribute = xmlDocument.CreateAttribute("height");
            xmlAttribute.Value = floatingTool.ActualHeight.ToString();
            xmlFloatingTool.Attributes.Append(xmlAttribute);

            xmlParentNode.AppendChild(xmlFloatingTool);
            return xmlFloatingTool;
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
                DocumentPaneGroup documentPaneGroup = node as DocumentPaneGroup;
                int count = documentPaneGroup.IViewContainer.GetUserControlCount();

                System.Diagnostics.Trace.Assert(count > 0, "Document pane has no documents");

                XmlNode xmlNodeParent = SaveDocumentGroupNode(xmlDocument, xmlParentPane, documentPaneGroup);

                for (int index = 0; index < count; ++index)
                {
                    UserControl userControl = documentPaneGroup.IViewContainer.GetUserControl(index);
                    if (userControl == null)
                    {
                        break;
                    }
                    SaveDocumentNode(xmlDocument, xmlNodeParent, userControl.DataContext as IViewModel, userControl.Name);
                }
            }
            else if (node is ToolPaneGroup)
            {
                ToolPaneGroup toolPane = node as ToolPaneGroup;
                int count = toolPane.IViewContainer.GetUserControlCount();

                System.Diagnostics.Trace.Assert(count > 0, "Too pane has no tools");

                XmlNode xmlNodeParent = SaveToolGroupNode(xmlDocument, xmlParentPane, toolPane);

                for (int index = 0; index < count; ++index)
                {
                    UserControl userControl = toolPane.IViewContainer.GetUserControl(index);
                    if (userControl == null)
                    {
                        break;
                    }
                    SaveToolNode(xmlDocument, xmlNodeParent, userControl.DataContext as IViewModel, userControl.Name);
                }
            }
            else if (node is Grid)
            {
                SaveSplitterPaneNode(xmlDocument, node as Grid, xmlParentPane);
            }
        }

        public static void SaveLayout(XmlDocument xmlDocument, Grid rootGrid, List<FloatingTool> floatingTools)
        {
            XmlElement xmlLayoutManager = xmlDocument.CreateElement("LayoutManager");
            xmlDocument.AppendChild(xmlLayoutManager);

            SaveNode(xmlDocument, rootGrid, xmlLayoutManager);

            foreach (FloatingTool floatingTool in floatingTools)
            {
                int count = floatingTool.IViewContainer.GetUserControlCount();
                
                System.Diagnostics.Trace.Assert(count > 0, "Floating tool pane has no tools");

                XmlElement xmlNodeParent = SaveFloatingToolNode(xmlDocument, xmlLayoutManager, floatingTool);

                for (int index = 0; index < count; ++index)
                {
                    UserControl userControl = floatingTool.IViewContainer.GetUserControl(index);
                    if (userControl == null)
                    {
                        break;
                    }
                    SaveToolNode(xmlDocument, xmlNodeParent, userControl.DataContext as IViewModel, userControl.Name);
                }
            }
        }
    }
}
