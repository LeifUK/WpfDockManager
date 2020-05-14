using System.Collections.Generic;
using System.Xml;
using System.Windows.Controls;
using System.Windows;
using System;

namespace WpfDockManagerDemo.DockManager.Serialisation
{
    internal class LayoutReader
    {
        private static void SetWidthOrHeight(XmlElement xmlElement, FrameworkElement parentFrameworkElement, bool isParentHorizontal, int row, int column)
        {
            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("Width") as XmlAttribute;

            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a Width attribute");

            double width = System.Convert.ToDouble(xmlAttribute.Value);

            xmlAttribute = xmlElement.Attributes.GetNamedItem("Height") as XmlAttribute;

            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a Height attribute");

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

        private static void SetLocationAndSize(XmlElement xmlElement, Window window)
        {
            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("Left") as XmlAttribute;
            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a Left attribute");
            double left = System.Convert.ToDouble(xmlAttribute.Value);

            xmlAttribute = xmlElement.Attributes.GetNamedItem("Top") as XmlAttribute;
            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a Top attribute");
            double top = System.Convert.ToDouble(xmlAttribute.Value);

            xmlAttribute = xmlElement.Attributes.GetNamedItem("Width") as XmlAttribute;
            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a Width attribute");
            double width = System.Convert.ToDouble(xmlAttribute.Value);

            xmlAttribute = xmlElement.Attributes.GetNamedItem("Height") as XmlAttribute;
            System.Diagnostics.Trace.Assert(xmlAttribute != null, xmlElement.Name + " element does not have a Height attribute");
            double height = System.Convert.ToDouble(xmlAttribute.Value);

            window.Left = left;
            window.Top = top;
            window.Width = width;
            window.Height = height;
        }

        private static void LoadToolPaneGroup(Dictionary<string, UserControl> viewsMap, XmlElement xmlToolPaneGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlChild in xmlToolPaneGroup.ChildNodes)
            {
                if (xmlChild is XmlElement)
                {
                    if ((xmlChild as XmlElement).Name == "Tool")
                    {
                        XmlElement xmlToolElement = xmlChild as XmlElement;

                        XmlAttribute xmlAttribute = xmlToolElement.Attributes.GetNamedItem("ContentId") as XmlAttribute;

                        System.Diagnostics.Trace.Assert(xmlAttribute != null, "Tool element does not have a ContentId attribute");

                        if (viewsMap.ContainsKey(xmlAttribute.Value))
                        {
                            iViewContainer.AddUserControl(viewsMap[xmlAttribute.Value]);
                            viewsMap.Remove(xmlAttribute.Value);
                        }
                    }
                }
            }
        }

        private static void LoadDocumentPaneGroup(Dictionary<string, UserControl> viewsMap, XmlElement xmlDocumentPaneGroup, IViewContainer iViewContainer)
        {
            foreach (var xmlChild in xmlDocumentPaneGroup.ChildNodes)
            {
                if (xmlChild is XmlElement)
                {
                    if ((xmlChild as XmlElement).Name == "Document")
                    {
                        XmlElement xmlToolElement = xmlChild as XmlElement;

                        XmlAttribute xmlAttribute = xmlToolElement.Attributes.GetNamedItem("ContentId") as XmlAttribute;

                        if (viewsMap.ContainsKey(xmlAttribute.Value))
                        {
                            iViewContainer.AddUserControl(viewsMap[xmlAttribute.Value]);
                            viewsMap.Remove(xmlAttribute.Value);
                        }
                    }
                }
            }
        }

        private static Guid GetGuid(XmlElement xmlElement)
        {
            XmlAttribute xmlAttribute = xmlElement.Attributes.GetNamedItem("Guid") as XmlAttribute;
            return new Guid(xmlAttribute.Value);
        }

