using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            if (!CurrentUser.HasPermission("УправлениеПользователями"))
            {
                MessageBox.Show("Нет доступа.");
                return;
            }
            LoadUsers();
        }

        private void LoadUsers()
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT 
                        u.КодПользователя,
                        s.ФИО,
                        s.Должность,
                        r.НаименованиеРоли,
                        u.Логин,
                        u.Активен
                    FROM dbo.Пользователи u
                    INNER JOIN dbo.Сотрудники s ON u.КодСотрудника = s.КодСотрудника
                    INNER JOIN dbo.Роли r ON u.КодРоли = r.КодРоли
                    ORDER BY s.ФИО";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);

                UsersGrid.ItemsSource = table.DefaultView;
            }
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(EmployeeIdBox.Text, out int employeeId) ||
                !int.TryParse(RoleIdBox.Text, out int roleId))
            {
                MessageBox.Show("EmployeeId и RoleId должны быть числами.");
                return;
            }

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                string query = @"
                    INSERT INTO dbo.Пользователи
                    (КодСотрудника, КодРоли, Логин, Пароль, Активен)
                    VALUES
                    (@employeeId, @roleId, @login, @password, 1)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@employeeId", employeeId);
                    cmd.Parameters.AddWithValue("@roleId", roleId);
                    cmd.Parameters.AddWithValue("@login", LoginBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", PasswordBox.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Пользователь создан.");
            LoadUsers();
        }
    }
}