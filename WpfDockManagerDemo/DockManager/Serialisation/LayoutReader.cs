using System.Collections.Generic;
using System.Xml;
using System.Windows.Controls;
using System.Windows;

namespace WpfDockManagerDemo.DockManager.Serialisation
{
    internal class LayoutReader
    {
        private static void SetWidthOrHeight(XmlElement xmlElement, FrameworkElement parentFrameworkElement, bool isParentHorizontal, int row, int column)
        {
            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("width") as XmlAttribute;

            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a width attribute");

            double width = System.Convert.ToDouble(xmlAttribute.Value);

            xmlAttribute = xmlElement.Attributes.GetNamedItem("height") as XmlAttribute;

            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a height attribute");

            double height = System.Convert.ToDouble(xmlAttribute.Value);

            Grid grid = parentFrameworkElement as Grid;
            if (grid != null)
            {
                if (isParentHorizontal)
                {
                    if (row < grid.RowDefinitions.Count)
                    {
                        grid.RowDefinitions[row].Height = new GridLength(height, GridUnitType.Star);
                    }
                }
                else
                {
                    if (column < grid.ColumnDefinitions.Count)
                    {
                        grid.ColumnDefinitions[column].Width = new GridLength(width, GridUnitType.Star);
                    }
                }
            }
        }

        private static void LoadToolGroup(Dictionary<string, UserControl> viewsMap, XmlElement xmlToolGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlToolGroupNode in xmlToolGroup.ChildNodes)
            {
                if (xmlToolGroupNode is XmlElement)
                {
                    if ((xmlToolGroupNode as XmlElement).Name == "Tool")
                    {
                        XmlElement xmlToolElement = xmlToolGroupNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlToolElement.Attributes.GetNamedItem("ContentId") as XmlAttribute;

                        System.Diagnostics.Trace.Assert(xmlAttribute != null, "Tool element does not have an ID attribute");

                        if (viewsMap.ContainsKey(xmlAttribute.Value))
                        {
                            iViewContainer.AddUserControl(viewsMap[xmlAttribute.Value]);
                            viewsMap.Remove(xmlAttribute.Value);
                        }
                    }
                }
            }
        }

        private static void LoadDocumentGroup(Dictionary<string, UserControl> viewsMap, XmlElement xmlDocumentGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlDocumentGroupNode in xmlDocumentGroup.ChildNodes)
            {
                if (xmlDocumentGroupNode is XmlElement)
                {
                    if ((xmlDocumentGroupNode as XmlElement).Name == "Document")
                    {
                        XmlElement xmlToolElement = xmlDocumentGroupNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlToolElement.Attributes.GetNamedItem("ContentId") as XmlAttribute;

                        // Warning warning ID needed?
                        System.Diagnostics.Trace.Assert(xmlAttribute != null, "Document element does not have an ID attribute");

                        if (viewsMap.ContainsKey(xmlAttribute.Value))
                        {
                            iViewContainer.AddUserControl(viewsMap[xmlAttribute.Value]);
                            viewsMap.Remove(xmlAttribute.Value);
                        }
                    }
                }
            }
        }

        public static void LoadNode(ILayoutFactory iLayoutFactory, Dictionary<string, UserControl> viewsMap, FrameworkElement rootFrameworkElement, FrameworkElement parentFrameworkElement, XmlNode parentXmlNode, bool isParentHorizontal)
        {
            int row = 0;
            int rowIncrement = isParentHorizontal ? 2 : 0;
            int column = 0;
            int columnIncrement = isParentHorizontal ? 0 : 2;

            foreach (var xmlChildNode in parentXmlNode.ChildNodes)
            {
                if (xmlChildNode is XmlElement)
                {
                    if ((xmlChildNode as XmlElement).Name == "SplitterPane")
                    {
                        XmlElement xmlSplitterPane = xmlChildNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlSplitterPane.Attributes.GetNamedItem("Orientation") as XmlAttribute;

                        System.Diagnostics.Trace.Assert(xmlAttribute != null, "SplitterPane element does not have an orientation attribute");

                        bool isChildHorizontal = xmlAttribute.Value == "Horizontal";

                        SplitterPane newGrid = new SplitterPane(isChildHorizontal);

                        if (parentFrameworkElement == rootFrameworkElement)
                        {
                            iLayoutFactory.SetRootPane(newGrid, out row, out column);
                        }
                        else
                        {
                            System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                            parentElement.AddChild(newGrid);
                            Grid.SetRow(newGrid, row);
                            Grid.SetColumn(newGrid, column);
                        }
                        SetWidthOrHeight(xmlSplitterPane, parentFrameworkElement, isParentHorizontal, row, column);

                        row += rowIncrement;
                        column += columnIncrement;

                        LoadNode(iLayoutFactory, viewsMap, rootFrameworkElement, newGrid, xmlSplitterPane, isChildHorizontal);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "DocumentPanel")
                    {
                        DocumentPanel documentPanel = new DocumentPanel();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(documentPanel);

                        XmlElement xmlDocumentPanel = xmlChildNode as XmlElement;

                        SetWidthOrHeight(xmlDocumentPanel, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadNode(iLayoutFactory, viewsMap, rootFrameworkElement, documentPanel, xmlDocumentPanel, true);

                        Grid.SetRow(documentPanel, row);
                        Grid.SetColumn(documentPanel, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "DocumentGroup")
                    {
                        DocumentPane documentPane = iLayoutFactory.CreateDocumentPane();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(documentPane);

                        XmlElement xmlDocumentGroup = xmlChildNode as XmlElement;

                        SetWidthOrHeight(xmlDocumentGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadDocumentGroup(viewsMap, xmlDocumentGroup, documentPane.IViewContainer);
                        Grid.SetRow(documentPane, row);
                        Grid.SetColumn(documentPane, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "ToolGroup")
                    {
                        ToolPane toolPane = iLayoutFactory.CreateToolPane();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(toolPane);

                        XmlElement xmlToolGroup = xmlChildNode as XmlElement;

                        SetWidthOrHeight(xmlToolGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadToolGroup(viewsMap, xmlToolGroup, toolPane.IViewContainer);
                        Grid.SetRow(toolPane, row);
                        Grid.SetColumn(toolPane, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "FloatingTool")
                    {
                        FloatingTool floatingTool = iLayoutFactory.CreateFloatingTool();

                        XmlElement xmlfloatingTool = xmlChildNode as XmlElement;
                        LoadToolGroup(viewsMap, xmlfloatingTool, floatingTool.IViewContainer);
                    }
                }

                if ((row > 2) || (column > 2))
                {
                    // we can only have two child elements (plus a splitter) in each grid
                    break;
                }
            }
        }
    }
}
