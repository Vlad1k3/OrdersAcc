using Avalonia.Controls;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;
using System;
using MsBox.Avalonia;

namespace OrdersAcc
{
    public partial class ordersWindow : Window
    {
        private string loggedInUser;
        private string userPosition;

        public ordersWindow(string user, string position)
        {
            loggedInUser = user;
            userPosition = position;
            InitializeComponent();

            if (userPosition == "admin")
            {
                Button analyzeButton = new Button
                {
                    Content = "Analyze",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                };
                analyzeButton.Click += AnalyzeButton_Click;
                var stackPanel = this.FindControl<StackPanel>("BottomStackPanel");
                stackPanel.Children.Add(analyzeButton);
            }

            if (orders == null)
            {
                Console.WriteLine("Error: 'orders' ListBox is not found. Please check the XAML definition.");
            }
            else
            {
                LoadOrdersFromDatabase();
            }
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            Window analyzeWindow = new AnalyzeWindow();
            analyzeWindow.Show();
        }
        private void LoadOrdersFromDatabase()
        {
            if (orders == null)
            {
                Console.WriteLine("Error: 'orders' ListBox is null. LoadOrdersFromDatabase will not proceed.");
                return;
            }

            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
            string selectQuery = "SELECT EqModel, EqType, Request_ID FROM Orders";

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
                                int request_ID = reader.GetInt32("Request_ID");
                                string eqModel = reader.GetString("EqModel");
                                string eqType = reader.GetString("EqType");

                                Console.WriteLine($"Adding order to ListBox: {eqType} - {eqModel}");
                                orders.Items.Add($"{request_ID} - {eqType} - {eqModel}");
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

                if (parts.Length < 3)
                {
                    Console.WriteLine("Ошибка: ожидаемая строка формата 'Request_ID - Тип - Модель', но получена: " + selectedOrder);
                    return;
                }

                if (!int.TryParse(parts[0].Trim(), out int request_ID))
                {
                    Console.WriteLine("Ошибка: не удалось преобразовать Request_ID в целое число.");
                    return;
                }

                string eqType = parts[1].Trim();
                string eqModel = parts[2].Trim();

                LoadOrderDetailsFromDatabase(request_ID, eqType, eqModel);
            }
        }

        private void LoadOrderDetailsFromDatabase(int request_ID, string eqType, string eqModel)
        {
            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
            string selectQuery = @"SELECT o.ProblemDesc, o.StartDate, o.Completion_Date, o.Status, u.FIO 
                                FROM Orders o 
                                JOIN Users u ON o.Client_ID = u.User_ID 
                                WHERE o.EqType = @EqType AND o.EqModel = @EqModel AND o.Request_ID = @Request_ID";
            
            try{
                using (var connection = new MySqlConnection(connectionString))
                {
                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Request_ID", request_ID);
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
                                string status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString("Status");


                                if (eqType != null && eqModel != null && problemDesc != null && userName != null && startDate != null && completionDate != null)
                                {
                                    InfoWindow infoWindow = new InfoWindow(eqType, eqModel, problemDesc, userName, startDate, completionDate, status , userPosition, request_ID);
                                    infoWindow.SizeToContent = SizeToContent.WidthAndHeight;
                                    infoWindow.ShowDialog(this);
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

        public void searchOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderNumberTextBox = this.FindControl<TextBox>("orderNumberTextBox");
            if (orderNumberTextBox == null)
            {
                Console.WriteLine("Error: 'orderNumberTextBox' is not found. Please check the XAML definition.");
                return;
            }

            if (!int.TryParse(orderNumberTextBox.Text, out int request_ID))
            {
                Console.WriteLine("Ошибка: не удалось преобразовать номер заказа в целое число.");
                return;
            }

            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
            string selectQuery = @"SELECT o.EqType, o.EqModel, o.ProblemDesc, o.StartDate, o.Completion_Date, u.FIO 
                                   FROM Orders o 
                                   JOIN Users u ON o.Client_ID = u.User_ID 
                                   WHERE o.Request_ID = @Request_ID";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    using (var command = new MySqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Request_ID", request_ID);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string eqType = reader.GetString("EqType");
                                string eqModel = reader.GetString("EqModel");
                                string problemDesc = reader.IsDBNull(reader.GetOrdinal("ProblemDesc")) ? string.Empty : reader.GetString("ProblemDesc");
                                string startDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? string.Empty : reader.GetDateTime("StartDate").ToString("yyyy-MM-dd");
                                string completionDate = reader.IsDBNull(reader.GetOrdinal("Completion_Date")) ? string.Empty : reader.GetDateTime("Completion_Date").ToString("yyyy-MM-dd");
                                string userName = reader.IsDBNull(reader.GetOrdinal("FIO")) ? string.Empty : reader.GetString("FIO");

                                Console.WriteLine($"eqType: {eqType}, eqModel: {eqModel}, problemDesc: {problemDesc}, userName: {userName}, startDate: {startDate}, completionDate: {completionDate}");

                                InfoWindow infoWindow = new InfoWindow(eqType, eqModel, problemDesc, userName, startDate, completionDate, "gjitk yf[eq]", userPosition, 1);
                                infoWindow.SizeToContent = SizeToContent.WidthAndHeight;
                                infoWindow.ShowDialog(this);
                            }
                            else
                            {
                                var box = MessageBoxManager.GetMessageBoxStandard("Alert", "Заказ с указанным номером не был найден").ShowAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при выполнении поиска заказа: " + ex.Message);
            }
        }
        private void backButton_Click(object sender, RoutedEventArgs e){

            Window newWindow = new MainWindow();
            newWindow.Show();
            this.Close();
        }
    }
}
