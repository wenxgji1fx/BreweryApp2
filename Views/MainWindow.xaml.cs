using System;
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

            AdminButton.Visibility = Visibility.Collapsed;
            DirectorButton.Visibility = Visibility.Collapsed;
            AgronomistButton.Visibility = Visibility.Collapsed;
            OrdersButton.Visibility = Visibility.Collapsed;
            StockButton.Visibility = Visibility.Collapsed;
            MyTasksButton.Visibility = Visibility.Collapsed;
            ManagerTasksButton.Visibility = Visibility.Collapsed;
            ReportsButton.Visibility = Visibility.Collapsed;
            BrewerButton.Visibility = Visibility.Collapsed;
            LabButton.Visibility = Visibility.Collapsed;

            LogistButton.Visibility = Visibility.Collapsed;
            DriverButton.Visibility = Visibility.Collapsed;
            MechanicButton.Visibility = Visibility.Collapsed;
            OperatorBrewingButton.Visibility = Visibility.Collapsed;
            OperatorFiltrationButton.Visibility = Visibility.Collapsed;
            MasterSectionButton.Visibility = Visibility.Collapsed;

            // Администратор
            if (IsRole("Администратор") || CurrentUser.HasPermission("УправлениеПользователями"))
            {
                AdminButton.Visibility = Visibility.Visible;
                ReportsButton.Visibility = Visibility.Visible;
            }

            // Директор
            if (IsRole("Директор") || CurrentUser.HasPermission("ПросмотрВсехОтчетов"))
            {
                DirectorButton.Visibility = Visibility.Visible;
                ReportsButton.Visibility = Visibility.Visible;
                ManagerTasksButton.Visibility = Visibility.Visible;
            }

            // Руководитель производства
            if (IsRole("Руководитель производства"))
            {
                OperatorBrewingButton.Visibility = Visibility.Visible;
                OperatorFiltrationButton.Visibility = Visibility.Visible;
                ManagerTasksButton.Visibility = Visibility.Visible;
                ReportsButton.Visibility = Visibility.Visible;
            }

            // Пивовар
            if (IsRole("Пивовар"))
            {
                BrewerButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // OperatorBrewing
            if (IsRole("OperatorBrewing"))
            {
                OperatorBrewingButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // OperatorFiltration
            if (IsRole("OperatorFiltration"))
            {
                OperatorFiltrationButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Лаборант
            if (IsRole("Лаборант") || CurrentUser.HasPermission("КонтрольКачества"))
            {
                LabButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Руководитель продаж
            if (IsRole("Руководитель продаж"))
            {
                OrdersButton.Visibility = Visibility.Visible;
                ManagerTasksButton.Visibility = Visibility.Visible;
                ReportsButton.Visibility = Visibility.Visible;
            }

            // Менеджер продаж
            if (IsRole("Менеджер продаж") || CurrentUser.HasPermission("ПросмотрЗаказов"))
            {
                OrdersButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Главный агроном
            if (IsRole("Главный агроном"))
            {
                AgronomistButton.Visibility = Visibility.Visible;
                ManagerTasksButton.Visibility = Visibility.Visible;
                ReportsButton.Visibility = Visibility.Visible;
            }

            // Агроном
            if (IsRole("Агроном") || CurrentUser.HasPermission("ПросмотрАгрономии"))
            {
                AgronomistButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Кладовщик
            if (IsRole("Кладовщик") || CurrentUser.HasPermission("ПросмотрСклада"))
            {
                StockButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Логист
            if (IsRole("Логист"))
            {
                LogistButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Водитель-экспедитор
            if (IsRole("Водитель-экспедитор"))
            {
                DriverButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Механизатор
            if (IsRole("Механизатор"))
            {
                MechanicButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
            }

            // Мастер участка
            if (IsRole("MasterSection"))
            {
                MasterSectionButton.Visibility = Visibility.Visible;
                MyTasksButton.Visibility = Visibility.Visible;
                ReportsButton.Visibility = Visibility.Visible;
            }
        }

        // ==================== СТАРТОВАЯ СТРАНИЦА ====================

        private void OpenStartPage()
        {
            string role = NormalizeRole(CurrentUser.Role);

            switch (role)
            {
                case "Администратор":
                    MainFrame.Navigate(new AdminPage());
                    break;

                case "Директор":
                    MainFrame.Navigate(new DirectorPage());
                    break;

                case "Руководитель производства":
                    MainFrame.Navigate(new ManagerTasksPage());
                    break;

                case "Пивовар":
                    MainFrame.Navigate(new BrewerPanel());
                    break;

                case "OperatorBrewing":
                    MainFrame.Navigate(new OperatorBrewingPage());
                    break;

                case "OperatorFiltration":
                    MainFrame.Navigate(new OperatorFiltrationPage());
                    break;

                case "Лаборант":
                    MainFrame.Navigate(new LabPanel());
                    break;

                case "Руководитель продаж":
                    MainFrame.Navigate(new ManagerTasksPage());
                    break;

                case "Менеджер продаж":
                    MainFrame.Navigate(new OrdersPage());
                    break;

                case "Главный агроном":
                    MainFrame.Navigate(new AgronomistPage());
                    break;

                case "Агроном":
                    MainFrame.Navigate(new AgronomistPage());
                    break;

                case "Кладовщик":
                    MainFrame.Navigate(new StockPage());
                    break;

                case "Логист":
                    MainFrame.Navigate(new LogistPage());
                    break;

                case "Водитель-экспедитор":
                    MainFrame.Navigate(new DriverPage());
                    break;

                case "Механизатор":
                    MainFrame.Navigate(new MechanicPage());
                    break;

                case "MasterSection":
                    MainFrame.Navigate(new MasterSectionPage());
                    break;

                default:
                    OpenStartPageByPermissions();
                    break;
            }
        }

        private void OpenStartPageByPermissions()
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
            else
                MessageBox.Show(
                    $"Для роли \"{CurrentUser.Role}\" стартовая страница не настроена.",
                    "Ошибка роли",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }

        // ==================== ОБРАБОТЧИКИ КНОПОК ====================

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminPage());
        }

        private void DirectorButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DirectorPage());
        }

        private void AgronomistButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AgronomistPage());
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void StockButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StockPage());
        }

        private void MyTasksButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MyTasksPage());
        }

        private void ManagerTasksButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ManagerTasksPage());
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsPage());
        }

        private void BrewerButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BrewerPanel());
        }

        private void LabButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LabPanel());
        }

        private void LogistButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LogistPage());
        }

        private void DriverButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DriverPage());
        }

        private void MechanicButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MechanicPage());
        }

        private void OperatorBrewingButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OperatorBrewingPage());
        }

        private void OperatorFiltrationButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OperatorFiltrationPage());
        }

        private void MasterSectionButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MasterSectionPage());
        }

        // ==================== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ====================

        private bool IsRole(params string[] roles)
        {
            string currentRole = NormalizeRole(CurrentUser.Role);

            foreach (string role in roles)
            {
                if (currentRole == NormalizeRole(role))
                    return true;
            }

            return false;
        }

        private string NormalizeRole(string role)
        {
            return string.IsNullOrWhiteSpace(role)
                ? string.Empty
                : role.Trim();
        }

        // ==================== ВЫХОД ====================

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            new LoginWindow().Show();
            Close();
        }
    }
}