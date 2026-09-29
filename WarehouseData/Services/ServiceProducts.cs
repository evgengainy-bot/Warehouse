using System.Collections.Generic;
using System.Collections.ObjectModel;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class ServiceProducts
    {
        private readonly ApplicationContext _context;

        public ServiceProducts(ApplicationContext context)
        {
            _context = context;
        }

        public ApplicationContext GetContext()
        {
            return _context;
        }

        public ObservableCollection<Product> GetAll()
        {
            return _context.Products;
        }

        public Product? GetByArticle(string article)
        {
            foreach (var p in _context.Products)
            {
                if (p.Article == article)
                    return p;
            }
            return null;
        }

        public void Add(Product product)
        {
            if (product == null) return;
            if (string.IsNullOrWhiteSpace(product.Name)) return;

            if (string.IsNullOrWhiteSpace(product.Article))
                product.Article = GenerateArticle();

            _context.Products.Add(product);
        }

        public void Update(string article, Product updated)
        {
            var product = GetByArticle(article);
            if (product == null) return;

            product.Name = updated.Name;
            product.Unit = updated.Unit;
            product.Price = updated.Price;
            product.CategoryId = updated.CategoryId;
            product.ManufacturerId = updated.ManufacturerId;
            product.SupplierId = updated.SupplierId;
            product.DiscountPercent = updated.DiscountPercent;
            product.Description = updated.Description;
            product.PhotoPath = updated.PhotoPath;
            product.Category = updated.Category;
            product.Manufacturer = updated.Manufacturer;
            product.Supplier = updated.Supplier;
        }

        private string GenerateArticle()
        {
            return "ART-" + (_context.Products.Count + 1).ToString("D4");
        }

        public void Delete(string article)
        {
            var product = GetByArticle(article);
            if (product == null) return;
            _context.Products.Remove(product);
        }

        public List<Product> Search(string query)
        {
            var result = new List<Product>();
            if (string.IsNullOrWhiteSpace(query))
            {
                result.AddRange(_context.Products);
                return result;
            }

            string lower = query.ToLower();
            foreach (var p in _context.Products)
            {
                if (p.Name.ToLower().Contains(lower) ||
                    p.Article.ToLower().Contains(lower))
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public List<Product> FilterBySupplier(int supplierId)
        {
            var result = new List<Product>();
            foreach (var p in _context.Products)
            {
                if (p.SupplierId == supplierId)
                    result.Add(p);
            }
            return result;
        }

        public bool ChangeStock(string article, int delta)
        {
            var product = GetByArticle(article);
            if (product == null) return false;

            int newQty = product.StockQuantity + delta;
            if (newQty < 0) return false; 

            product.StockQuantity = newQty;
            return true;
        }
    }
}