using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.Views;
using WarehouseData.Models;
using WarehouseData.Services;

using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse.Controls
{
    public partial class UserControlWarehouses : UserControl
    {
        public event Action<string>? WarehouseSelected;

        private readonly ServiceWhs _serviceWhs;
        private readonly ServiceProducts _serviceProducts;
        private readonly ServiceOrders _serviceOrders;
        private WhModel? _selectedWarehouse;
      
        public WhModel? SelectedWarehouse => _selectedWarehouse;
        public Product? SelectedProduct => DgProducts.SelectedItem as Product;

        public void RefreshGrid()
        {
            RefreshProductsGrid();
        }
        public UserControlWarehouses(ServiceWhs serviceWhs,
            ServiceProducts serviceProducts,
            ServiceOrders serviceOrders)
        {
            InitializeComponent();
            _serviceWhs = serviceWhs;
            _serviceProducts = serviceProducts;
            _serviceOrders = serviceOrders;

            FillWarehousesList();
            FillSuppliersFilter();
            lbxWhList.Focus();
        }

        private void FillWarehousesList()
        {
            int idx = lbxWhList.SelectedIndex;
            if (idx < 0) idx = 0;

            lbxWhList.ItemsSource = null;
            lbxWhList.Items.Clear();
            lbxWhList.ItemsSource = _serviceWhs.GetByOrg();

            if (lbxWhList.Items.Count > 0)
            {
                try { lbxWhList.SelectedIndex = idx; }
                catch { lbxWhList.SelectedIndex = 0; }
            }
        }

        private void FillSuppliersFilter()
        {
            int selectedTag = 0;
            if (CmbSupplier.SelectedItem is ComboBoxItem current && current.Tag is int t)
                selectedTag = t;

            CmbSupplier.Items.Clear();
            CmbSupplier.Items.Add(new ComboBoxItem { Content = "Все поставщики", Tag = 0 });

            foreach (var sup in _serviceWhs.GetContext().Suppliers)
            {
                CmbSupplier.Items.Add(new ComboBoxItem { Content = sup.Name, Tag = sup.Id });
            }

            foreach (ComboBoxItem item in CmbSupplier.Items)
            {
                if (item.Tag is int tag && tag == selectedTag)
                {
                    CmbSupplier.SelectedItem = item;
                    return;
                }
            }

            CmbSupplier.SelectedIndex = 0;
        }

        private void lbxWhList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedWarehouse = lbxWhList.SelectedItem as WhModel;

            if (_selectedWarehouse != null)
            {
                lblWhName.Content = $"Склад: {_selectedWarehouse.WhName}  |  Адрес: {_selectedWarehouse.WhAddress}";
                WarehouseSelected?.Invoke(_selectedWarehouse.WhName);
               
                FillSuppliersFilter();
                RefreshProductsGrid();
            }
            else
            {
                lblWhName.Content = "Выберите склад";
                WarehouseSelected?.Invoke("");
                DgProducts.ItemsSource = null;
            }
        }

        private void RefreshProductsGrid()
        {
            if (_selectedWarehouse == null)
            {
                DgProducts.ItemsSource = null;
                return;
            }

            var products = new List<Product>(_selectedWarehouse.products);

            string search = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                products = products.FindAll(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Article.ToLower().Contains(search));
            }

            if (CmbSupplier.SelectedItem is ComboBoxItem cbi && cbi.Tag is int suppId && suppId > 0)
            {
                products = products.FindAll(p => p.SupplierId == suppId);
            }

            if (CmbSort.SelectedItem is ComboBoxItem sortItem)
            {
                string sort = sortItem.Content?.ToString() ?? "";
                if (sort == "Остаток ↑")
                    products.Sort((a, b) => a.StockQuantity.CompareTo(b.StockQuantity));
                else if (sort == "Остаток ↓")
                    products.Sort((a, b) => b.StockQuantity.CompareTo(a.StockQuantity));
                else if (sort == "Цена ↑")
                    products.Sort((a, b) => a.Price.CompareTo(b.Price));
                else if (sort == "Цена ↓")
                    products.Sort((a, b) => b.Price.CompareTo(a.Price));
            }

            DgProducts.ItemsSource = null;
            DgProducts.ItemsSource = products;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedWarehouse == null) return;
            RefreshProductsGrid();
        }

        private void CmbSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedWarehouse == null) return;
            RefreshProductsGrid();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedWarehouse == null) return;
            RefreshProductsGrid();
        }

        private void DgProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgProducts.SelectedItem is Product product)
            {
                ProductModWindow dlg = new ProductModWindow(product, 2,
                    _serviceProducts, _serviceWhs.GetContext());
                dlg.Owner = Window.GetWindow(this);
                if (dlg.ShowDialog() == true)
                    RefreshProductsGrid();
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductModWindow dlg = new ProductModWindow(null, 1,
                _serviceProducts, _serviceWhs.GetContext());
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() == true)
            {
                var all = _serviceProducts.GetAll();
                if (all.Count > 0)
                {
                    var newProduct = all[all.Count - 1];
                    _selectedWarehouse.products.Add(newProduct);
                }
                RefreshProductsGrid();
            }
        }

        private void BtnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgProducts.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар для редактирования.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductModWindow dlg = new ProductModWindow(product, 2,
                _serviceProducts, _serviceWhs.GetContext());
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() == true)
                RefreshProductsGrid();
        }

        private void BtnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DgProducts.SelectedItem is not Product product)
            {
                MessageBox.Show("Выберите товар для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductDeleteWindow dlg = new ProductDeleteWindow(product);
            dlg.Owner = Window.GetWindow(this);

            if (dlg.ShowDialog() == true)
            {
                _selectedWarehouse.products.Remove(product);
                _serviceProducts.Delete(product.Article);
                RefreshProductsGrid();
            }
        }

        private void BtnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OrderWindow dlg = new OrderWindow(_selectedWarehouse,
                _serviceOrders, _serviceWhs.GetContext());
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() == true)
                RefreshProductsGrid();
        }

        private void WhAdd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WhModWindow dlg = new WhModWindow("Новый склад", "", 1, null, _serviceWhs);
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() == true)
                FillWarehousesList();
        }

        private void WhEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Выберите склад для редактирования.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            WhModWindow dlg = new WhModWindow("Изменить склад",
                _selectedWarehouse.WhAddress, 2, _selectedWarehouse, _serviceWhs);
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() == true)
                FillWarehousesList();
        }

        private void WhDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_selectedWarehouse == null)
            {
                MessageBox.Show("Выберите склад для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            WhDeleteWindow dlg = new WhDeleteWindow(_selectedWarehouse);
            dlg.Owner = Window.GetWindow(this);

            if (dlg.ShowDialog() == true)
            {
                _serviceWhs.Delete(_selectedWarehouse.WhId);
                _selectedWarehouse = null;
                DgProducts.ItemsSource = null;
                lblWhName.Content = "Выберите склад";
                WarehouseSelected?.Invoke("");
                FillWarehousesList();
            }
        }
    }
}