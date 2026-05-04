using System;
using System.Collections.Generic;
using System.Linq;
using CandleStore.Data;
using CandleStore.Models;
using CandleStore.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CandleStore.Tests.Services
{
    [TestFixture]
    public class OrderServiceTests
    {
        private AppDbContext _context;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            // Создаём временную базу данных в памяти (изолированная среда)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _orderService = new OrderService(_context); // нужно будет передать контекст

            // Добавляем тестовые товары
            _context.Products.AddRange(new[]
            {
                new Product { Id = 1, Name = "Test Candle 1", Price = 100, Stock = 10 },
                new Product { Id = 2, Name = "Test Candle 2", Price = 200, Stock = 0 },
                new Product { Id = 3, Name = "Test Candle 3", Price = 300, Stock = 5 }
            });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // ----- ПОЗИТИВНЫЕ ТЕСТЫ -----

        [Test]
        public void CreateOrder_ValidData_ReturnsOrderId()
        {
            // Arrange
            var items = new List<(int productId, int quantity)>
            {
                (1, 2), // 2 шт. товара 1 (цена 100) -> сумма 200
                (3, 1)  // 1 шт. товара 3 (цена 300) -> сумма 300
            }; // итого 500

            // Act
            int orderId = _orderService.CreateOrder("Иван Петров", items);

            // Assert
            Assert.That(orderId, Is.GreaterThan(0));
            var order = _context.Orders.Find(orderId);
            Assert.That(order, Is.Not.Null);
            Assert.That(order.CustomerName, Is.EqualTo("Иван Петров"));
            Assert.That(order.TotalAmount, Is.EqualTo(500));
        }

        [Test]
        public void CreateOrder_StockDecreasesCorrectly()
        {
            // Arrange
            var items = new List<(int productId, int quantity)> { (1, 3) };
            int initialStock = _context.Products.Find(1).Stock; // 10

            // Act
            _orderService.CreateOrder("Тест", items);
            int newStock = _context.Products.Find(1).Stock;

            // Assert
            Assert.That(newStock, Is.EqualTo(initialStock - 3));
        }

        // ----- НЕГАТИВНЫЕ ТЕСТЫ (обработка ошибок) -----

        [Test]
        public void CreateOrder_EmptyCustomerName_ThrowsArgumentException()
        {
            var items = new List<(int productId, int quantity)> { (1, 1) };
            Assert.Throws<ArgumentException>(() => _orderService.CreateOrder("", items));
            Assert.Throws<ArgumentException>(() => _orderService.CreateOrder(null, items));
        }

        [Test]
        public void CreateOrder_EmptyCart_ThrowsArgumentException()
        {
            var emptyItems = new List<(int productId, int quantity)>();
            Assert.Throws<ArgumentException>(() => _orderService.CreateOrder("Покупатель", emptyItems));
        }

        [Test]
        public void CreateOrder_ProductOutOfStock_ThrowsArgumentException()
        {
            // Товар с Id=2 имеет Stock=0
            var items = new List<(int productId, int quantity)> { (2, 1) };
            Assert.Throws<ArgumentException>(() => _orderService.CreateOrder("Покупатель", items));
        }

        [Test]
        public void CreateOrder_MoreThanAvailable_ThrowsArgumentException()
        {
            // Товар с Id=3 имеет Stock=5, запрашиваем 10
            var items = new List<(int productId, int quantity)> { (3, 10) };
            Assert.Throws<ArgumentException>(() => _orderService.CreateOrder("Покупатель", items));
        }

        [Test]
        public void CreateOrder_NegativeQuantity_ThrowsArgumentException()
        {
            var items = new List<(int productId, int quantity)> { (1, -1) };
            Assert.Throws<ArgumentException>(() => _orderService.CreateOrder("Покупатель", items));
        }

        [Test]
        public void CreateOrder_ProductNotFound_ThrowsArgumentException()
        {
            var items = new List<(int productId, int quantity)> { (999, 1) };
            Assert.Throws<ArgumentException>(() => _orderService.CreateOrder("Покупатель", items));
        }
    }
}