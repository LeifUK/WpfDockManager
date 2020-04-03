using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BetterWpfControls
{
    public class MenuButton : ComboBox, ICommandSource
    {
        #region .ctors

        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton), new FrameworkPropertyMetadata(typeof(MenuButton)));
        }

        public MenuButton()
        {
        }

        #endregion .ctors

        #region Properties

        #region Content

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(MenuButton), new UIPropertyMetadata(null, (s, e) =>
            {
            }));

        #endregion Content

        #region ContentTemplate

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(MenuButton), new UIPropertyMetadata(null));

        #endregion ContentTemplate

        #region ContentBorderBrush

        public Brush ContentBorderBrush
        {
            get { return (Brush)GetValue(ContentBorderBrushProperty); }
            set { SetValue(ContentBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentBorderBrushProperty =
            DependencyProperty.Register("ContentBorderBrush", typeof(Brush), typeof(MenuButton), new UIPropertyMetadata(null));

        #endregion ContentBorderBrush

        #region ContentBorderThickness

        public Thickness ContentBorderThickness
        {
            get { return (Thickness)GetValue(ContentBorderThicknessProperty); }
            set { SetValue(ContentBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentBorderThicknessProperty =
            DependencyProperty.Register("ContentBorderThickness", typeof(Thickness), typeof(MenuButton), new UIPropertyMetadata(null));

        #endregion ContentBorderThickness

        #region Command

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuButton), new UIPropertyMetadata(null));

        #endregion Command

        #region CommandParameter

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MenuButton), new UIPropertyMetadata(null));

        #endregion CommandParameter

        #region CommandTarget

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(MenuButton), new UIPropertyMetadata(null));

        #endregion CommandTarget

        #region AlwaysShowToggleButton

        public bool AlwaysShowToggleButton
        {
            get { return (bool)GetValue(AlwaysShowToggleButtonProperty); }
            set { SetValue(AlwaysShowToggleButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AlwaysShowToggleButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlwaysShowToggleButtonProperty =
            DependencyProperty.Register("AlwaysShowToggleButton", typeof(bool), typeof(MenuButton), new UIPropertyMetadata(true));

        #endregion AlwaysShowToggleButton

        #endregion Properties

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuButtonItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuButtonItem;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }

        #endregion Methods
    }
}
