using System.Windows;
using WarehouseData.Context;
using WarehouseData.Services;
using Warehouse.Views;

namespace Warehouse
{
    public partial class App : Application
    {
        private ApplicationContext _context;
        public ServiceOrgs SvcOrgs { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _context = new ApplicationContext();
            SvcOrgs = new ServiceOrgs(_context);

            OrgsView orgsView = new OrgsView(SvcOrgs);
            orgsView.Show();
        }
    }
}