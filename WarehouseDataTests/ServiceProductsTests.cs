using System.Numerics;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;
using Xunit;

namespace WarehouseDataTests
{
    public class ServiceProductsTests
    {
        private (ApplicationContext ctx, ServiceProducts svc) CreateSvc()
        {
            Organization.OrgCounter = BigInteger.Zero;
            Warehouse.WarehousesCounter = BigInteger.Zero;

            var ctx = new ApplicationContext();

            ctx.Categories.Add(new Category
            {
                Id = 1,
                Name = "Категория"
            });

            ctx.Manufacturers.Add(new Manufacturer
            {
                Id = 1,
                Name = "Производитель"
            });

            ctx.Suppliers.Add(new Supplier
            {
                Id = 1,
                Name = "Поставщик"
            });

            var svc = new ServiceProducts(ctx);

            return (ctx, svc);
        }

        private Product CreateTestProduct(ApplicationContext ctx, string article = "TEST-001")
        {
            return new Product
            {
                Article = article,
                Name = "Тестовый товар",
                Unit = "шт",
                Price = 100m,
                StockQuantity = 10,

                Category = ctx.Categories[0],
                Manufacturer = ctx.Manufacturers[0],
                Supplier = ctx.Suppliers[0],

                CategoryId = ctx.Categories[0].Id,
                ManufacturerId = ctx.Manufacturers[0].Id,
                SupplierId = ctx.Suppliers[0].Id
            };
        }

        [Fact]
        public void GetAll_ReturnsProducts()
        {
            var (ctx, svc) = CreateSvc();

            var product = CreateTestProduct(ctx);
            svc.Add(product);

            var products = svc.GetAll();

            Assert.NotNull(products);
            Assert.True(products.Count > 0);
        }

        [Fact]
        public void Add_AddsProduct()
        {
            var (ctx, svc) = CreateSvc();

            int countBefore = svc.GetAll().Count;

            var product = CreateTestProduct(ctx);

            svc.Add(product);

            Assert.Equal(countBefore + 1, svc.GetAll().Count);
        }

        [Fact]
        public void GetByArticle_ReturnsCorrectProduct()
        {
            var (ctx, svc) = CreateSvc();

            var product = CreateTestProduct(ctx);

            svc.Add(product);

            var found = svc.GetByArticle(product.Article);

            Assert.NotNull(found);
            Assert.Equal(product.Article, found!.Article);
        }

        [Fact]
        public void Delete_RemovesProduct()
        {
            var (ctx, svc) = CreateSvc();

            var product = CreateTestProduct(ctx);

            svc.Add(product);

            int countBefore = svc.GetAll().Count;

            svc.Delete(product.Article);

            Assert.Equal(countBefore - 1, svc.GetAll().Count);
        }

        [Fact]
        public void ChangeStock_IncreasesQuantity()
        {
            var (ctx, svc) = CreateSvc();

            var product = CreateTestProduct(ctx);

            svc.Add(product);

            int before = product.StockQuantity;

            bool result = svc.ChangeStock(product.Article, 5);

            Assert.True(result);
            Assert.Equal(before + 5, product.StockQuantity);
        }

        [Fact]
        public void ChangeStock_DecreasesQuantity()
        {
            var (ctx, svc) = CreateSvc();

            var product = CreateTestProduct(ctx);

            svc.Add(product);

            int before = product.StockQuantity;

            bool result = svc.ChangeStock(product.Article, -before);

            Assert.True(result);
            Assert.Equal(0, product.StockQuantity);
        }

        [Fact]
        public void ChangeStock_BelowZero_ReturnsFalse()
        {
            var (ctx, svc) = CreateSvc();

            var product = CreateTestProduct(ctx);

            svc.Add(product);

            bool result = svc.ChangeStock(
                product.Article,
                -(product.StockQuantity + 1));

            Assert.False(result);
        }

        [Fact]
        public void Search_FindsByName()
        {
            var (ctx, svc) = CreateSvc();

            var product = CreateTestProduct(ctx);

            svc.Add(product);

            string query = product.Name.Substring(0, 4);

            var results = svc.Search(query);

            Assert.Contains(
                results,
                p => p.Article == product.Article);
        }
    }
}