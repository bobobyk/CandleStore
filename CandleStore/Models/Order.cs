using System;
using System.Collections.Generic;

namespace CandleStore.Models
{
    /// <summary>
    /// Класс, представляющий заказ
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Уникальный номер заказа
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Имя покупателя
        /// </summary>
        public string CustomerName { get; set; }
        
        /// <summary>
        /// Дата и время оформления заказа
        /// </summary>
        public DateTime OrderDate { get; set; }
        
        /// <summary>
        /// Общая сумма заказа
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// Список позиций в заказе
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; }
        
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Order()
        {
            OrderDate = DateTime.Now;
            OrderItems = new List<OrderItem>();
        }
    }
}