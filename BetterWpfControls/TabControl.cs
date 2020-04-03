using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BetterWpfControls.Components;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using BetterWpfControls.Converters;
using BetterWpfControls.Panels;

namespace BetterWpfControls
{
    [TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_HeaderPanel", Type = typeof(ScrollablePanel))]
    [TemplatePart(Name = "PART_QuickLinksHost", Type = typeof(MenuButton))]
    public class TabControl : System.Windows.Controls.TabControl
    {
        #region .ctors

        static TabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControl), new FrameworkPropertyMetadata(typeof(TabControl)));
        }

        public TabControl()
        {
            NavigateToItemCommand = new DelegateCommand<object>(NavigateToItem);
        }

        #endregion .ctors

        #region Properties

        #region ScrollToLeftContent

        public object ScrollToLeftContent
        {
            get { return (object)GetValue(ScrollToLeftContentProperty); }
            set { SetValue(ScrollToLeftContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToLeftContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToLeftContentProperty =
            DependencyProperty.Register("ScrollToLeftContent", typeof(object), typeof(TabControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion ScrollToLeftContent

        #region ScrollToRightContent

        public object ScrollToRightContent
        {
            get { return (object)GetValue(ScrollToRightContentProperty); }
            set { SetValue(ScrollToRightContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToRightContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToRightContentProperty =
            DependencyProperty.Register("ScrollToRightContent", typeof(object), typeof(TabControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion ScrollToRightContent

        #region NavigateToItemCommand

        public ICommand NavigateToItemCommand
        {
            get { return (ICommand)GetValue(NavigateToItemCommandProperty); }
            set { SetValue(NavigateToItemCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigateToItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigateToItemCommandProperty =
            DependencyProperty.Register("NavigateToItemCommand", typeof(ICommand), typeof(TabControl), new UIPropertyMetadata(null));

        #endregion NavigateToItemCommand

        #region ShowQuickLinksButton

        public bool ShowQuickLinksButton
        {
            get { return (bool)GetValue(ShowQuickLinksButtonProperty); }
            set { SetValue(ShowQuickLinksButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowQuickLinksButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowQuickLinksButtonProperty =
            DependencyProperty.Register("ShowQuickLinksButton", typeof(bool), typeof(TabControl), new UIPropertyMetadata(true));

        #endregion ShowQuickLinksButton

        #region SelectItemOnScroll

        public bool SelectItemOnScroll
        {
            get { return (bool)GetValue(SelectItemOnScrollProperty); }
            set { SetValue(SelectItemOnScrollProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectItemOnScroll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectItemOnScrollProperty =
            DependencyProperty.Register("SelectItemOnScroll", typeof(bool), typeof(TabControl), new UIPropertyMetadata(true));

        #endregion SelectItemOnScroll

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var tp = Template.FindName("PART_HeaderPanel", this) as ScrollablePanel;
            if (tp != null)
            {
                tp.SetBinding(ScrollablePanel.SelectedItemProperty, new Binding(SelectedItemProperty.Name) { Mode = BindingMode.TwoWay, Source = this });
                tp.SetBinding(ScrollablePanel.ScrollToLeftContentProperty, new Binding(ScrollToLeftContentProperty.Name) { Mode = BindingMode.TwoWay, Source = this });
                tp.SetBinding(ScrollablePanel.ScrollToRightContentProperty, new Binding(ScrollToRightContentProperty.Name) { Mode = BindingMode.TwoWay, Source = this });
                tp.SetBinding(ScrollablePanel.SelectItemOnScrollProperty, new Binding(SelectItemOnScrollProperty.Name) { Mode = BindingMode.TwoWay, Source = this });
            }
            var sb = Template.FindName("PART_QuickLinksHost", this) as MenuButton;
            if (sb != null)
            {
                if (tp != null)
                {
                    var b = new MultiBinding() { Converter = new BoolToVisibilityConverter() };
                    b.Bindings.Add(new Binding(ScrollablePanel.IsTruncatingItemsProperty.Name) { Source = tp, Mode = BindingMode.OneWay });
                    b.Bindings.Add(new Binding(TabControl.ShowQuickLinksButtonProperty.Name) { Source = this, Mode = BindingMode.OneWay });
                    sb.SetBinding(FrameworkElement.VisibilityProperty, b);
                }
                sb.SetBinding(MenuButton.ItemsSourceProperty, new Binding("Items") { Mode = BindingMode.OneWay, Source = this });
                sb.SetBinding(MenuButton.CommandProperty, new Binding(NavigateToItemCommandProperty.Name) { Mode = BindingMode.OneWay, Source = this });
                if (sb.ItemTemplate == null && sb.GetBindingExpression(MenuButton.ItemTemplateProperty) == null)
                {
                    var factory = new FrameworkElementFactory(typeof(ContentPresenter));
                    factory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    factory.SetBinding(ContentPresenter.ContentProperty, new Binding() { Converter = new WrapVisualConverter(), ConverterParameter = this });
                    factory.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding(ItemTemplateProperty.Name) { Source = this });
                    sb.ItemTemplate = new DataTemplate() { VisualTree = factory };
                }
            }
        }
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabItem;
        }

        private void NavigateToItem(object item)
        {
            SelectedItem = item;
        }

        #endregion Methods
    }
}