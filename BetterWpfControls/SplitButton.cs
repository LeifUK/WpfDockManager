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
using System.Windows.Markup;

namespace BetterWpfControls
{
	[ContentProperty("Items")]
	public class SplitButton : ComboBox
	{
		#region .ctors

		static SplitButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
		}

		public SplitButton()
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
			DependencyProperty.Register("Content", typeof(object), typeof(SplitButton), new UIPropertyMetadata(null));

		#endregion Content

		#region Command

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton), new UIPropertyMetadata(null));

		#endregion Command

		#region CommandParameter

		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton), new UIPropertyMetadata(null));

		#endregion CommandParameter

		#region ShowOptions

		public bool ShowOptions
		{
			get { return (bool)GetValue(ShowOptionsProperty); }
			set { SetValue(ShowOptionsProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ShowOptions.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ShowOptionsProperty =
			DependencyProperty.Register("ShowOptions", typeof(bool), typeof(SplitButton), new UIPropertyMetadata(true));

		#endregion ShowOptions

		#endregion Properties

		#region Methods

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown(e);

			if (e.Source is ComboBoxItem)
			{
				IsDropDownOpen = false;

				SelectedItem = e.Source;

				if (e.Source is MenuButtonItem)
				{
					var m = e.Source as MenuButtonItem;
					if (m.Command != null)
					{
						if (m.Command.CanExecute(m.CommandParameter))
						{
							m.Command.Execute(m.CommandParameter);
						}
					}
				}
			}
		}

		#endregion Methods
	}
}
