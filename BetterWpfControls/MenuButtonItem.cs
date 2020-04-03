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
	public class MenuButtonItem : ComboBoxItem
	{
		#region .ctors

		static MenuButtonItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButtonItem), new FrameworkPropertyMetadata(typeof(MenuButtonItem)));
		}

		#endregion .ctors

		#region Properties

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuButtonItem), new UIPropertyMetadata(null));

		public object CommandParameter
		{
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register("CommandParameter", typeof(object), typeof(MenuButtonItem), new UIPropertyMetadata(null));

		public bool IsHighlightingEnabled
		{
			get { return (bool)GetValue(IsHighlightingEnabledProperty); }
			set { SetValue(IsHighlightingEnabledProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsHighlightingEnabled.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsHighlightingEnabledProperty =
			DependencyProperty.Register("IsHighlightingEnabled", typeof(bool), typeof(MenuButtonItem), new UIPropertyMetadata(true));

		#endregion Properties

		#region Methods

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			if (Command != null && Command.CanExecute(CommandParameter))
			{
				Command.Execute(CommandParameter);
			}
		}

		#endregion Methods
	}
}
