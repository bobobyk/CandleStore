namespace CandleStore.Models
{
    /// <summary>
    /// Класс, представляющий позицию в заказе (связь товара с заказом)
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Уникальный идентификатор позиции
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID заказа (внешний ключ)
        /// </summary>
        public int OrderId { get; set; }
        
        /// <summary>
        /// ID товара (внешний ключ)
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// Количество товара
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Цена за единицу на момент заказа
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Ссылка на заказ (навигационное свойство)
        /// </summary>
        public Order Order { get; set; }
        
        /// <summary>
        /// Ссылка на товар (навигационное свойство)
        /// </summary>
        public Product Product { get; set; }
    }
}