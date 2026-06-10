using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            CurrentUser.Clear();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            u.КодПользователя,
                            u.КодСотрудника,
                            u.КодРоли,
                            r.НаименованиеРоли,
                            s.ФИО,
                            s.Подразделение
                        FROM dbo.Пользователи u
                        INNER JOIN dbo.Роли r ON u.КодРоли = r.КодРоли
                        INNER JOIN dbo.Сотрудники s ON u.КодСотрудника = s.КодСотрудника
                        WHERE u.Логин = @login
                          AND u.Пароль = @password
                          AND u.Активен = 1
                          AND s.Активен = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Неверный логин или пароль.");
                                return;
                            }

                            CurrentUser.UserId = Convert.ToInt32(reader["КодПользователя"]);
                            CurrentUser.EmployeeId = Convert.ToInt32(reader["КодСотрудника"]);
                            CurrentUser.RoleId = Convert.ToInt32(reader["КодРоли"]);
                            CurrentUser.Role = reader["НаименованиеРоли"].ToString() ?? "";
                            CurrentUser.FIO = reader["ФИО"].ToString() ?? "";
                            CurrentUser.Department = reader["Подразделение"] == DBNull.Value
                                ? ""
                                : reader["Подразделение"].ToString() ?? "";
                        }
                    }

                    LoadPermissions(conn);

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка входа:\n" + ex.Message);
            }
        }

        private void LoadPermissions(SqlConnection conn)
        {
            CurrentUser.Permissions = new List<string>();

            string query = @"
                SELECT НаименованиеПрава
                FROM dbo.V_ПраваПользователя
                WHERE КодПользователя = @userId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userId", CurrentUser.UserId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CurrentUser.Permissions.Add(reader["НаименованиеПрава"].ToString() ?? "");
                    }
                }
            }
        }
    }
}