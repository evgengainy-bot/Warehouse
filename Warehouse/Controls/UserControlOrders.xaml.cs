using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;

using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse.Controls
{
    public partial class UserControlOrders : UserControl
    {
        private readonly ServiceOrders _service;
        private readonly ApplicationContext _context;

        public UserControlOrders(ServiceOrders service, ApplicationContext context)
        {
            InitializeComponent();

            _service = service;
            _context = context;

            CmbWarehouse.Items.Add("Все склады");
            foreach (var wh in _context.Warehouses)
                CmbWarehouse.Items.Add(wh);

            CmbWarehouse.SelectedIndex = 0;

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            List<Order> orders;

            if (CmbWarehouse.SelectedItem is WhModel wh)
                orders = _service.GetByWarehouse(wh.WhId);
            else
                orders = new List<Order>(_service.GetAll());

            DgOrders.ItemsSource = null;
            DgOrders.ItemsSource = orders;
            DgOrderItems.ItemsSource = null;
        }

        private void CmbWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshGrid();
        }

        private void DgOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgOrders.SelectedItem is Order order)
                DgOrderItems.ItemsSource = order.Items;
            else
                DgOrderItems.ItemsSource = null;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgOrders.SelectedItem is Order order)
            {
                var result = MessageBox.Show(
                    $"Удалить накладную №{order.OrderId}?\n\nВнимание: остатки товаров не будут восстановлены.",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _service.Delete(order.OrderId);
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show("Выберите накладную для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}