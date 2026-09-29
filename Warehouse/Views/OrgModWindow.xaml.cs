using System;
using System.Windows;
using System.Windows.Input;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse.Views
{
    public partial class OrgModWindow : Window
    {
        private readonly int _mode;
        private readonly Organization? _org;
        private readonly ServiceOrgs _service;

        public OrgModWindow(string title, string label, string currentName,
            int mode, Organization? org, ServiceOrgs service)
        {
            InitializeComponent();

            this.Title = title;
            lblTitle.Content = label;
            tbName.Text = currentName;
            tbName.SelectAll();
            tbName.Focus();

            _mode = mode;
            _org = org;
            _service = service;

            if (mode == 3)
                tbName.IsReadOnly = true;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string name = tbName.Text.Trim();

            if (_mode != 3 && string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите наименование организации.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbName.Focus();
                return;
            }

            if (_mode == 1)
            {
                var newOrg = new Organization(name);
                _service.Add(newOrg);
            }
            else if (_mode == 2 && _org != null)
            {
                _service.Update(_org.OrgId, name);
            }
            else if (_mode == 3 && _org != null)
            {
                _service.Delete(_org.OrgId);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnOk_Click(sender, e);
            else if (e.Key == Key.Escape)
                BtnCancel_Click(sender, e);
        }
    }
}