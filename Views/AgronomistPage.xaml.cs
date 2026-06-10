using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Controls;
using BreweryApp.Data;

namespace BreweryApp.Views
{
    public partial class AgronomistPage : Page
    {
        public AgronomistPage()
        {
            InitializeComponent();
            LoadAll();
        }

        private void LoadAll()
        {
            LoadGrid("SELECT КодУчастка, Название, Площадь, Местоположение FROM dbo.ЗемельныеУчастки", FieldsGrid);
            LoadGrid("SELECT * FROM dbo.Посевы", SowingGrid);
            LoadGrid("SELECT * FROM dbo.ПроцессыВыращивания", GrowingGrid);
            LoadGrid("SELECT * FROM dbo.Урожай", HarvestGrid);
        }

        private void LoadGrid(string query, DataGrid grid)
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                grid.ItemsSource = table.DefaultView;
            }
        }
    }
}