using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfOpenControls.DockManager
{
    /// <summary>
    /// Interaction logic for NewThemeView.xaml
    /// </summary>
    public partial class NewThemeView : Window
    {
        public NewThemeView()
        {
            InitializeComponent();
            _keyConverter = new KeyConverter();
        }

        KeyConverter _keyConverter;

        private void _buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string text = _keyConverter.ConvertToString(e.Key);
            if (text.Length > 0)
            {
                if ((sender as TextBox).Text.Length == 0)
                {
                    e.Handled = char.IsDigit(text[0]);
                }
                else
                {
                    e.Handled = !char.IsLetterOrDigit(text[0]);
                }
            }
        }
    }
}
