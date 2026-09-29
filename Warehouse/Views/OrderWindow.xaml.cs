using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse.Views
{
    public partial class OrderWindow : Window
    {
        private readonly WhModel _warehouse;
        private readonly ServiceOrders _serviceOrders;
        private readonly ServiceProducts _serviceProducts;
        private readonly ApplicationContext _context;

        private readonly List<OrderItem> _items = new List<OrderItem>();

        public OrderWindow(WhModel warehouse, ServiceOrders serviceOrders,
            ApplicationContext context)
        {
            InitializeComponent();

            _warehouse = warehouse;
            _serviceOrders = serviceOrders;
            _serviceProducts = serviceOrders.GetContext() != null
                ? new ServiceProducts(context)
                : new ServiceProducts(context);
            _context = context;

            tbWarehouse.Text = $"{warehouse.WhName}  |  {warehouse.WhAddress}";

            RefreshProductCombo();
            RefreshItemsGrid();
        }

        private void RefreshProductCombo()
        {
            var selected = cmbProduct.SelectedItem as Product;

            cmbProduct.ItemsSource = null;
            cmbProduct.Items.Clear();
            cmbProduct.ItemsSource = _context.Products;

            if (selected != null)
            {
                foreach (var p in _context.Products)
                {
                    if (p.Article == selected.Article)
                    {
                        cmbProduct.SelectedItem = p;
                        break;
                    }
                }
            }
            else if (_context.Products.Count > 0)
            {
                cmbProduct.SelectedIndex = 0;
            }
        }


        private void cmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProduct.SelectedItem is Product p)
                tbCurrentStock.Text = p.StockQuantity.ToString();
            else
                tbCurrentStock.Text = "";
        }

        private void BtnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            var svcProducts = new ServiceProducts(_context);

            ProductModWindow dlg = new ProductModWindow(null, 1, svcProducts, _context);
            dlg.Owner = this;

            if (dlg.ShowDialog() == true)
            {
                RefreshProductCombo();

                if (_context.Products.Count > 0)
                {
                    cmbProduct.SelectedItem = _context.Products[_context.Products.Count - 1];
                    
                    if (cmbProduct.SelectedItem is Product p)
                        tbCurrentStock.Text = p.StockQuantity.ToString();
                }
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            lblError.Text = "";

            if (cmbProduct.SelectedItem is not Product product)
            {
                lblError.Text = "Выберите товар или создайте новый.";
                return;
            }

            if (!int.TryParse(tbQuantity.Text, out int qty) || qty <= 0)
            {
                lblError.Text = "Введите количество — целое число больше 0.";
                tbQuantity.Focus();
                return;
            }

            bool found = false;
            foreach (var existing in _items)
            {
                if (existing.Product.Article == product.Article)
                {
                    existing.Quantity += qty;
                    found = true;
                    break;
                }
            }

            if (!found)
                _items.Add(new OrderItem(product, qty));

            RefreshItemsGrid();
            tbQuantity.Text = "1";
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (DgItems.SelectedItem is OrderItem item)
            {
                _items.Remove(item);
                RefreshItemsGrid();
            }
            else
            {
                lblError.Text = "Выберите позицию для удаления.";
            }
        }

        private void RefreshItemsGrid()
        {
            DgItems.ItemsSource = null;
            DgItems.ItemsSource = _items;

            decimal total = 0;
            foreach (var item in _items)
                total += item.PriceAtMoment * item.Quantity;

            tbTotal.Text = $"{total:N2} руб.";
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            lblError.Text = "";

            if (_items.Count == 0)
            {
                lblError.Text = "Добавьте хотя бы одну позицию.";
                return;
            }

            bool isIncome = cmbOrderType.SelectedIndex == 0;

            (bool success, string error) result;

            if (isIncome)
                result = _serviceOrders.CreateIncomeOrder(
                    _warehouse.WhId, _items, tbComment.Text.Trim());
            else
                result = _serviceOrders.CreateOutcomeOrder(
                    _warehouse.WhId, _items, tbComment.Text.Trim());

            if (result.success)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                lblError.Text = result.error;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}