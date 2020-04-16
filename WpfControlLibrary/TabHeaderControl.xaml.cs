using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfControlLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class TabHeaderControl : UserControl, INotifyPropertyChanged
    {
        public TabHeaderControl()
        {
            InitializeComponent();
        }

        #region Dependency properties

        #region SelectedItem dependency property

        /*
         * The data context of the selected framework element
         */

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(Object), typeof(TabHeaderControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnSelectedItemChanged)));
        public Object SelectedItem
        {
            get
            {
                return GetValue(SelectedItemProperty);
            }
            set
            {
                if (value != SelectedItem)
                {
                    SetValue(SelectedItemProperty, value);
                }
            }
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnSelectedItemChanged(e);
        }

        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                _listBox.SelectedItem = e.NewValue;
            }
        }

        #endregion

        #region ItemsSource dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TabHeaderControl), new FrameworkPropertyMetadata((IEnumerable)null, new PropertyChangedCallback(OnItemsSourceChanged)));

        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnItemsSourceChanged(e);
        }

        private void PrepareItemsSource(IEnumerable itemsSource)
        {
            _listBox.Items.Clear();

            foreach (var item in itemsSource)
            {
                _listBox.Items.Add(item);
            }
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                PrepareItemsSource(e.NewValue as IEnumerable);

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

        #region SelectedTabBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedTabBackgroundProperty = DependencyProperty.Register("SelectedTabBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnSelectedTabBackgroundChanged)));
        public Brush SelectedTabBackground
        {
            get
            {
                return (Brush)GetValue(SelectedTabBackgroundProperty);
            }
            set
            {
                if (value != SelectedTabBackground)
                {
                    SetValue(SelectedTabBackgroundProperty, value);
                }
            }
        }

        private static void OnSelectedTabBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnSelectedTabBackgroundChanged(e);
        }

        protected virtual void OnSelectedTabBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // Warning warning
                //TheSelectedTabBackground = (Brush)e.NewValue;
            }
        }

        #endregion

        #region UnselectedTabBackground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedTabBackgroundProperty = DependencyProperty.Register("UnselectedTabBackground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnUnselectedTabBackgroundChanged)));
        public Brush UnselectedTabBackground
        {
            get
            {
                return (Brush)GetValue(UnselectedTabBackgroundProperty);
            }
            set
            {
                if (value != UnselectedTabBackground)
                {
                    SetValue(UnselectedTabBackgroundProperty, value);
                }
            }
        }

        private static void OnUnselectedTabBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnUnselectedTabBackgroundChanged(e);
        }

        protected virtual void OnUnselectedTabBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // Warning warning
                //TheSelectedTabBackground = (Brush)e.NewValue;
            }
        }

        #endregion

        #region SelectedTabForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty SelectedTabForegroundProperty = DependencyProperty.Register("SelectedTabForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnSelectedTabForegroundChanged)));
        public Brush SelectedTabForeground
        {
            get
            {
                return (Brush)GetValue(SelectedTabForegroundProperty);
            }
            set
            {
                if (value != SelectedTabForeground)
                {
                    SetValue(SelectedTabForegroundProperty, value);
                }
            }
        }

        private static void OnSelectedTabForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnSelectedTabForegroundChanged(e);
        }

        protected virtual void OnSelectedTabForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // Warning warning
                //TheSelectedTabBackground = (Brush)e.NewValue;
            }
        }

        #endregion

        #region UnselectedTabForeground dependency property

        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static readonly DependencyProperty UnselectedTabForegroundProperty = DependencyProperty.Register("UnselectedTabForeground", typeof(Brush), typeof(TabHeaderControl), new FrameworkPropertyMetadata((FrameworkElement)null, new PropertyChangedCallback(OnUnselectedTabForegroundChanged)));
        public Brush UnselectedTabForeground
        {
            get
            {
                return (Brush)GetValue(UnselectedTabForegroundProperty);
            }
            set
            {
                if (value != UnselectedTabForeground)
                {
                    SetValue(UnselectedTabForegroundProperty, value);
                }
            }
        }

        private static void OnUnselectedTabForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabHeaderControl)d).OnUnselectedTabForegroundChanged(e);
        }

        protected virtual void OnUnselectedTabForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                // Warning warning
                //TheSelectedTabBackground = (Brush)e.NewValue;
            }
        }

        #endregion

        #endregion

        private void _listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        // Warning warning
        private Brush _selectedTabBackground;
        public Brush TheSelectedTabBackground
        {
            get
            {
                return _selectedTabBackground;
            }
            set
            {
                _selectedTabBackground = value;
                NotifyPropertyChanged("TheSelectedTabBackground");
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged    
    }
}
