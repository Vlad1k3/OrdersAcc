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
    public partial class InfoWindow : Window
    {
        private int Request_ID;
        private string _eqType;
        private string _eqModel;
        private string userPosition;

        public InfoWindow(string eqType, string eqModel, string problemDesc, string userName, string startDate, string completionDate, string status, string position, int request_ID)
        {
            InitializeComponent();

            Request_ID = request_ID;
            _eqType = eqType;
            _eqModel = eqModel;
            userPosition = position;


            var eqTypeTextBox = this.FindControl<TextBox>("EqTypeTextBox");
            var eqModelTextBox = this.FindControl<TextBox>("EqModelTextBox");
            var statusTextBox = this.FindControl<TextBox>("StatusTextBox");
            var problemDescTextBox = this.FindControl<TextBox>("ProblemDescTextBox");
            var userNameTextBox = this.FindControl<TextBox>("UserNameTextBox");
            var startDateTextBox = this.FindControl<TextBox>("StartDateTextBox");
            var completionDateTextBox = this.FindControl<TextBox>("CompletionDateTextBox");

            eqTypeTextBox.Text = eqType;
            eqModelTextBox.Text = eqModel;
            statusTextBox.Text = status;
            problemDescTextBox.Text = problemDesc;
            userNameTextBox.Text = userName;
            startDateTextBox.Text = startDate;
            completionDateTextBox.Text = completionDate;
       

            if (userPosition != "admin"){
                eqTypeTextBox.IsReadOnly = true;
                eqModelTextBox.IsReadOnly = true;
                statusTextBox.IsReadOnly = true;
                problemDescTextBox.IsReadOnly = true;
                userNameTextBox.IsReadOnly = true;
                startDateTextBox.IsReadOnly = true;
                completionDateTextBox.IsReadOnly = true;
            }
            else{
                eqTypeTextBox.IsReadOnly = false;
                eqModelTextBox.IsReadOnly = false;
                statusTextBox.IsReadOnly = false;
                problemDescTextBox.IsReadOnly = false;
                userNameTextBox.IsReadOnly = false;
                startDateTextBox.IsReadOnly = false;
                completionDateTextBox.IsReadOnly = false;
            }
            if (userPosition == "admin")
            {
                Button doneButton= new Button
                {
                    Content = "Done",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                };
                doneButton.Click += SubmitButton_Click;
                var stackPanel = this.FindControl<StackPanel>("MainStackPanel");
                stackPanel.Children.Add(doneButton);
            }
        }

        private async void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ShowConfirmationDialog("Подтверждение удаления", "Вы уверены, что хотите удалить эту запись?");

            if (result == MessageBoxResult.Yes)
            {
                DeleteOrder();
            }
        }
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            string eqType = EqTypeTextBox.Text;
            string eqModel = EqModelTextBox.Text;
            string status = StatusTextBox.Text;
            string UserName = UserNameTextBox.Text;
            string StartDate = StartDateTextBox.Text;
            string Completion_Date = CompletionDateTextBox.Text;
            string ProblemDesc = ProblemDescTextBox.Text;

            var box = MessageBoxManager.GetMessageBoxStandard("Уведомление", "Время добавлено!", MsBox.Avalonia.Enums.ButtonEnum.Ok);
            var res = await box.ShowWindowDialogAsync(this);
            string connectionString = "Server=localhost;Port=3306;Database=OrdersApp;Uid=ssofixd;Pwd=290805";
            string insertQuery = "UPDATE Orders SET ProblemDesc = @ProblemDesc, EqModel = @eqModel, Status = @status, EqType = @eqType, StartDate = @StartDate, Completion_Date = @Completion_Date WHERE Request_ID = @Request_ID";
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ProblemDesc", ProblemDesc);
                        command.Parameters.AddWithValue("@eqModel", eqModel);
                        command.Parameters.AddWithValue("@eqType", eqType);
                        command.Parameters.AddWithValue("@status", status);
                        command.Parameters.AddWithValue("@StartDate", StartDate);
                        command.Parameters.AddWithValue("@Completion_Date", Completion_Date );
                        command.Parameters.AddWithValue("@Request_ID", Request_ID);
                        connection.Open();
                        command.ExecuteNonQuery();
                        if(res == MsBox.Avalonia.Enums.ButtonResult.Ok){
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
