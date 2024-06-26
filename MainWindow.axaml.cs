using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using MsBox.Avalonia;
using MySql.Data.MySqlClient;

namespace OrdersAcc
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void registrationButton_Click(object sender, RoutedEventArgs e)
        {
            Window newWindow = new registrationWindow();
            newWindow.Show();
        }

    public void switchWindowButton(object sender, RoutedEventArgs e)
    {
        string loginName = UsernameTextBox.Text;
        string password = PasswordTextBox.Text;

        string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
        string selectQuery = "SELECT User_ID, Type FROM Users WHERE Login = @login AND Password = @password";

        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@login", loginName);
                    command.Parameters.AddWithValue("@password", password);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string loggedInUser = loginName;
                            string userType = reader.GetString("Type"); // Получение типа пользователя
                            Window newWindow = new ordersWindow(loggedInUser, userType);
                            newWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            var box = MessageBoxManager.GetMessageBoxStandard("Error", "Invalid login or password.").ShowAsync();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    }
}
