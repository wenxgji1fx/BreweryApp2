using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            InitializeComponent();

            if (!CurrentUser.HasPermission("ПросмотрЗаказов"))
            {
                MessageBox.Show("У вас нет доступа к заказам.");
                return;
            }

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            z.КодЗаказа AS [Код заказа],
                            k.Название AS [Клиент],
                            s.ФИО AS [Менеджер],
                            z.ДатаЗаказа AS [Дата заказа],
                            z.Статус AS [Статус]
                        FROM dbo.Заказы z
                        INNER JOIN dbo.Клиенты k ON z.КодКлиента = k.КодКлиента
                        INNER JOIN dbo.Сотрудники s ON z.КодМенеджера = s.КодСотрудника
                        ORDER BY z.КодЗаказа DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    OrdersGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заказов:\n" + ex.Message);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentUser.HasPermission("РедактированиеЗаказов"))
            {
                MessageBox.Show("У вас нет права добавлять заказы.");
                return;
            }

            AddOrderWindow win = new AddOrderWindow();
            win.Owner = Window.GetWindow(this);
            win.ShowDialog();
            LoadOrders();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentUser.HasPermission("РедактированиеЗаказов"))
            {
                MessageBox.Show("У вас нет права удалять заказы.");
                return;
            }

            if (OrdersGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите заказ для удаления.");
                return;
            }

            DataRowView row = (DataRowView)OrdersGrid.SelectedItem;
            int orderId = Convert.ToInt32(row["Код заказа"]);

            MessageBoxResult result = MessageBox.Show(
                "Удалить заказ и все его позиции?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string deleteItemsQuery = "DELETE FROM dbo.СоставЗаказа WHERE КодЗаказа = @orderId";
                        using (SqlCommand cmdItems = new SqlCommand(deleteItemsQuery, conn, transaction))
                        {
                            cmdItems.Parameters.AddWithValue("@orderId", orderId);
                            cmdItems.ExecuteNonQuery();
                        }

                        string deleteOrderQuery = "DELETE FROM dbo.Заказы WHERE КодЗаказа = @orderId";
                        using (SqlCommand cmdOrder = new SqlCommand(deleteOrderQuery, conn, transaction))
                        {
                            cmdOrder.Parameters.AddWithValue("@orderId", orderId);
                            cmdOrder.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Заказ удален.");
                        LoadOrders();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления заказа:\n" + ex.Message);
            }
        }
    }
}