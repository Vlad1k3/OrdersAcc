using Avalonia.Controls;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;
using System;

namespace OrdersAcc
{
    public partial class ordersWindow : Window
    {
        private string loggedInUser;

        public ordersWindow(string user)
        {
            loggedInUser = user;
            InitializeComponent();

            if (orders == null)
            {
                Console.WriteLine("Error: 'orders' ListBox is not found. Please check the XAML definition.");
            }
            else
            {
                LoadOrdersFromDatabase();
            }
        }

        private void LoadOrdersFromDatabase()
        {
            if (orders == null)
            {
                Console.WriteLine("Error: 'orders' ListBox is null. LoadOrdersFromDatabase will not proceed.");
                return;
            }

            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
            string selectQuery = "SELECT EqModel, EqType FROM Orders";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(selectQuery, connection))
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string eqModel = reader.GetString("EqModel");
                                string eqType = reader.GetString("EqType");

                                Console.WriteLine($"Adding order to ListBox: {eqType} - {eqModel}");
                                orders.Items.Add($"{eqType} - {eqModel}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при загрузке заказов из базы данных: " + ex.Message);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null)
            {
                Console.WriteLine("Error: sender is null in ListBox_SelectionChanged.");
                return;
            }

            var listBox = sender as ListBox;

            if (listBox?.SelectedItem != null)
            {
                var selectedOrder = listBox.SelectedItem.ToString();
                string[] parts = selectedOrder.Split(" - ");

                if (parts.Length < 2)
                {
                    Console.WriteLine("Ошибка: ожидаемая строка формата 'Тип - Модель', но получена: " + selectedOrder);
                    return;
                }

                string eqType = parts[0].Trim();
                string eqModel = parts[1].Trim();

                LoadOrderDetailsFromDatabase(eqType, eqModel);
            }
        }

        private void LoadOrderDetailsFromDatabase(string eqType, string eqModel)
        {
            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
            string selectQuery = @"SELECT o.ProblemDesc, o.StartDate, o.Completion_Date, u.FIO 
                                FROM Orders o 
                                JOIN Users u ON o.Client_ID = u.User_ID 
                                WHERE o.EqType = @EqType AND o.EqModel = @EqModel";
            
            try{
                using (var connection = new MySqlConnection(connectionString))
                {
                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EqType", eqType);
                        command.Parameters.AddWithValue("@EqModel", eqModel);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string problemDesc = reader.IsDBNull(reader.GetOrdinal("ProblemDesc")) ? string.Empty : reader.GetString("ProblemDesc");
                                string startDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? string.Empty : reader.GetDateTime("StartDate").ToString("yyyy-MM-dd");
                                string completionDate = reader.IsDBNull(reader.GetOrdinal("Completion_Date")) ? string.Empty : reader.GetDateTime("Completion_Date").ToString("yyyy-MM-dd");
                                string userName = reader.IsDBNull(reader.GetOrdinal("FIO")) ? string.Empty : reader.GetString("FIO");

                                // Дополнительное логгирование для проверки значений
                                Console.WriteLine($"eqType: {eqType}, eqModel: {eqModel}, problemDesc: {problemDesc}, userName: {userName}, startDate: {startDate}, completionDate: {completionDate}");

                                if (eqType != null && eqModel != null && problemDesc != null && userName != null && startDate != null && completionDate != null)
                                {
                                    InfoWindow infoWindow = new InfoWindow(eqType, eqModel, problemDesc, userName, startDate, completionDate);
                                    infoWindow.Show();
                                }
                                else
                                {
                                    Console.WriteLine("Одно из полей равно null.");
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при загрузке деталей заказа из базы данных: " + ex.Message);
            }
        
            
        }

          
        


        public void updateListButton_Click(object sender, RoutedEventArgs e)
        {
            if (orders == null)
            {
                Console.WriteLine("Error: 'orders' ListBox is null in updateListButton_Click.");
                return;
            }

            orders.Items.Clear();
            LoadOrdersFromDatabase();
        }

        public void createOrderButton(object sender, RoutedEventArgs e)
        {
            Window newWindow = new createOrderWindow(loggedInUser);
            newWindow.Show();
        }
    }
}