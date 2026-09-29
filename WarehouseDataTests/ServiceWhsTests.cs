using System.Numerics;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;
using Xunit;

namespace WarehouseDataTests
{
    public class ServiceWhsTests
    {
        private (ApplicationContext ctx, ServiceWhs svc, Organization org) CreateSvc()
        {
            Organization.OrgCounter = BigInteger.Zero;
            Warehouse.WarehousesCounter = BigInteger.Zero;
            var ctx = new ApplicationContext();
            var org = ctx.Organizations[0];
            var svc = new ServiceWhs(ctx, org);
            return (ctx, svc, org);
        }

        [Fact]
        public void GetByOrg_ReturnsOnlyOrgWarehouses()
        {
            var (ctx, svc, org) = CreateSvc();

            var whs = svc.GetByOrg();

            Assert.All(whs, wh => Assert.Equal(org.OrgId, wh.OrgId));
        }

        [Fact]
        public void Add_AddsWarehouse()
        {
            var (ctx, svc, org) = CreateSvc();
            int countBefore = svc.GetByOrg().Count;

            var newWh = new Warehouse("Новый склад", "ул. Новая, 1", org.OrgId);
            svc.Add(newWh);

            Assert.Equal(countBefore + 1, svc.GetByOrg().Count);
        }

        [Fact]
        public void Add_AlsoAddsToOrgCollection()
        {
            var (ctx, svc, org) = CreateSvc();
            int orgWhCountBefore = org.warehouses.Count;

            var newWh = new Warehouse("Склад доп", "Адрес", org.OrgId);
            svc.Add(newWh);

            Assert.Equal(orgWhCountBefore + 1, org.warehouses.Count);
        }

        [Fact]
        public void Update_ChangesWhName()
        {
            var (ctx, svc, org) = CreateSvc();
            var wh = svc.GetByOrg()[0];
            string newName = "Обновлённый склад";

            svc.Update(wh.WhId, newName, wh.WhAddress);

            var updated = svc.GetById(wh.WhId);
            Assert.Equal(newName, updated!.WhName);
        }

        [Fact]
        public void Delete_RemovesWarehouse()
        {
            var (ctx, svc, org) = CreateSvc();
            int countBefore = svc.GetByOrg().Count;
            var wh = svc.GetByOrg()[0];

            svc.Delete(wh.WhId);

            Assert.Equal(countBefore - 1, svc.GetByOrg().Count);
        }
    }
}