using MySql.Data.MySqlClient;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace OrdersAcc
{
    public partial class createOrderWindow : Window
    {
        private string loggedInUser;
        private string selectedEqType; 

        public createOrderWindow(string user)
        {
            loggedInUser = user;
            InitializeComponent();
        }

        private void EqTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem != null)
            {
                selectedEqType = (comboBox.SelectedItem as ComboBoxItem).Content.ToString();
                Console.WriteLine($"Выбранный тип устройства: {selectedEqType}");
            }
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";

            DateTime currentDate = DateTime.Now;
            DateTime completionDate = currentDate.AddDays(2);
            string problemDesc = txtProblemDesc.Text;
            string eqModel = txtEqModel.Text;

            if (string.IsNullOrEmpty(selectedEqType))
            {
                Console.WriteLine("Ошибка: Тип устройства не выбран.");
                return;
            }

            int clientId = GetClientIdByUsername(loggedInUser, connectionString);

            if (clientId == -1)
            {
                Console.WriteLine("Ошибка: Не удалось найти клиента с указанным именем.");
                return;
            }

            string insertQuery = "INSERT INTO Orders (Master_ID, Status, Client_ID, ProblemDesc, EqModel, EqType, StartDate, Completion_Date) " +
                "VALUES (@Master_ID, @Status, @Client_ID, @ProblemDesc, @EqModel, @EqType, @StartDate, @Completion_Date)";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Master_ID", 1);
                        command.Parameters.AddWithValue("@Status", "New");
                        command.Parameters.AddWithValue("@Client_ID", clientId);
                        command.Parameters.AddWithValue("@ProblemDesc", problemDesc);
                        command.Parameters.AddWithValue("@EqModel", eqModel);
                        command.Parameters.AddWithValue("@EqType", selectedEqType);
                        command.Parameters.AddWithValue("@StartDate", currentDate.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@Completion_Date", completionDate.ToString("yyyy-MM-dd"));

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Запись успешно сохранена.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: Запись не была сохранена.");
                        }
                    }

                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при сохранении заказа: " + ex.Message);
            }
        }

        private int GetClientIdByUsername(string username, string connectionString)
        {
            int clientId = -1;
            string selectQuery = "SELECT User_ID FROM Users WHERE Login = @Username"; 

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                clientId = reader.GetInt32("User_ID");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при получении ID клиента: " + ex.Message);
            }

            return clientId;
        }
    }
}
