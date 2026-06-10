using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using BreweryApp.Data;

namespace BreweryApp.Views
{
    public partial class LabPanel : UserControl
    {
        private bool isCheckRunning = false;
        private int currentControlId = 0;

        public LabPanel()
        {
            InitializeComponent();
            LoadQualityItems();
        }

        private void LoadQualityItems()
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT ck.КодКонтроля,
                               CONCAT('Партия ', p.КодПартииГотовойПродукции, ' (', ck.Дата, ')') AS Партия,
                               ck.ТипКонтроля, ck.Дата,
                               CASE WHEN ck.СтатусКачества IS NULL THEN 'Не проверена' ELSE ck.СтатусКачества END AS Статус
                        FROM КонтрольКачества ck
                        INNER JOIN ПартииГотовойПродукции p ON ck.КодОбъекта = p.КодПартииГотовойПродукции
                        ORDER BY ck.Дата";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    QualityDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void StartCheck_Click(object sender, RoutedEventArgs e)
        {
            if (QualityDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите партию.");
                return;
            }

            DataRowView row = (DataRowView)QualityDataGrid.SelectedItem;
            currentControlId = Convert.ToInt32(row["КодКонтроля"]);
            isCheckRunning = true;
            StatusText.Text = $"Проверка {row["Партия"]} начата";
            StartCheckButton.IsEnabled = false;
            FinishCheckButton.IsEnabled = true;
        }

        private void FinishCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!isCheckRunning || QualityDataGrid.SelectedItem == null)
                return;

            DataRowView row = (DataRowView)QualityDataGrid.SelectedItem;

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE КонтрольКачества SET СтатусКачества = @status WHERE КодКонтроля = @id", conn);
                    cmd.Parameters.AddWithValue("@status", "Проверена");
                    cmd.Parameters.AddWithValue("@id", currentControlId);
                    int affected = cmd.ExecuteNonQuery();

                    if (affected == 0)
                    {
                        MessageBox.Show("Не удалось обновить статус. Проверьте выбранную запись.");
                        return;
                    }
                }

                LoadQualityItems(); // обновляем DataGrid
                StatusText.Text = $"Проверка {row["Партия"]} завершена";

                isCheckRunning = false;
                StartCheckButton.IsEnabled = true;
                FinishCheckButton.IsEnabled = false;
                currentControlId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка обновления статуса: " + ex.Message);
            }
        }

        private void ResetCheck_Click(object sender, RoutedEventArgs e)
        {
            if (QualityDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите партию.");
                return;
            }

            DataRowView row = (DataRowView)QualityDataGrid.SelectedItem;
            int id = Convert.ToInt32(row["КодКонтроля"]);

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE КонтрольКачества SET СтатусКачества = 'Не проверена' WHERE КодКонтроля = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                LoadQualityItems(); // обновляем DataGrid
                StatusText.Text = $"Статус {row["Партия"]} сброшен на 'Не проверена'";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сброса статуса: " + ex.Message);
            }
        }
    }
}