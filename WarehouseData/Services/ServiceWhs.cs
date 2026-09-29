using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using WarehouseData.Context;
using WarehouseData.Models;

namespace WarehouseData.Services
{
    public class ServiceWhs
    {
        private readonly ApplicationContext _context;
        public Organization Org { get; private set; }

        public ServiceWhs(ApplicationContext context, Organization org)
        {
            _context = context;
            Org = org;
        }

        public ApplicationContext GetContext()
        {
            return _context;
        }

        public ObservableCollection<Warehouse> GetAll()
        {
            return _context.Warehouses;
        }

        public List<Warehouse> GetByOrg()
        {
            var result = new List<Warehouse>();
            foreach (var wh in _context.Warehouses)
            {
                if (wh.OrgId == Org.OrgId)
                    result.Add(wh);
            }
            return result;
        }

        public Warehouse? GetById(BigInteger id)
        {
            foreach (var wh in _context.Warehouses)
            {
                if (wh.WhId == id)
                    return wh;
            }
            return null;
        }

        public void Add(Warehouse wh)
        {
            if (wh == null) return;
            if (string.IsNullOrWhiteSpace(wh.WhName)) return;

            _context.Warehouses.Add(wh);
            Org.warehouses.Add(wh);
        }

        public void Update(BigInteger id, string newName, string newAddress)
        {
            var wh = GetById(id);
            if (wh == null) return;
            if (string.IsNullOrWhiteSpace(newName)) return;

            wh.WhName = newName;
            wh.WhAddress = newAddress;
        }

        public void Delete(BigInteger id)
        {
            var wh = GetById(id);
            if (wh == null) return;

            _context.Warehouses.Remove(wh);

            Org.warehouses.Remove(wh);
        }
    }
}