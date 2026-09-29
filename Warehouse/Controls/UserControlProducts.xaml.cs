using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Warehouse.Views;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse.Controls
{
    public partial class UserControlProducts : UserControl
    {
        private readonly ServiceProducts _service;
        private readonly ApplicationContext _context;

        public UserControlProducts(ServiceProducts service, ApplicationContext context)
        {
            InitializeComponent();

            _service = service;
            _context = context;

            FillCategoryFilter();
            RefreshGrid();
        }

        private void FillCategoryFilter()
        {
            CmbCategory.Items.Clear();
            CmbCategory.Items.Add(new ComboBoxItem { Content = "Все категории", Tag = 0 });

            foreach (var cat in _context.Categories)
                CmbCategory.Items.Add(new ComboBoxItem { Content = cat.Name, Tag = cat.Id });

            CmbCategory.SelectedIndex = 0;
        }

        private void RefreshGrid()
        {
            string search = TxtSearch.Text.Trim().ToLower();
            var products = string.IsNullOrEmpty(search)
                ? new List<Product>(_service.GetAll())
                : _service.Search(TxtSearch.Text.Trim());

            if (CmbCategory.SelectedItem is ComboBoxItem cbi && cbi.Tag is int catId && catId > 0)
                products = products.FindAll(p => p.CategoryId == catId);

            DgProducts.ItemsSource = null;
            DgProducts.ItemsSource = products;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void CmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void DgProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DgProducts.SelectedItem is Product p)
            {
                ProductModWindow dlg = new ProductModWindow(p, 2, _service, _context);
                dlg.Owner = Window.GetWindow(this);
                if (dlg.ShowDialog() == true)
                    RefreshGrid();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductModWindow dlg = new ProductModWindow(null, 1, _service, _context);
            dlg.Owner = Window.GetWindow(this);
            if (dlg.ShowDialog() == true)
                RefreshGrid();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DgProducts.SelectedItem is Product p)
            {
                ProductModWindow dlg = new ProductModWindow(p, 2, _service, _context);
                dlg.Owner = Window.GetWindow(this);
                if (dlg.ShowDialog() == true)
                    RefreshGrid();
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgProducts.SelectedItem is Product p)
            {
                var result = MessageBox.Show(
                    $"Удалить товар \"{p.Name}\"?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _service.Delete(p.Article);
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}