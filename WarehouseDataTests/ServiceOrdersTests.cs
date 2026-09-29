using System.Collections.Generic;
using System.Numerics;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;
using Xunit;

namespace WarehouseDataTests
{
    public class ServiceOrdersTests
    {
        private (ApplicationContext ctx, ServiceOrders svcOrders, ServiceProducts svcProducts) CreateSvc()
        {
            Organization.OrgCounter = BigInteger.Zero;
            Warehouse.WarehousesCounter = BigInteger.Zero;
            Order.OrderCounter = BigInteger.Zero;

            var ctx = new ApplicationContext();

            var cat = new Category(1, "Тест категория");
            var man = new Manufacturer(1, "Тест производитель");
            var sup = new Supplier(1, "Тест поставщик");
            ctx.Categories.Add(cat);
            ctx.Manufacturers.Add(man);
            ctx.Suppliers.Add(sup);

            var testProduct = new Product
            {
                Article = "TST-001",
                Name = "Тестовый товар",
                Unit = "шт",
                Price = 100m,
                StockQuantity = 10,
                CategoryId = cat.Id,
                ManufacturerId = man.Id,
                SupplierId = sup.Id,
                Category = cat,
                Manufacturer = man,
                Supplier = sup
            };
            ctx.Products.Add(testProduct);

            var svcProducts = new ServiceProducts(ctx);
            var svcOrders = new ServiceOrders(ctx, svcProducts);
            return (ctx, svcOrders, svcProducts);
        }

        private List<OrderItem> MakeItems(Product product, int qty)
        {
            return new List<OrderItem>
            {
                new OrderItem(product, qty)
            };
        }

        [Fact]
        public void CreateIncomeOrder_IncreasesStock()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];
            int stockBefore = product.StockQuantity;

            var result = svcOrders.CreateIncomeOrder(
                wh.WhId, MakeItems(product, 5));

            Assert.True(result.success);
            Assert.Equal(stockBefore + 5, product.StockQuantity);
        }

        [Fact]
        public void CreateIncomeOrder_CreatesOrderRecord()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];
            int countBefore = svcOrders.GetAll().Count;

            svcOrders.CreateIncomeOrder(wh.WhId, MakeItems(product, 3), "Тест");

            Assert.Equal(countBefore + 1, svcOrders.GetAll().Count);
        }

        [Fact]
        public void CreateIncomeOrder_AddsProductToWarehouse()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];

            svcOrders.CreateIncomeOrder(wh.WhId, MakeItems(product, 1));

            Assert.Contains(product, wh.products);
        }

        [Fact]
        public void CreateIncomeOrder_EmptyItems_ReturnsFalse()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var wh = ctx.Warehouses[0];

            var result = svcOrders.CreateIncomeOrder(
                wh.WhId, new List<OrderItem>());

            Assert.False(result.success);
            Assert.NotEmpty(result.error);
        }

        [Fact]
        public void CreateOutcomeOrder_DecreasesStock()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];
            int stockBefore = product.StockQuantity;
            int qty = 3;

            var result = svcOrders.CreateOutcomeOrder(
                wh.WhId, MakeItems(product, qty));

            Assert.True(result.success);
            Assert.Equal(stockBefore - qty, product.StockQuantity);
        }

        [Fact]
        public void CreateOutcomeOrder_NotEnoughStock_ReturnsFalse()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];

            var result = svcOrders.CreateOutcomeOrder(
                wh.WhId, MakeItems(product, product.StockQuantity + 999));

            Assert.False(result.success);
            Assert.NotEmpty(result.error);
        }

        [Fact]
        public void CreateOutcomeOrder_ZeroQuantity_ReturnsFalse()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];

            var result = svcOrders.CreateOutcomeOrder(
                wh.WhId, MakeItems(product, 0));

            Assert.False(result.success);
        }

        [Fact]
        public void Delete_RemovesOrder()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];

            svcOrders.CreateIncomeOrder(wh.WhId, MakeItems(product, 1));
            int countBefore = svcOrders.GetAll().Count;
            var order = svcOrders.GetAll()[0];

            svcOrders.Delete(order.OrderId);

            Assert.Equal(countBefore - 1, svcOrders.GetAll().Count);
        }

        [Fact]
        public void GetByWarehouse_ReturnsCorrectOrders()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];

            svcOrders.CreateIncomeOrder(wh.WhId, MakeItems(product, 2));

            var orders = svcOrders.GetByWarehouse(wh.WhId);
            Assert.All(orders, o => Assert.Equal(wh.WhId, o.WarehouseId));
        }

        [Fact]
        public void CreateIncomeOrder_OrderHasCorrectItems()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];
            int qty = 7;

            svcOrders.CreateIncomeOrder(wh.WhId, MakeItems(product, qty));

            var order = svcOrders.GetAll()[0];
            Assert.Equal(1, order.Items.Count);
            Assert.Equal(qty, order.Items[0].Quantity);
            Assert.Equal(product.Article, order.Items[0].Product.Article);
        }

        [Fact]
        public void CreateIncomeOrder_MultipleItems_AllProcessed()
        {
            var (ctx, svcOrders, svcProducts) = CreateSvc();
            var product = ctx.Products[0];
            var wh = ctx.Warehouses[0];

            var cat = ctx.Categories[0];
            var man = ctx.Manufacturers[0];
            var sup = ctx.Suppliers[0];
            var product2 = new Product
            {
                Article = "TST-002",
                Name = "Второй товар",
                Unit = "кг",
                Price = 200m,
                StockQuantity = 5,
                CategoryId = cat.Id,
                ManufacturerId = man.Id,
                SupplierId = sup.Id,
                Category = cat,
                Manufacturer = man,
                Supplier = sup
            };
            ctx.Products.Add(product2);

            var items = new List<OrderItem>
            {
                new OrderItem(product, 3),
                new OrderItem(product2, 4)
            };

            int stock1Before = product.StockQuantity;
            int stock2Before = product2.StockQuantity;

            var result = svcOrders.CreateIncomeOrder(wh.WhId, items);

            Assert.True(result.success);
            Assert.Equal(stock1Before + 3, product.StockQuantity);
            Assert.Equal(stock2Before + 4, product2.StockQuantity);
            Assert.Equal(2, svcOrders.GetAll()[0].Items.Count);
        }
    }
}