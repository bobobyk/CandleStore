using System;
using System.Collections.Generic;
using System.Linq;
using CandleStore.Data;
using CandleStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CandleStore.Services
{
    /// <summary>
    /// Сервис для работы с заказами
    /// </summary>
    public class OrderService
    {
        /// <summary>
        /// Создание нового заказа
        /// </summary>
        /// <param name="customerName">Имя покупателя</param>
        /// <param name="items">Список товаров (ID, количество)</param>
        /// <returns>ID созданного заказа</returns>
        /// <exception cref="ArgumentException">Выбрасывается при некорректных данных</exception>
        public int CreateOrder(string customerName, List<(int productId, int quantity)> items)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Имя покупателя не может быть пустым");
            
            if (items == null || items.Count == 0)
                throw new ArgumentException("Заказ не может быть пустым");
            
            using var db = new AppDbContext();
            
            // Проверка наличия товаров на складе
            foreach (var item in items)
            {
                var product = db.Products.Find(item.productId);
                if (product == null)
                    throw new ArgumentException($"Товар с ID {item.productId} не найден");
                
                if (product.Stock < item.quantity)
                    throw new ArgumentException($"Недостаточно товара '{product.Name}'. В наличии: {product.Stock}");
                
                if (item.quantity <= 0)
                    throw new ArgumentException("Количество товара должно быть положительным");
            }
            
            // Создание заказа
            var order = new Order
            {
                CustomerName = customerName,
                OrderDate = DateTime.Now
            };
            
            db.Orders.Add(order);
            db.SaveChanges();
            
            decimal totalAmount = 0;
            
            // Добавление позиций заказа
            foreach (var item in items)
            {
                var product = db.Products.Find(item.productId);
                
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.productId,
                    Quantity = item.quantity,
                    UnitPrice = product.Price
                };
                
                db.OrderItems.Add(orderItem);
                totalAmount += product.Price * item.quantity;
                
                // Уменьшение остатков на складе
                product.Stock -= item.quantity;
            }
            
            // Обновление общей суммы заказа
            order.TotalAmount = totalAmount;
            db.SaveChanges();
            
            return order.Id;
        }
        
        /// <summary>
        /// Получение всех товаров
        /// </summary>
        public List<Product> GetAllProducts()
        {
            using var db = new AppDbContext();
            return db.Products.ToList();
        }
        
        /// <summary>
        /// Получение товара по ID
        /// </summary>
        public Product GetProductById(int id)
        {
            using var db = new AppDbContext();
            return db.Products.Find(id);
        }
        
        /// <summary>
        /// Получение всех заказов с деталями
        /// </summary>
        public List<Order> GetAllOrders()
        {
            using var db = new AppDbContext();
            return db.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToList();
        }
        
        /// <summary>
        /// Добавление нового товара
        /// </summary>
        public void AddProduct(string name, string scent, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название товара не может быть пустым");
            
            if (price <= 0)
                throw new ArgumentException("Цена должна быть больше 0");
            
            if (stock < 0)
                throw new ArgumentException("Остаток не может быть отрицательным");
            
            using var db = new AppDbContext();
            var product = new Product
            {
                Name = name,
                Scent = scent,
                Price = price,
                Stock = stock,
                CreatedAt = DateTime.Now
            };
            
            db.Products.Add(product);
            db.SaveChanges();
        }
    }
}