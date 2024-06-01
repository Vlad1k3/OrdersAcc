using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;
using System;

namespace OrdersAcc
{
    public partial class registrationWindow : Window
    {
        public registrationWindow()
        {
            InitializeComponent();
        }

   
        private void CreateProfilerButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";

            string fio = txtFIO.Text;
            string phone = txtPhoneNumber.Text;
            string login = txtLogin.Text;
            string password = txtPassword.Text;
            string jobTitle = txtJobtitle.Text;

            string insertQuery = "INSERT INTO Users (FIO, Phone, Login, Password, Type) VALUES (@fio, @phone, @login, @password, @jobTitle)";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@fio", fio);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);
                        command.Parameters.AddWithValue("@jobTitle", jobTitle);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Данные успешно сохранены в базе данных!");
                            this.Close();
                        }
                        else
                        {
                            Console.WriteLine("Ошибка при сохранении данных в базу данных.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }
    }
}
