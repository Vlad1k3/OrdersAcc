using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace OrdersAcc
{
    public partial class PasswordDialog : Window
    {
        public string Password { get; private set; }

        public PasswordDialog()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Password = this.FindControl<TextBox>("passwordBox").Text;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Password = null;
            this.Close();
        }
    }
}
