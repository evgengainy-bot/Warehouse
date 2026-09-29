using System.Collections.ObjectModel;
using WarehouseData.Models;

namespace WarehouseData.Context
{
    public class ApplicationContext
    {
        public ObservableCollection<Organization> Organizations { get; set; }
        public ObservableCollection<Warehouse> Warehouses { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Manufacturer> Manufacturers { get; set; }
        public ObservableCollection<Supplier> Suppliers { get; set; }

        public ApplicationContext()
        {
            Organizations = new ObservableCollection<Organization>();
            Warehouses = new ObservableCollection<Warehouse>();
            Products = new ObservableCollection<Product>();
            Orders = new ObservableCollection<Order>();
            Categories = new ObservableCollection<Category>();
            Manufacturers = new ObservableCollection<Manufacturer>();
            Suppliers = new ObservableCollection<Supplier>();

            FillTestData();
        }

        private void FillTestData()
        {
            var org1 = new Organization("Рога и копыта, ООО");
            var org2 = new Organization("Пупкин и сыновья, ООО");

            var wh1 = new Warehouse("Склад Рогов и Копыт №1", "ул. Лесная, 1", org1.OrgId);
            var wh2 = new Warehouse("Склад Рогов и Копыт №2", "ул. Лесная, 2", org1.OrgId);
            var wh3 = new Warehouse("Склад Пупкина №1", "пр. Горный, 10", org2.OrgId);
            var wh4 = new Warehouse("Склад Пупкина №2", "пр. Горный, 12", org2.OrgId);

            org1.warehouses.Add(wh1);
            org1.warehouses.Add(wh2);
            org2.warehouses.Add(wh3);
            org2.warehouses.Add(wh4);

            Organizations.Add(org1);
            Organizations.Add(org2);

            Warehouses.Add(wh1);
            Warehouses.Add(wh2);
            Warehouses.Add(wh3);
            Warehouses.Add(wh4);

            Categories.Add(new Category(1, "Электроника"));
            Categories.Add(new Category(2, "Одежда и обувь"));
            Categories.Add(new Category(3, "Продукты питания"));
            Categories.Add(new Category(4, "Инструменты"));
            Categories.Add(new Category(5, "Канцелярия"));
            Categories.Add(new Category(6, "Другое"));
        }
    }
}