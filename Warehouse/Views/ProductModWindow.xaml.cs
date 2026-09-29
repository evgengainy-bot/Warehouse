using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse.Views
{
    public partial class ProductModWindow : Window
    {
        private readonly int _mode;
        private readonly Product? _product;
        private readonly ServiceProducts _service;
        private readonly ApplicationContext _context;

        public ProductModWindow(Product? product, int mode,
            ServiceProducts service, ApplicationContext context)
        {
            InitializeComponent();

            _product = product;
            _mode = mode;
            _service = service;
            _context = context;

            this.Title = mode == 1 ? "Новый товар" : "Изменить товар";

            RefreshCombos();

            if (product != null)
            {
                tbName.Text = product.Name;
                cmbUnit.Text = product.Unit;
                tbPrice.Text = product.Price.ToString("N2");
                tbDiscount.Text = product.DiscountPercent.ToString("N2");
                tbDescription.Text = product.Description ?? "";
                tbPhoto.Text = product.PhotoPath ?? "";

                cmbCategory.SelectedItem = product.Category;
                cmbManufacturer.SelectedItem = product.Manufacturer;
                cmbSupplier.SelectedItem = product.Supplier;

                if (!string.IsNullOrEmpty(product.PhotoPath))
                    LoadPhotoPreview(product.PhotoPath);
            }
            else
            {
                cmbUnit.Text = "шт";
                tbPrice.Text = "0";
                tbDiscount.Text = "0";
                if (_context.Categories.Count > 0)
                    cmbCategory.SelectedIndex = 0;
                if (_context.Manufacturers.Count > 0)
                    cmbManufacturer.SelectedIndex = 0;
                if (_context.Suppliers.Count > 0)
                    cmbSupplier.SelectedIndex = 0;
            }

            tbName.Focus();
        }

        private void RefreshCombos()
        {
            var selCat = cmbCategory.SelectedItem as Category;
            var selMan = cmbManufacturer.SelectedItem as Manufacturer;
            var selSup = cmbSupplier.SelectedItem as Supplier;

            cmbCategory.ItemsSource = null;
            cmbCategory.ItemsSource = _context.Categories;

            cmbManufacturer.ItemsSource = null;
            cmbManufacturer.ItemsSource = _context.Manufacturers;

            cmbSupplier.ItemsSource = null;
            cmbSupplier.ItemsSource = _context.Suppliers;

            if (selCat != null) cmbCategory.SelectedItem = selCat;
            if (selMan != null) cmbManufacturer.SelectedItem = selMan;
            if (selSup != null) cmbSupplier.SelectedItem = selSup;
        }

        private void BtnNewCategory_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SimpleInputWindow("Новая категория", "Наименование категории:");
            dlg.Owner = this;
            if (dlg.ShowDialog() == true && !string.IsNullOrWhiteSpace(dlg.InputValue))
            {
                int newId = _context.Categories.Count + 1;
                var cat = new Category(newId, dlg.InputValue.Trim());
                _context.Categories.Add(cat);
                RefreshCombos();
                cmbCategory.SelectedItem = cat;
            }
        }

        private void BtnNewManufacturer_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SimpleInputWindow("Новый производитель", "Наименование производителя:");
            dlg.Owner = this;
            if (dlg.ShowDialog() == true && !string.IsNullOrWhiteSpace(dlg.InputValue))
            {
                int newId = _context.Manufacturers.Count + 1;
                var man = new Manufacturer(newId, dlg.InputValue.Trim());
                _context.Manufacturers.Add(man);
                RefreshCombos();
                cmbManufacturer.SelectedItem = man;
            }
        }

        private void BtnNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SimpleInputWindow("Новый поставщик", "Наименование поставщика:");
            dlg.Owner = this;
            if (dlg.ShowDialog() == true && !string.IsNullOrWhiteSpace(dlg.InputValue))
            {
                int newId = _context.Suppliers.Count + 1;
                var sup = new Supplier(newId, dlg.InputValue.Trim());
                _context.Suppliers.Add(sup);
                RefreshCombos();
                cmbSupplier.SelectedItem = sup;
            }
        }

        private void BtnBrowsePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Выберите фото товара";
            dlg.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*";
            dlg.InitialDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (dlg.ShowDialog() == true)
            {
                tbPhoto.Text = dlg.FileName;
                LoadPhotoPreview(dlg.FileName);
            }
        }

        private void LoadPhotoPreview(string path)
        {
            try
            {
                var bitmap = new BitmapImage(new Uri(path));
                imgPreview.Source = bitmap;
            }
            catch
            {
                imgPreview.Source = null;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string name = tbName.Text.Trim();
            string unit = cmbUnit.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите наименование товара.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show("Укажите единицу измерения.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbUnit.Focus();
                return;
            }

            if (!decimal.TryParse(tbPrice.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbPrice.Focus();
                return;
            }

            if (!decimal.TryParse(tbDiscount.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть от 0 до 100.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbDiscount.Focus();
                return;
            }

            var category = cmbCategory.SelectedItem as Category;
            var manufacturer = cmbManufacturer.SelectedItem as Manufacturer;
            var supplier = cmbSupplier.SelectedItem as Supplier;

            if (category == null)
            {
                MessageBox.Show("Выберите категорию или создайте новую.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (manufacturer == null)
            {
                MessageBox.Show("Выберите производителя или создайте нового.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (supplier == null)
            {
                MessageBox.Show("Выберите поставщика или создайте нового.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_mode == 1)
            {
                var newProduct = new Product
                {
                    Article = "",
                    Name = name,
                    Unit = unit,
                    Price = price,
                    DiscountPercent = discount,
                    StockQuantity = 0, 
                    Description = tbDescription.Text.Trim(),
                    PhotoPath = tbPhoto.Text.Trim(),
                    CategoryId = category.Id,
                    ManufacturerId = manufacturer.Id,
                    SupplierId = supplier.Id,
                    Category = category,
                    Manufacturer = manufacturer,
                    Supplier = supplier
                };
                _service.Add(newProduct);
            }
            else if (_mode == 2 && _product != null)
            {
                var updated = new Product
                {
                    Article = _product.Article,
                    Name = name,
                    Unit = unit,
                    Price = price,
                    DiscountPercent = discount,
                    StockQuantity = _product.StockQuantity, 
                    Description = tbDescription.Text.Trim(),
                    PhotoPath = tbPhoto.Text.Trim(),
                    CategoryId = category.Id,
                    ManufacturerId = manufacturer.Id,
                    SupplierId = supplier.Id,
                    Category = category,
                    Manufacturer = manufacturer,
                    Supplier = supplier
                };
                _service.Update(_product.Article, updated);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}