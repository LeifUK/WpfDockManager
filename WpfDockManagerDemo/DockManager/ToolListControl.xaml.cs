using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections;
using System.Windows.Media;

namespace WpfDockManagerDemo.DockManager
{
    /// <summary>
    /// Interaction logic for ToolListControl.xaml
    /// </summary>
    public partial class ToolListControl : UserControl
    {
        public ToolListControl()
        {
            InitializeComponent();
        }

        public event EventHandler ItemClick;

        #region Dependency properties

        #region ItemsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(System.Collections.ObjectModel.ObservableCollection<ToolListItem>), typeof(ToolListControl), new FrameworkPropertyMetadata((System.Collections.ObjectModel.ObservableCollection<ToolListItem>)null, new PropertyChangedCallback(OnItemsSourceChanged)));

        internal System.Collections.ObjectModel.ObservableCollection<ToolListItem> ItemsSource
        {
            get
            {
                return (System.Collections.ObjectModel.ObservableCollection<ToolListItem>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolListControl)d).OnItemsSourceChanged(e);
        }

        private void PrepareItemsSource(System.Collections.ObjectModel.ObservableCollection<ToolListItem> itemsSource)
        {
            _listBox.Items.Clear();
            foreach (var item in itemsSource)
            {
                _listBox.Items.Add(item);
            }
            if (_listBox.Items.Count > 0)
            {
                _listBox.SelectedIndex = 0;
            }
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PrepareItemsSource(e.NewValue as System.Collections.ObjectModel.ObservableCollection<ToolListItem>);

                if (ItemsSource is System.Collections.Specialized.INotifyCollectionChanged)
                {
                    (ItemsSource as System.Collections.Specialized.INotifyCollectionChanged).CollectionChanged += TabeHeaderControl_CollectionChanged;
                }
            }
        }

        private void TabeHeaderControl_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PrepareItemsSource(ItemsSource);
        }

        #endregion

        #region SelectedIndex dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ToolListControl), new FrameworkPropertyMetadata(-1, new PropertyChangedCallback(OnSelectedIndexChanged)));

        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                if (value != SelectedIndex)
                {
                    SetValue(SelectedIndexProperty, value);
                }
            }
        }

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolListControl)d).OnSelectedIndexChanged(e);
        }

        protected virtual void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.SelectedIndex = (int)e.NewValue;
                _listBox.ScrollIntoView(_listBox.SelectedItem);
            }
        }

        #endregion

        #region DisplayMemberPath dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(ToolListControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnDisplayMemberPathChanged)));

        public string DisplayMemberPath
        {
            get
            {
                return (string)GetValue(DisplayMemberPathProperty);
            }
            set
            {
                if (value != DisplayMemberPath)
                {
                    SetValue(DisplayMemberPathProperty, value);
                }
            }
        }

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolListControl)d).OnDisplayMemberPathChanged(e);
        }

        protected virtual void OnDisplayMemberPathChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.DisplayMemberPath = (string)e.NewValue;
            }
        }

        #endregion

        #region IsHorizontal dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty IsHorizontalProperty = DependencyProperty.Register("IsHorizontal", typeof(bool), typeof(ToolListControl), new FrameworkPropertyMetadata(true, OnIsHorizontalChanged));

        public bool IsHorizontal
        {
            get
            {
                return (bool)GetValue(IsHorizontalProperty);
            }
            set
            {
                if (value != IsHorizontal)
                {
                    SetValue(IsHorizontalProperty, value);
                }
            }
        }

        private static void OnIsHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToolListControl)d).OnIsHorizontalChanged(e);
        }

        protected virtual void OnIsHorizontalChanged(DependencyPropertyChangedEventArgs e)
        {
            _rotation.Angle = (bool)e.NewValue ? 0.0 :90.0;
        }

        #endregion

        #region BarBrush dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty BarBrushProperty = DependencyProperty.Register("BarBrush", typeof(Brush), typeof(ToolListControl), new FrameworkPropertyMetadata(Brushes.DarkSlateBlue, null));

        public Brush BarBrush
        {
            get
            {
                return (Brush)GetValue(BarBrushProperty);
            }
            set
            {
                if (value != BarBrush)
                {
                    SetValue(BarBrushProperty, value);
                }
            }
        }

        #endregion BarBrush dependency property

        #region BarBrushMouseOver dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty BarBrushMouseOverProperty = DependencyProperty.Register("BarBrushMouseOver", typeof(Brush), typeof(ToolListControl), new FrameworkPropertyMetadata(Brushes.LightSteelBlue, null));

        public Brush BarBrushMouseOver
        {
            get
            {
                return (Brush)GetValue(BarBrushMouseOverProperty);
            }
            set
            {
                if (value != BarBrushMouseOver)
                {
                    SetValue(BarBrushMouseOverProperty, value);
                }
            }
        }

        #endregion BarBrushMouseOver dependency property

        #endregion Dependency properties

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_listBox.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            // SelectedIndex is yet to be set so we must find it outselves ... 
            Point cursorScreenPosition = WpfControlLibrary.Utilities.GetCursorPosition();

            for (int index = 0; index < _listBox.Items.Count; ++index)
            {
                ListBoxItem item = _listBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;

                Point cursorItemPosition = item.PointFromScreen(cursorScreenPosition);
                if ((cursorItemPosition.X >= 0) && (cursorItemPosition.Y >= 0) && (cursorItemPosition.X <= item.ActualWidth) && (cursorItemPosition.Y <= item.ActualHeight))
                {
                    ItemClick?.Invoke(item.DataContext, null);
                    return;
                }
            }
        }

        private void _listBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Warning warning TBD
        }
    }
}
