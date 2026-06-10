using BreweryApp.Data;
using BreweryApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Microsoft.Data.SqlClient;
namespace BreweryApp.Views
{
    public partial class AddOrderItemWindow : Window
    {
        public object Item { get; set; } = new object();

        public AddOrderItemWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                List<ProductItem> products = new List<ProductItem>();

                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            КодПартииГотовойПродукции,
                            Количество,
                            СрокГодности,
                            Статус
                        FROM dbo.ПартииГотовойПродукции
                        ORDER BY КодПартииГотовойПродукции DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["КодПартииГотовойПродукции"]);
                            decimal qty = Convert.ToDecimal(reader["Количество"]);
                            DateTime expiry = Convert.ToDateTime(reader["СрокГодности"]);
                            string status = reader["Статус"].ToString();

                            products.Add(new ProductItem
                            {
                                КодПартииГотовойПродукции = id,
                                Display = $"Партия {id} | Кол-во: {qty} | Срок: {expiry:dd.MM.yyyy} | Статус: {status}"
                            });
                        }
                    }
                }

                ProductBox.ItemsSource = products;

                if (products.Count > 0)
                    ProductBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки партий:\n" + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (ProductBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите партию.");
                return;
            }

            if (string.IsNullOrWhiteSpace(QuantityBox.Text))
            {
                MessageBox.Show("Введите количество.");
                return;
            }

            if (!decimal.TryParse(
                QuantityBox.Text.Replace(',', '.'),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal quantity))
            {
                MessageBox.Show("Количество введено неверно.");
                return;
            }

            if (quantity <= 0)
            {
                MessageBox.Show("Количество должно быть больше 0.");
                return;
            }

            Item = new OrderItem
            {
                ProductId = Convert.ToInt32(ProductBox.SelectedValue),
                Quantity = Convert.ToInt32(quantity),   // если quantity decimal
                Название = ProductBox.Text,            // название продукта
                ФИО = CurrentUser.FIO,                 // имя текущего пользователя
                Должность = CurrentUser.Role,          // должность текущего пользователя
                Display = ProductBox.Text,             // отображаемое имя позиции
                Item = ProductBox.Text                  // уникальный идентификатор / имя позиции
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}