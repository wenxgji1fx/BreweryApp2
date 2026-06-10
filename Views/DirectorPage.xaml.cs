using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Controls;
using BreweryApp.Data;

namespace BreweryApp.Views
{
    public partial class DirectorPage : Page
    {
        public DirectorPage()
        {
            InitializeComponent();
        }

        private void OrdersReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadGrid(@"
                SELECT 
                    z.КодЗаказа,
                    k.Название AS Клиент,
                    z.ДатаЗаказа,
                    z.Статус
                FROM dbo.Заказы z
                INNER JOIN dbo.Клиенты k ON z.КодКлиента = k.КодКлиента
                ORDER BY z.КодЗаказа DESC");
        }

        private void StockReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadGrid(@"
                SELECT 
                    s.Название AS Склад,
                    SUM(o.Количество) AS Остаток
                FROM dbo.ОстаткиГотовойПродукции o
                INNER JOIN dbo.СкладГотовойПродукции s
                    ON o.КодСкладаГотовойПродукции = s.КодСкладаГотовойПродукции
                GROUP BY s.Название");
        }

        private void TasksReport_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadGrid(@"
                SELECT 
                    Подразделение,
                    Статус,
                    COUNT(*) AS Количество
                FROM dbo.Задачи
                GROUP BY Подразделение, Статус
                ORDER BY Подразделение, Статус");
        }

        private void LoadGrid(string query)
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                DirectorGrid.ItemsSource = table.DefaultView;
            }
        }
    }
}