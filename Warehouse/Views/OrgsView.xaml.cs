using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarehouseData.Models;
using WarehouseData.Services;

namespace Warehouse.Views
{
    public partial class OrgsView : Window
    {
        private readonly ServiceOrgs _service;
        private Organization? _selectedOrg;

        public OrgsView(ServiceOrgs svcOrgs)
        {
            InitializeComponent();
            _service = svcOrgs;
            FillOrgsCollection();
            lbxOrgsList.Focus();
        }

        private void FillOrgsCollection()
        {
            int idx = lbxOrgsList.SelectedIndex;
            if (idx < 0) idx = 0;

            lbxOrgsList.ItemsSource = null;
            lbxOrgsList.Items.Clear();
            lbxOrgsList.ItemsSource = _service.GetAll();

            if (lbxOrgsList.Items.Count > 0)
            {
                try { lbxOrgsList.SelectedIndex = idx; }
                catch { lbxOrgsList.SelectedIndex = 0; }
            }
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_selectedOrg != null && !string.IsNullOrWhiteSpace(_selectedOrg.OrgName))
            {
                MainWindow mainWindow = new MainWindow(_service.GetContext(), _selectedOrg);
                mainWindow.Show();
             
            }
            else
            {
                MessageBox.Show("Выберите организацию из списка.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OrgAdd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OrgModWindow dlg = new OrgModWindow("Новая организация", "Наименование:", "", 1, null, _service);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
                FillOrgsCollection();
        }

        private void OrgEdit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_selectedOrg == null)
            {
                MessageBox.Show("Выберите организацию для редактирования.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OrgModWindow dlg = new OrgModWindow("Изменить организацию", "Наименование:",
                _selectedOrg.OrgName, 2, _selectedOrg, _service);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
                FillOrgsCollection();
        }

        private void OrgDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_selectedOrg == null)
            {
                MessageBox.Show("Выберите организацию для удаления.", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OrgDeleteWindow dlg = new OrgDeleteWindow(_selectedOrg);
            dlg.Owner = this;

            if (dlg.ShowDialog() == true)
            {
                _service.Delete(_selectedOrg.OrgId);
                _selectedOrg = null;
                FillOrgsCollection();
                tbStatus.Text = "Организация удалена";
            }
        }

        private void lbxOrgsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedOrg = lbxOrgsList.SelectedItem as Organization;
            if (_selectedOrg != null && !string.IsNullOrWhiteSpace(_selectedOrg.OrgName))
                tbStatus.Text = _selectedOrg.OrgName;
        }
    }
}