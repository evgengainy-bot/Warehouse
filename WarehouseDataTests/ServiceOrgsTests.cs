using System.Numerics;
using WarehouseData.Context;
using WarehouseData.Models;
using WarehouseData.Services;
using Xunit;

namespace WarehouseDataTests
{
    public class ServiceOrgsTests
    {
        private ApplicationContext CreateFreshContext()
        {
            Organization.OrgCounter = BigInteger.Zero;
            Warehouse.WarehousesCounter = BigInteger.Zero;
            return new ApplicationContext();
        }

        [Fact]
        public void GetAll_ReturnsOrganizations()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);

            var orgs = svc.GetAll();

            Assert.NotNull(orgs);
            Assert.True(orgs.Count > 0);
        }

        [Fact]
        public void Add_AddsOrganization()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);
            int countBefore = svc.GetAll().Count;

            var newOrg = new Organization("Тест организация");
            svc.Add(newOrg);

            Assert.Equal(countBefore + 1, svc.GetAll().Count);
        }

        [Fact]
        public void Add_NullOrg_DoesNotAdd()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);
            int countBefore = svc.GetAll().Count;

            svc.Add(null!);

            Assert.Equal(countBefore, svc.GetAll().Count);
        }

        [Fact]
        public void Add_EmptyName_DoesNotAdd()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);
            int countBefore = svc.GetAll().Count;

            var emptyOrg = new Organization("   ");
            
            svc.Add(emptyOrg);

            Assert.Equal(countBefore, svc.GetAll().Count);
        }

        [Fact]
        public void GetById_ReturnsCorrectOrg()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);
            var first = svc.GetAll()[0];

            var found = svc.GetById(first.OrgId);

            Assert.NotNull(found);
            Assert.Equal(first.OrgId, found!.OrgId);
        }

        [Fact]
        public void Update_ChangesOrgName()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);
            var org = svc.GetAll()[0];
            string newName = "Новое название";

            svc.Update(org.OrgId, newName);

            var updated = svc.GetById(org.OrgId);
            Assert.Equal(newName, updated!.OrgName);
        }

        [Fact]
        public void Delete_RemovesOrganization()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);
            int countBefore = svc.GetAll().Count;
            var org = svc.GetAll()[0];

            svc.Delete(org.OrgId);

            Assert.Equal(countBefore - 1, svc.GetAll().Count);
        }

        [Fact]
        public void Delete_AlsoRemovesWarehouses()
        {
            var ctx = CreateFreshContext();
            var svc = new ServiceOrgs(ctx);
            var org = svc.GetAll()[0];
            int whCountBefore = ctx.Warehouses.Count;
            int orgWhCount = org.warehouses.Count;

            svc.Delete(org.OrgId);

            Assert.Equal(whCountBefore - orgWhCount, ctx.Warehouses.Count);
        }
    }
}