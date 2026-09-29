using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class ServiceOrders
    {
        private readonly ApplicationContext _context;
        private readonly ServiceProducts _serviceProducts;

        public ServiceOrders(ApplicationContext context, ServiceProducts serviceProducts)
        {
            _context = context;
            _serviceProducts = serviceProducts;
        }

        public ApplicationContext GetContext()
        {
            return _context;
        }

        public ObservableCollection<Order> GetAll()
        {
            return _context.Orders;
        }

        public List<Order> GetByWarehouse(BigInteger warehouseId)
        {
            var result = new List<Order>();
            foreach (var o in _context.Orders)
            {
                if (o.WarehouseId == warehouseId)
                    result.Add(o);
            }
            return result;
        }

        public Order? GetById(BigInteger orderId)
        {
            foreach (var o in _context.Orders)
            {
                if (o.OrderId == orderId)
                    return o;
            }
            return null;
        }

        private Warehouse? FindWarehouse(BigInteger warehouseId)
        {
            foreach (var w in _context.Warehouses)
            {
                if (w.WhId == warehouseId)
                    return w;
            }
            return null;
        }

        public (bool success, string error) CreateIncomeOrder(
            BigInteger warehouseId,
            List<OrderItem> items,
            string comment = "")
        {
            if (items == null || items.Count == 0)
                return (false, "Добавьте хотя бы одну позицию в накладную.");

            var wh = FindWarehouse(warehouseId);
            if (wh == null)
                return (false, "Склад не найден.");

            var order = new Order(OrderType.Income, warehouseId, comment);
            order.Warehouse = wh;

            foreach (var item in items)
            {
                if (item.Quantity <= 0)
                    return (false, $"Количество для '{item.Product.Name}' должно быть больше 0.");

                item.Product.StockQuantity += item.Quantity;

                bool found = false;
                foreach (var p in wh.products)
                {
                    if (p.Article == item.Product.Article)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    wh.products.Add(item.Product);

                order.Items.Add(item);
            }

            _context.Orders.Add(order);
            return (true, "");
        }

        public (bool success, string error) CreateOutcomeOrder(
            BigInteger warehouseId,
            List<OrderItem> items,
            string comment = "")
        {
            if (items == null || items.Count == 0)
                return (false, "Добавьте хотя бы одну позицию в накладную.");

            var wh = FindWarehouse(warehouseId);
            if (wh == null)
                return (false, "Склад не найден.");

            foreach (var item in items)
            {
                if (item.Quantity <= 0)
                    return (false, $"Количество для '{item.Product.Name}' должно быть больше 0.");

                if (item.Product.StockQuantity < item.Quantity)
                    return (false, $"Недостаточно товара '{item.Product.Name}'. " +
                        $"Остаток: {item.Product.StockQuantity}, требуется: {item.Quantity}.");
            }

            var order = new Order(OrderType.Outcome, warehouseId, comment);
            order.Warehouse = wh;

            foreach (var item in items)
            {
                item.Product.StockQuantity -= item.Quantity;
                order.Items.Add(item);
            }

            _context.Orders.Add(order);
            return (true, "");
        }

        public void Delete(BigInteger orderId)
        {
            var order = GetById(orderId);
            if (order == null) return;
            _context.Orders.Remove(order);
        }
    }
}