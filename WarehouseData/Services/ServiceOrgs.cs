using System.Collections.ObjectModel;
using System.Numerics;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class ServiceOrgs
    {
        private readonly ApplicationContext _context;

        public ServiceOrgs(ApplicationContext context)
        {
            _context = context;
        }

        public ApplicationContext GetContext()
        {
            return _context;
        }

        public ObservableCollection<Organization> GetAll()
        {
            return _context.Organizations;
        }

        public Organization? GetById(BigInteger id)
        {
            foreach (var org in _context.Organizations)
            {
                if (org.OrgId == id)
                    return org;
            }
            return null;
        }

        public void Add(Organization org)
        {
            if (org == null) return;
            if (string.IsNullOrWhiteSpace(org.OrgName)) return;
            _context.Organizations.Add(org);
        }

        public void Update(BigInteger id, string newName)
        {
            var org = GetById(id);
            if (org == null) return;
            if (string.IsNullOrWhiteSpace(newName)) return;
            org.OrgName = newName;
        }

        public void Delete(BigInteger id)
        {
            var org = GetById(id);
            if (org == null) return;

            var whsToRemove = new System.Collections.Generic.List<Warehouse>();
            foreach (var wh in _context.Warehouses)
            {
                if (wh.OrgId == id)
                    whsToRemove.Add(wh);
            }
            foreach (var wh in whsToRemove)
            {
                _context.Warehouses.Remove(wh);
            }

            _context.Organizations.Remove(org);
        }
    }
}