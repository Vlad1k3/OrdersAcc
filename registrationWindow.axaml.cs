using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;
using System;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using System.Text.RegularExpressions;
using Avalonia.Input;
using System.Threading.Tasks;

namespace OrdersAcc
{
    public partial class registrationWindow : Window
    {
        public registrationWindow()
        {
            InitializeComponent();
            txtFIO.AddHandler(TextInputEvent, OnFIOTextInput, RoutingStrategies.Tunnel);
            txtPhoneNumber.AddHandler(TextInputEvent, OnPhoneTextInput, RoutingStrategies.Tunnel);
        }

        private async void CreateProfilerButton_Click(object sender, RoutedEventArgs e)
        {
            string fio = txtFIO.Text;
            string phone = txtPhoneNumber.Text;
            string login = txtLogin.Text;
            string password = txtPassword.Text;
            string jobTitle = txtJobtitle.Text;

            if (jobTitle.ToLower() == "admin")
            {


                var passwordDialog = new PasswordDialog();
                await passwordDialog.ShowDialog(this);

                if (passwordDialog.Password == "102938")
                {
                    CreateProfile(fio, phone, login, password, jobTitle);
                }
                else
                {
                    await MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                    {
                        ButtonDefinitions = ButtonEnum.Ok,
                        ContentTitle = "Ошибка",
                        ContentMessage = "Неверный пароль администратора!"
                    }).ShowWindowDialogAsync(this);
                }
                
            }
            else
            {
                if (!ValidateFields(fio, phone, login, password, jobTitle))
                {
                    return;
                }

                CreateProfile(fio, phone, login, password, jobTitle);
            }
        }

        private void CreateProfile(string fio, string phone, string login, string password, string jobTitle)
        {
            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
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

        private bool ValidateFields(string fio, string phone, string login, string password, string jobTitle)
        {
            if (string.IsNullOrWhiteSpace(fio) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(jobTitle))
            {
                var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "Alert",
                    ContentMessage = "Все поля должны быть заполнены"
                }).ShowWindowDialogAsync(this);
                return false;
            }

            if (!Regex.IsMatch(fio, @"^[a-zA-Zа-яА-Я\s]+$"))
            {
                var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "Alert",
                    ContentMessage = "Поле 'ФИО' должно содержать только буквы."
                }).ShowWindowDialogAsync(this);
                return false;
            }

            if (!Regex.IsMatch(phone, @"^\+?\d+$"))
            {
                var box = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    ContentTitle = "Alert",
                    ContentMessage = "Поле 'Номер телефона' должно содержать только цифры и может начинаться с '+'"
                }).ShowWindowDialogAsync(this);
                return false;
            }

            return true;
        }

        private void OnFIOTextInput(object sender, TextInputEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[a-zA-Zа-яА-Я\s]+$"))
            {
                e.Handled = true;
            }
        }

        private void OnPhoneTextInput(object sender, TextInputEventArgs e)
        {
            var textBox = sender as TextBox;
            string newText = textBox.Text + e.Text;

            if (!Regex.IsMatch(newText, @"^\+?\d*$"))
            {
                e.Handled = true;
            }
        }
    }
}