        public static void LoadNode(ILayoutFactory iLayoutFactory, Dictionary<string, UserControl> viewsMap, FrameworkElement rootFrameworkElement, FrameworkElement parentFrameworkElement, XmlNode xmlParentElement, bool isParentHorizontal)
        {
            int row = 0;
            int rowIncrement = isParentHorizontal ? 2 : 0;
            int column = 0;
            int columnIncrement = isParentHorizontal ? 0 : 2;

            foreach (var xmlChildNode in xmlParentElement.ChildNodes)
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
                        newGrid.Tag = GetGuid(xmlSplitterPane);

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
                    else if ((xmlChildNode as XmlElement).Name == "DocumentPaneGroup")
                    {
                        DocumentPaneGroup documentPaneGroup = iLayoutFactory.CreateDocumentPaneGroup();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(documentPaneGroup);

                        XmlElement xmlDocumentGroup = xmlChildNode as XmlElement;

                        documentPaneGroup.Tag = GetGuid(xmlDocumentGroup); ;
                        SetWidthOrHeight(xmlDocumentGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadDocumentPaneGroup(viewsMap, xmlDocumentGroup, documentPaneGroup.IViewContainer);
                        Grid.SetRow(documentPaneGroup, row);
                        Grid.SetColumn(documentPaneGroup, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "ToolPaneGroup")
                    {
                        ToolPaneGroup toolPaneGroup = iLayoutFactory.CreateToolPaneGroup();

                        System.Windows.Markup.IAddChild parentElement = (System.Windows.Markup.IAddChild)parentFrameworkElement;
                        parentElement.AddChild(toolPaneGroup);

                        XmlElement xmlToolPaneGroup = xmlChildNode as XmlElement;

                        toolPaneGroup.Tag = GetGuid(xmlToolPaneGroup); ;
                        SetWidthOrHeight(xmlToolPaneGroup, parentFrameworkElement, isParentHorizontal, row, column);

                        LoadToolPaneGroup(viewsMap, xmlToolPaneGroup, toolPaneGroup.IViewContainer);
                        Grid.SetRow(toolPaneGroup, row);
                        Grid.SetColumn(toolPaneGroup, column);
                        row += rowIncrement;
                        column += columnIncrement;
                    }
                    else if ((xmlChildNode as XmlElement).Name == "FloatingToolPaneGroup")
                    {
                        FloatingToolPaneGroup floatingToolPaneGroup = iLayoutFactory.CreateFloatingToolPaneGroup();
                        XmlElement xmlfloatingTool = xmlChildNode as XmlElement;
                        floatingToolPaneGroup.Tag = GetGuid(xmlfloatingTool);
                        SetLocationAndSize(xmlfloatingTool, floatingToolPaneGroup);
                        LoadToolPaneGroup(viewsMap, xmlfloatingTool, floatingToolPaneGroup.IViewContainer);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "FloatingDocumentPaneGroup")
                    {
                        FloatingDocumentPaneGroup floatingDocumentPaneGroup = iLayoutFactory.CreateFloatingDocumentPaneGroup();
                        XmlElement xmlfloatingDocument = xmlChildNode as XmlElement;
                        floatingDocumentPaneGroup.Tag = GetGuid(xmlfloatingDocument);
                        SetLocationAndSize(xmlfloatingDocument, floatingDocumentPaneGroup);
                        LoadDocumentPaneGroup(viewsMap, xmlfloatingDocument, floatingDocumentPaneGroup.IViewContainer);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "LeftSide")
                    {
                        XmlElement xmlLeftSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolPaneGroups(iLayoutFactory, viewsMap, WindowLocation.LeftSide, xmlLeftSide);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "TopSide")
                    {
                        XmlElement xmlTopSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolPaneGroups(iLayoutFactory, viewsMap, WindowLocation.TopSide, xmlTopSide);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "RightSide")
                    {
                        XmlElement xmlRightSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolPaneGroups(iLayoutFactory, viewsMap, WindowLocation.RightSide, xmlRightSide);
                    }
                    else if ((xmlChildNode as XmlElement).Name == "BottomSide")
                    {
                        XmlElement xmlBottomSide = xmlChildNode as XmlElement;
                        LoadUnPinnedToolPaneGroups(iLayoutFactory, viewsMap, WindowLocation.TopSide, xmlBottomSide);
                    }
                }

                if (parentFrameworkElement != rootFrameworkElement)
                {
                    if ((row > 2) || (column > 2))
                    {
                        // we can only have two child elements (plus a splitter) in each grid
                        break;
                    }
                }
            }
        }

        private static void LoadUnPinnedToolPaneGroups(ILayoutFactory iLayoutFactory, Dictionary<string, UserControl> viewsMap, WindowLocation windowLocation, XmlElement xmlParentElement)
        {
            foreach (var xmlChildNode in xmlParentElement.ChildNodes)
            {
                if (xmlChildNode is XmlElement)
                {
                    if ((xmlChildNode as XmlElement).Name == "UnpinnedToolData")
                    {
                        XmlElement xmlUnpinnedToolData = xmlChildNode as XmlElement;

                        XmlAttribute xmlAttribute = xmlUnpinnedToolData.Attributes.GetNamedItem("Sibling") as XmlAttribute;
                        string guid = xmlAttribute.Value;
                        xmlAttribute = xmlUnpinnedToolData.Attributes.GetNamedItem("IsHorizontal") as XmlAttribute;
                        bool.TryParse(xmlAttribute.Value, out bool isHorizontal);
                        xmlAttribute = xmlUnpinnedToolData.Attributes.GetNamedItem("IsFirst") as XmlAttribute;
                        bool.TryParse(xmlAttribute.Value, out bool isFirst);

                        foreach (var xmlUnpinnedToolDataChildNode in xmlUnpinnedToolData.ChildNodes)
                        {
                            if (xmlUnpinnedToolDataChildNode is XmlElement)
                            {
                                if ((xmlUnpinnedToolDataChildNode as XmlElement).Name == "ToolPaneGroup")
                                {
                                    ToolPaneGroup toolPaneGroup = iLayoutFactory.CreateToolPaneGroup();
                                    XmlElement xmlToolPaneGroup = xmlUnpinnedToolDataChildNode as XmlElement;
                                    LoadToolPaneGroup(viewsMap, xmlToolPaneGroup, toolPaneGroup.IViewContainer);
                                    iLayoutFactory.CreateUnpinnedToolPaneGroup(windowLocation, toolPaneGroup, guid, isHorizontal, isFirst);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

