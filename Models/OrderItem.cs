using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreweryApp.Models
{
    public class OrderItem
    {
        public required string Название { get; set; }
        public required string ФИО { get; set; }
        public string Должность { get; set; } = string.Empty;
        public required string Display { get; set; }
        public required string Item { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;

        public OrderItem()
        {
            Название = string.Empty;
            ФИО = string.Empty;
            Должность = string.Empty;
            Display = string.Empty;
            Item = string.Empty;
        }

        public OrderItem(string название, string фио, string должность, string display, string item, int productId, int quantity = 1)
        {
            Название = название;
            ФИО = фио;
            Должность = должность;
            Display = display;
            Item = item;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
