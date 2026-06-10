using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class ManagerTasksPage : Page
    {
        public ManagerTasksPage()
        {
            InitializeComponent();

            if (!CurrentUser.HasPermission("ПостановкаЗадач"))
            {
                MessageBox.Show("У вас нет доступа к постановке задач.");
                return;
            }

            LoadEmployees();
            LoadTasks();
        }

        private void LoadEmployees()
        {
            List<EmployeeItem> list = new List<EmployeeItem>();

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT КодСотрудника, ФИО, Должность
                    FROM dbo.Сотрудники
                    WHERE КодРуководителя = @bossId
                      AND Активен = 1
                    ORDER BY ФИО";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@bossId", CurrentUser.EmployeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new EmployeeItem
                            {
                                КодСотрудника = Convert.ToInt32(reader["КодСотрудника"]),
                                ФИО = reader["ФИО"].ToString() ?? "",
                                Должность = reader["Должность"].ToString() ?? ""
                            });
                        }
                    }
                }
            }

            EmployeeBox.ItemsSource = list;

            if (list.Count > 0)
                EmployeeBox.SelectedIndex = 0;
        }

        private void LoadTasks()
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                string query = @"
                    SELECT 
                        z.КодЗадачи AS [Код],
                        z.Заголовок AS [Заголовок],
                        z.Описание AS [Описание],
                        z.ТипЗадачи AS [Тип],
                        z.Статус AS [Статус],
                        s.ФИО AS [Исполнитель],
                        z.ДатаСоздания AS [Дата создания],
                        z.Дедлайн AS [Дедлайн],
                        z.КомментарийИсполнителя AS [Комментарий исполнителя]
                    FROM dbo.Задачи z
                    INNER JOIN dbo.Сотрудники s ON z.КодИсполнителя = s.КодСотрудника
                    WHERE z.КодПостановщика = @bossId
                    ORDER BY z.КодЗадачи DESC";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@bossId", CurrentUser.EmployeeId);

                DataTable table = new DataTable();
                adapter.Fill(table);

                TasksGrid.ItemsSource = table.DefaultView;
            }
        }

        private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите исполнителя.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TitleBox.Text) || string.IsNullOrWhiteSpace(TaskTypeBox.Text))
            {
                MessageBox.Show("Введите заголовок и тип задачи.");
                return;
            }

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                string query = @"
                    INSERT INTO dbo.Задачи
                    (
                        Заголовок,
                        Описание,
                        КодПостановщика,
                        КодИсполнителя,
                        Подразделение,
                        ТипЗадачи,
                        Статус,
                        Дедлайн
                    )
                    VALUES
                    (
                        @title,
                        @description,
                        @bossId,
                        @employeeId,
                        @department,
                        @taskType,
                        N'Новая',
                        @deadline
                    )";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", TitleBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@description", DescriptionBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@bossId", CurrentUser.EmployeeId);
                    cmd.Parameters.AddWithValue("@employeeId", Convert.ToInt32(EmployeeBox.SelectedValue));
                    cmd.Parameters.AddWithValue("@department", CurrentUser.Department);
                    cmd.Parameters.AddWithValue("@taskType", TaskTypeBox.Text.Trim());

                    if (DeadlinePicker.SelectedDate.HasValue)
                        cmd.Parameters.AddWithValue("@deadline", DeadlinePicker.SelectedDate.Value);
                    else
                        cmd.Parameters.AddWithValue("@deadline", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Задача создана.");
            TitleBox.Clear();
            DescriptionBox.Clear();
            TaskTypeBox.Clear();
            DeadlinePicker.SelectedDate = null;

            LoadTasks();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
            LoadTasks();
        }
    }
}