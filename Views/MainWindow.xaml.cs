using System.Windows;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            UserInfoText.Text = $"{CurrentUser.FIO}\nРоль: {CurrentUser.Role}\nПодразделение: {CurrentUser.Department}";
            ConfigureMenu();
            OpenStartPage();
        }

        private void ConfigureMenu()
        {
            if (CurrentUser.HasPermission("УправлениеПользователями"))
                AdminButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("ПросмотрВсехОтчетов"))
                DirectorButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("ПросмотрАгрономии"))
                AgronomistButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("ПросмотрЗаказов"))
                OrdersButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("ПросмотрСклада"))
                StockButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("ПросмотрСвоихЗадач"))
                MyTasksButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("ПостановкаЗадач"))
                ManagerTasksButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("ПросмотрОтчетовПодразделения") ||
                CurrentUser.HasPermission("ПросмотрВсехОтчетов"))
                ReportsButton.Visibility = Visibility.Visible;
            if (CurrentUser.HasPermission("ПросмотрВарок") || CurrentUser.HasPermission("ИсполнениеВарок"))
                BrewerButton.Visibility = Visibility.Visible;

            if (CurrentUser.HasPermission("КонтрольКачества"))
                LabButton.Visibility = Visibility.Visible;
        }

        private void OpenStartPage()
        {
            if (CurrentUser.HasPermission("УправлениеПользователями"))
                MainFrame.Navigate(new AdminPage());
            else if (CurrentUser.HasPermission("ПросмотрВсехОтчетов"))
                MainFrame.Navigate(new DirectorPage());
            else if (CurrentUser.HasPermission("ПросмотрАгрономии"))
                MainFrame.Navigate(new AgronomistPage());
            else if (CurrentUser.HasPermission("ПросмотрСвоихЗадач"))
                MainFrame.Navigate(new MyTasksPage());
            else if (CurrentUser.HasPermission("ПросмотрВарок") || CurrentUser.HasPermission("ИсполнениеВарок"))
                MainFrame.Navigate(new BrewerPanel());
            else if (CurrentUser.HasPermission("КонтрольКачества"))
                MainFrame.Navigate(new LabPanel());
        }

        private void BrewerButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BrewerPanel()); // твой UserControl для пивовара
        }

        private void LabButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LabPanel()); // твой UserControl для лаборанта
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AdminPage());
        private void DirectorButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new DirectorPage());
        private void AgronomistButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AgronomistPage());
        private void OrdersButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new OrdersPage());
        private void StockButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new StockPage());
        private void MyTasksButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new MyTasksPage());
        private void ManagerTasksButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ManagerTasksPage());
        private void ReportsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ReportsPage());

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            new LoginWindow().Show();
            Close();
        }
    }
}