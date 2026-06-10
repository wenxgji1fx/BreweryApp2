using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows;
using BreweryApp.Data;
using BreweryApp.Models;

namespace BreweryApp.Views
{
    public partial class AddOrderWindow : Window
    {
        private readonly List<OrderItem> items = new List<OrderItem>();

        public AddOrderWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                List<ClientItem> clients = new List<ClientItem>();

                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT КодКлиента, Название FROM dbo.Клиенты ORDER BY Название";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clients.Add(new ClientItem
                            {
                                КодКлиента = Convert.ToInt32(reader["КодКлиента"]),
                                Название = reader["Название"].ToString() ?? ""
                            });
                        }
                    }
                }

                ClientBox.ItemsSource = clients;
                if (clients.Count > 0)
                    ClientBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки клиентов:\n" + ex.Message);
            }
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            AddOrderItemWindow win = new AddOrderItemWindow();
            win.Owner = this;

            bool? result = win.ShowDialog();
            if (result == true && win.Item != null)
            {
                var orderItem = win.Item as OrderItem ?? throw new InvalidCastException("win.Item не является OrderItem");
                items.Add(orderItem);
                ItemsList.Items.Add(orderItem);
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem == null)
            {
                MessageBox.Show("Выберите позицию для удаления.");
                return;
            }

            OrderItem selectedItem = (OrderItem)ItemsList.SelectedItem;
            items.Remove(selectedItem);
            ItemsList.Items.Remove(selectedItem);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ClientBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента.");
                return;
            }

            if (string.IsNullOrWhiteSpace(StatusBox.Text))
            {
                MessageBox.Show("Введите статус заказа.");
                return;
            }

            if (items.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну позицию в заказ.");
                return;
            }

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string insertOrder = @"
                            INSERT INTO dbo.Заказы (КодКлиента, КодМенеджера, ДатаЗаказа, Статус)
                            OUTPUT INSERTED.КодЗаказа
                            VALUES (@clientId, @managerId, GETDATE(), @status)";

                        int orderId;

                        using (SqlCommand cmd = new SqlCommand(insertOrder, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@clientId", Convert.ToInt32(ClientBox.SelectedValue));
                            cmd.Parameters.AddWithValue("@managerId", CurrentUser.EmployeeId);
                            cmd.Parameters.AddWithValue("@status", StatusBox.Text.Trim());

                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (OrderItem item in items)
                        {
                            string insertItem = @"
                                INSERT INTO dbo.СоставЗаказа
                                (КодЗаказа, КодПартииГотовойПродукции, Количество)
                                VALUES
                                (@orderId, @productId, @quantity)";

                            using (SqlCommand itemCmd = new SqlCommand(insertItem, conn, transaction))
                            {
                                itemCmd.Parameters.AddWithValue("@orderId", orderId);
                                itemCmd.Parameters.AddWithValue("@productId", item.ProductId);
                                itemCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                                itemCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Заказ успешно сохранен.");
                        DialogResult = true;
                        Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения заказа:\n" + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}