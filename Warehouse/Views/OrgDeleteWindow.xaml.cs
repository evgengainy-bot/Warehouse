using System.Windows;
using System.Windows.Input;
using WarehouseData.Models;

namespace Warehouse.Views
{
    public partial class OrgDeleteWindow : Window
    {
        private readonly Organization _org;

        public OrgDeleteWindow(Organization org)
        {
            InitializeComponent();
            _org = org;
            tbOrgName.Text = org.OrgName;
            tbInput.Focus();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string entered = tbInput.Text.Trim();

            if (entered != _org.OrgName)
            {
                MessageBox.Show(
                    "Введённое название не совпадает с названием организации.\nУдаление отменено.",
                    "Ошибка подтверждения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                tbInput.Clear();
                tbInput.Focus();
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnDelete_Click(sender, e);
            else if (e.Key == Key.Escape)
                BtnCancel_Click(sender, e);
        }
    }
}