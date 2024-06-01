using Avalonia.Controls;

namespace OrdersAcc
{
    public partial class InfoWindow : Window
    {
        public InfoWindow(string eqType, string eqModel, string problemDesc, string userName, string startDate, string completionDate)
        {
            InitializeComponent();

            var eqTypeTextBlock = this.FindControl<TextBlock>("EqTypeTextBlock");
            var eqModelTextBlock = this.FindControl<TextBlock>("EqModelTextBlock");
            var problemDescTextBox = this.FindControl<TextBox>("ProblemDescTextBox");
            var userNameTextBlock = this.FindControl<TextBlock>("UserNameTextBlock");
            var startDateTextBlock = this.FindControl<TextBlock>("StartDateTextBlock");
            var completionDateTextBlock = this.FindControl<TextBlock>("CompletionDateTextBlock");



            eqTypeTextBlock.Text = $"Тип устройства: {eqType}";
            eqModelTextBlock.Text = $"Модель устройства: {eqModel}";
            problemDescTextBox.Text = $"Описание проблемы: {problemDesc}";
            userNameTextBlock.Text = $"Пользователь: {userName}";
            startDateTextBlock.Text = $"Дата начала работ: {startDate}";
            completionDateTextBlock.Text = $"Дата окончания работ: {completionDate}";
        }
    }
}
