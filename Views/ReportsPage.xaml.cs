using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();

            if (!CurrentUser.HasPermission("ПросмотрОтчетовПодразделения") &&
                !CurrentUser.HasPermission("ПросмотрВсехОтчетов"))
            {
                MessageBox.Show("У вас нет доступа к отчетам.");
                return;
            }
        }

        private void ReportClients_Click(object sender, RoutedEventArgs e)
        {
            LoadReport(@"
                SELECT 
                    k.Название AS [Клиент],
                    SUM(sz.Количество) AS [Общий объем продаж]
                FROM dbo.Заказы z
                INNER JOIN dbo.Клиенты k ON z.КодКлиента = k.КодКлиента
                INNER JOIN dbo.СоставЗаказа sz ON z.КодЗаказа = sz.КодЗаказа
                GROUP BY k.Название
                ORDER BY k.Название");
        }

        private void ReportStock_Click(object sender, RoutedEventArgs e)
        {
            LoadReport(@"
                SELECT 
                    s.Название AS [Склад],
                    SUM(o.Количество) AS [Остаток]
                FROM dbo.ОстаткиГотовойПродукции o
                INNER JOIN dbo.СкладГотовойПродукции s
                    ON o.КодСкладаГотовойПродукции = s.КодСкладаГотовойПродукции
                GROUP BY s.Название
                ORDER BY s.Название");
        }

        private void ReportOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadReport(@"
                SELECT
                    z.КодЗаказа AS [Код заказа],
                    k.Название AS [Клиент],
                    sz.КодПартииГотовойПродукции AS [Партия],
                    sz.Количество AS [Количество]
                FROM dbo.СоставЗаказа sz
                INNER JOIN dbo.Заказы z ON sz.КодЗаказа = z.КодЗаказа
                INNER JOIN dbo.Клиенты k ON z.КодКлиента = k.КодКлиента
                ORDER BY z.КодЗаказа DESC");
        }

        private void LoadReport(string query)
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    ReportGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка формирования отчета:\n" + ex.Message);
            }
        }
    }
}