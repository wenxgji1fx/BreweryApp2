using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class StockPage : Page
    {
        public StockPage()
        {
            InitializeComponent();

            if (!CurrentUser.HasPermission("ПросмотрСклада"))
            {
                MessageBox.Show("У вас нет доступа к складу.");
                return;
            }

            LoadStock();
        }

        private void LoadStock()
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            o.КодОстаткаГотовойПродукции AS [Код остатка],
                            s.Название AS [Склад],
                            o.КодПартииГотовойПродукции AS [Партия],
                            o.Количество AS [Количество]
                        FROM dbo.ОстаткиГотовойПродукции o
                        INNER JOIN dbo.СкладГотовойПродукции s
                            ON o.КодСкладаГотовойПродукции = s.КодСкладаГотовойПродукции
                        ORDER BY s.Название, o.КодПартииГотовойПродукции";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    StockGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки остатков:\n" + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadStock();
        }
    }
}