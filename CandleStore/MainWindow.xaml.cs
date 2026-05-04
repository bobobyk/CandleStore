using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CandleStore.Models;
using CandleStore.Services;

namespace CandleStore
{
    public partial class MainWindow : Window
    {
        private readonly OrderService _orderService;
        private List<Product> _products;
        private Dictionary<int, int> _cart;
        
        public MainWindow()
        {
            InitializeComponent();
            _orderService = new OrderService();
            _cart = new Dictionary<int, int>();
            LoadProducts();
            UpdateCartDisplay();
        }
        
        private void LoadProducts()
        {
            try
            {
                _products = _orderService.GetAllProducts();
                ProductsList.ItemsSource = _products;
                TxtStatus.Text = $"Загружено товаров: {_products.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }
        
        private void ProductsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product selectedProduct)
            {
                TxtName.Text = selectedProduct.Name;
                TxtScent.Text = selectedProduct.Scent;
                TxtPrice.Text = $"{selectedProduct.Price:F2} руб.";
                TxtStock.Text = selectedProduct.Stock.ToString();
                TxtQuantity.Text = "1";
            }
        }
        
        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TxtQuantity.Text, out int qty) && qty > 1)
                TxtQuantity.Text = (qty - 1).ToString();
        }
        
        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is Product product)
            {
                if (int.TryParse(TxtQuantity.Text, out int qty) && qty < product.Stock)
                    TxtQuantity.Text = (qty + 1).ToString();
                else
                    MessageBox.Show($"Максимум {product.Stock} шт.");
            }
        }
        
        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsList.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар");
                return;
            }
            
            if (!int.TryParse(TxtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }
            
            if (qty > product.Stock)
            {
                MessageBox.Show($"Доступно только {product.Stock} шт.");
                return;
            }
            
            if (_cart.ContainsKey(product.Id))
                _cart[product.Id] += qty;
            else
                _cart.Add(product.Id, qty);
            
            UpdateCartDisplay();
            TxtStatus.Text = $"Добавлено: {product.Name} x{qty}";
        }
        
        private void UpdateCartDisplay()
        {
            var items = new List<string>();
            decimal total = 0;
            
            foreach (var item in _cart)
            {
                var product = _products.First(p => p.Id == item.Key);
                decimal sum = product.Price * item.Value;
                items.Add($"{product.Name} x{item.Value} = {sum:F2} руб.");
                total += sum;
            }
            
            CartList.ItemsSource = items;
            TxtTotal.Text = $"{total:F2} руб.";  // ← КЛЮЧЕВАЯ СТРОКА!
        }
        
        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }
            
            var customerName = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите ваше имя:", "Оформление заказа", "Покупатель");
            
            if (string.IsNullOrWhiteSpace(customerName))
            {
                MessageBox.Show("Имя обязательно");
                return;
            }
            
            try
            {
                var items = _cart.Select(kvp => (kvp.Key, kvp.Value)).ToList();
                int orderId = _orderService.CreateOrder(customerName, items);
                
                MessageBox.Show($"Заказ №{orderId} оформлен!\nСпасибо, {customerName}!");
                
                _cart.Clear();
                UpdateCartDisplay();  // ← ОБНОВЛЯЕМ ПОСЛЕ ОЧИСТКИ
                LoadProducts();
                
                TxtName.Text = TxtScent.Text = TxtPrice.Text = TxtStock.Text = "";
                TxtQuantity.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}