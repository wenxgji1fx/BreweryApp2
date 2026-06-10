using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;

namespace BreweryApp.Views
{
    public partial class BrewerPanel : UserControl
    {
        private bool isBrewRunning = false;
        private int currentBrewId = 0;

        public BrewerPanel()
        {
            InitializeComponent();
            LoadBrews();
        }

        private void LoadBrews()
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT КодВарки, ДатаВарки, Объем,
                               CASE WHEN Статус IS NULL THEN 'Не начата' ELSE Статус END AS Статус
                        FROM Варки
                        ORDER BY ДатаВарки";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    BrewDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки варок: " + ex.Message);
            }
        }

        private void StartBrew_Click(object sender, RoutedEventArgs e)
        {
            if (BrewDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите варку.");
                return;
            }

            DataRowView row = (DataRowView)BrewDataGrid.SelectedItem;
            currentBrewId = Convert.ToInt32(row["КодВарки"]);

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Варки SET Статус = @status WHERE КодВарки = @id", conn);
                    cmd.Parameters.AddWithValue("@status", "Начата");
                    cmd.Parameters.AddWithValue("@id", currentBrewId);
                    cmd.ExecuteNonQuery();
                }

                isBrewRunning = true;
                StatusText.Text = $"Варка {row["КодВарки"]} начата";
                StartBrewButton.IsEnabled = false;
                FinishBrewButton.IsEnabled = true;

                LoadBrews();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления статуса: " + ex.Message);
            }
        }

        private void FinishBrew_Click(object sender, RoutedEventArgs e)
        {
            if (!isBrewRunning || BrewDataGrid.SelectedItem == null)
                return;

            DataRowView row = (DataRowView)BrewDataGrid.SelectedItem;

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Варки SET Статус = @status WHERE КодВарки = @id", conn);
                    cmd.Parameters.AddWithValue("@status", "Завершена");
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(row["КодВарки"]));
                    cmd.ExecuteNonQuery();
                }

                StatusText.Text = $"Варка {row["КодВарки"]} завершена";
                isBrewRunning = false;
                StartBrewButton.IsEnabled = true;
                FinishBrewButton.IsEnabled = false;

                LoadBrews();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления статуса: " + ex.Message);
            }
        }

        private void ResetBrew_Click(object sender, RoutedEventArgs e)
        {
            if (BrewDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите варку.");
                return;
            }

            DataRowView row = (DataRowView)BrewDataGrid.SelectedItem;
            int id = Convert.ToInt32(row["КодВарки"]);

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Варки SET Статус = 'Не начата' WHERE КодВарки = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                StatusText.Text = $"Статус варки {row["КодВарки"]} сброшен на 'Не начата'";
                LoadBrews();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сброса статуса: " + ex.Message);
            }
        }
    }
}