using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class MyTasksPage : Page
    {
        public MyTasksPage()
        {
            InitializeComponent();

            if (!CurrentUser.HasPermission("ПросмотрСвоихЗадач"))
            {
                MessageBox.Show("У вас нет доступа к задачам.");
                return;
            }

            LoadTasks();
        }

        private void LoadTasks()
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT
                            z.КодЗадачи AS [Код],
                            z.Заголовок AS [Заголовок],
                            z.Описание AS [Описание],
                            z.Подразделение AS [Подразделение],
                            z.ТипЗадачи AS [Тип],
                            z.Статус AS [Статус],
                            boss.ФИО AS [Постановщик],
                            z.ДатаСоздания AS [Дата создания],
                            z.Дедлайн AS [Дедлайн],
                            z.КомментарийИсполнителя AS [Комментарий]
                        FROM dbo.Задачи z
                        INNER JOIN dbo.Сотрудники boss
                            ON z.КодПостановщика = boss.КодСотрудника
                        WHERE z.КодИсполнителя = @employeeId
                        ORDER BY z.КодЗадачи DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@employeeId", CurrentUser.EmployeeId);

                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    TasksGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки задач:\n" + ex.Message);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            UpdateTaskStatus("Выполнена");
        }

        private void Failed_Click(object sender, RoutedEventArgs e)
        {
            UpdateTaskStatus("Не выполнена");
        }

        private void UpdateTaskStatus(string newStatus)
        {
            if (!CurrentUser.HasPermission("ИсполнениеСвоихЗадач"))
            {
                MessageBox.Show("У вас нет права изменять задачи.");
                return;
            }

            if (TasksGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите задачу.");
                return;
            }

            DataRowView row = (DataRowView)TasksGrid.SelectedItem;
            int taskId = Convert.ToInt32(row["Код"]);

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        UPDATE dbo.Задачи
                        SET Статус = @status,
                            КомментарийИсполнителя = @comment,
                            ДатаВыполнения = CASE 
                                WHEN @status = N'Выполнена' THEN GETDATE()
                                ELSE NULL
                            END
                        WHERE КодЗадачи = @taskId
                          AND КодИсполнителя = @employeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", newStatus);
                        cmd.Parameters.AddWithValue("@comment", CommentBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        cmd.Parameters.AddWithValue("@employeeId", CurrentUser.EmployeeId);

                        int affected = cmd.ExecuteNonQuery();

                        if (affected > 0)
                        {
                            MessageBox.Show("Статус задачи обновлен.");
                            CommentBox.Clear();
                            LoadTasks();
                        }
                        else
                        {
                            MessageBox.Show("Задача не найдена или не принадлежит вам.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления задачи:\n" + ex.Message);
            }
        }
    }
}