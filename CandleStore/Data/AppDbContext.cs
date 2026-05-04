using Microsoft.EntityFrameworkCore;
using CandleStore.Models;

namespace CandleStore.Data
{
    /// <summary>
    /// Контекст базы данных для работы с сущностями
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Таблица товаров
        /// </summary>
        public DbSet<Product> Products { get; set; }
        
        /// <summary>
        /// Таблица заказов
        /// </summary>
        public DbSet<Order> Orders { get; set; }
        
        /// <summary>
        /// Таблица позиций заказов
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }
        
        /// <summary>
        /// Настройка подключения к базе данных
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Строка подключения к локальной базе SQL Server
            // Если у вас другая версия SQL Server, измените (localdb)
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-GH44F28\SQLEXPRESS;Database=CandleStoreDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        
        /// <summary>
        /// Дополнительные настройки модели (ограничения, индексы и т.д.)
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка ограничений для Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            
            // Настройка ограничений для Order
            modelBuilder.Entity<Order>()
                .Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(100);
            
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);
            
            // Настройка связи Order - OrderItem (один ко многим)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Настройка связи Product - OrderItem (один ко многим)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Настройка точности для UnitPrice
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}