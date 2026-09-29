using System.Windows;
using System.Windows.Input;
using Warehouse.Controls;
using Warehouse.Views;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationContext _context;
        private readonly Organization _org;

        public ServiceWhs ServiceWhs { get; private set; }
        public ServiceProducts ServiceProducts { get; private set; }
        public ServiceOrders ServiceOrders { get; private set; }

        private UserControlWarehouses? _whControl;

        public MainWindow(ApplicationContext context, Organization org)
        {
            InitializeComponent();

            _context = context;
            _org = org;

            if (org != null && !string.IsNullOrWhiteSpace(org.OrgName))
            {
                tbStatusOrg.Text = org.OrgName;
                this.Title = org.OrgName + " — Управление складами";
            }

            ServiceWhs = new ServiceWhs(context, org!);
            ServiceProducts = new ServiceProducts(context);
            ServiceOrders = new ServiceOrders(context, ServiceProducts);

            _whControl = new UserControlWarehouses(ServiceWhs, ServiceProducts, ServiceOrders);
            _whControl.WarehouseSelected += OnWarehouseSelected;
            MainFrame.Navigate(_whControl);
        }

        public void OnWarehouseSelected(string whName)
        {
            tbStatusWh.Text = string.IsNullOrEmpty(whName) ? "не выбран" : whName;
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuWarehouses_Click(object sender, RoutedEventArgs e)
        {
            _whControl = new UserControlWarehouses(ServiceWhs, ServiceProducts, ServiceOrders);
            _whControl.WarehouseSelected += OnWarehouseSelected;
            tbStatusWh.Text = "не выбран";
            MainFrame.Navigate(_whControl);
        }

        private void GetProducts_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainFrame.Navigate(new UserControlProducts(ServiceProducts, _context));
        }

        private void GetOrders_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MainFrame.Navigate(new UserControlOrders(ServiceOrders, _context));
        }

        private void MenuAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_whControl == null || _whControl.SelectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад на вкладке складов.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductModWindow dlg = new ProductModWindow(null, 1, ServiceProducts, _context);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {
                var all = ServiceProducts.GetAll();
                if (all.Count > 0)
                {
                    var newProduct = all[all.Count - 1];
                    _whControl.SelectedWarehouse.products.Add(newProduct);
                }
                _whControl.RefreshGrid();
            }
        }

        private void MenuEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_whControl == null)
            {
                MessageBox.Show("Перейдите на вкладку складов.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = _whControl.SelectedProduct;
            if (product == null)
            {
                MessageBox.Show("Выберите товар в таблице.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductModWindow dlg = new ProductModWindow(product, 2, ServiceProducts, _context);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
                _whControl.RefreshGrid();
        }

        private void MenuDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_whControl == null)
            {
                MessageBox.Show("Перейдите на вкладку складов.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = _whControl.SelectedProduct;
            if (product == null)
            {
                MessageBox.Show("Выберите товар в таблице.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProductDeleteWindow dlg = new ProductDeleteWindow(product);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {
                _whControl.SelectedWarehouse?.products.Remove(product);
                ServiceProducts.Delete(product.Article);
                _whControl.RefreshGrid();
            }
        }

        private void MenuAddOrder_Click(object sender, RoutedEventArgs e)
        {
            if (_whControl == null || _whControl.SelectedWarehouse == null)
            {
                MessageBox.Show("Сначала выберите склад на вкладке складов.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OrderWindow dlg = new OrderWindow(_whControl.SelectedWarehouse, ServiceOrders, _context);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
                _whControl.RefreshGrid();
        }

        private void MenuDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UserControlOrders(ServiceOrders, _context));
            MessageBox.Show("Выберите накладную в списке и нажмите 'Удалить накладную'.",
                "Удаление накладной", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}