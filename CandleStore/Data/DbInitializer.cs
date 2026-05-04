using System.Linq;
using CandleStore.Models;

namespace CandleStore.Data
{
    /// <summary>
    /// Класс для инициализации базы данных тестовыми данными
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Заполнение базы данных начальными данными
        /// </summary>
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();
            
            // Проверяем, есть ли уже данные
            if (context.Products.Any())
            {
                return; // База уже заполнена
            }
            
            // Добавляем тестовые товары
            var products = new Product[]
            {
                new Product { Name = "Рождественская свеча", Scent = "Корица и апельсин", Price = 450, Stock = 25 },
                new Product { Name = "Лавандовая свеча", Scent = "Лаванда", Price = 390, Stock = 30 },
                new Product { Name = "Ванильная свеча", Scent = "Ваниль", Price = 350, Stock = 40 },
                new Product { Name = "Морской бриз", Scent = "Океан и озон", Price = 420, Stock = 15 },
                new Product { Name = "Сосновый лес", Scent = "Хвоя и кедр", Price = 480, Stock = 20 },
                new Product { Name = "Кофейная свеча", Scent = "Кофе с корицей", Price = 380, Stock = 35 }
            };
            
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}