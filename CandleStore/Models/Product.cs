using System;
using System.Collections.Generic;

namespace CandleStore.Models
{
    /// <summary>
    /// Класс, представляющий товар (ароматическую свечу)
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Уникальный идентификатор товара
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Название свечи (например, "Рождественская", "Лавандовая")
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Аромат свечи (ваниль, корица, роза и т.д.)
        /// </summary>
        public string Scent { get; set; }
        
        /// <summary>
        /// Цена в рублях
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Количество на складе
        /// </summary>
        public int Stock { get; set; }
        
        /// <summary>
        /// Дата добавления товара в каталог
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Навигационное свойство для связи с позициями заказов
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; set; }
        
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Product()
        {
            CreatedAt = DateTime.Now;
            OrderItems = new List<OrderItem>();
        }
        
        /// <summary>
        /// Переопределение ToString для отображения в списке
        /// </summary>
        public override string ToString()
        {
            return $"{Name} - {Scent} - {Price} руб. (Остаток: {Stock})";
        }
    }
}