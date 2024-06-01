using Avalonia.Controls;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace OrdersAcc
{
    public partial class InfoWindow : Window
    {
        private string _eqType;
        private string _eqModel;

        public InfoWindow(string eqType, string eqModel, string problemDesc, string userName, string startDate, string completionDate)
        {
            InitializeComponent();
            _eqType = eqType;
            _eqModel = eqModel;

            var eqTypeTextBlock = this.FindControl<TextBlock>("EqTypeTextBlock");
            var eqModelTextBlock = this.FindControl<TextBlock>("EqModelTextBlock");
            var problemDescTextBlock = this.FindControl<TextBlock>("ProblemDescTextBlock");
            var userNameTextBlock = this.FindControl<TextBlock>("UserNameTextBlock");
            var startDateTextBlock = this.FindControl<TextBlock>("StartDateTextBlock");
            var completionDateTextBlock = this.FindControl<TextBlock>("CompletionDateTextBlock");

            eqTypeTextBlock.Text = $"Тип устройства: {eqType}";
            eqModelTextBlock.Text = $"Модель устройства: {eqModel}";
            problemDescTextBlock.Text = $"Описание проблемы: {problemDesc}";
            userNameTextBlock.Text = $"Пользователь: {userName}";
            startDateTextBlock.Text = $"Дата начала работ: {startDate}";
            completionDateTextBlock.Text = $"Дата окончания работ: {completionDate}";
        }

        private async void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ShowConfirmationDialog("Подтверждение удаления", "Вы уверены, что хотите удалить эту запись?");

            if (result == MessageBoxResult.Yes)
            {
                DeleteOrder();
            }
        }

        private void DeleteOrder()
        {
            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805;";
            string deleteQuery = "DELETE FROM Orders WHERE EqType = @EqType AND EqModel = @EqModel";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EqType", _eqType);
                        command.Parameters.AddWithValue("@EqModel", _eqModel);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Запись успешно удалена.");
                            this.Close(); 
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: Запись не была найдена или не была удалена.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при удалении записи: " + ex.Message);
            }
        }

        private async Task<MessageBoxResult> ShowConfirmationDialog(string title, string message)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var stackPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Vertical,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 10
            };

            var textBlock = new TextBlock
            {
                Text = message,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };

            var buttonPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 20
            };

            var yesButton = new Button
            {
                Content = "Да",
                Width = 75
            };

            var noButton = new Button
            {
                Content = "Нет",
                Width = 75
            };

            yesButton.Click += (s, e) => dialog.Close(MessageBoxResult.Yes);
            noButton.Click += (s, e) => dialog.Close(MessageBoxResult.No);

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(buttonPanel);

            dialog.Content = stackPanel;

            return await dialog.ShowDialog<MessageBoxResult>(this);
        }
    }

    public enum MessageBoxResult
    {
        Yes,
        No
    }
}
