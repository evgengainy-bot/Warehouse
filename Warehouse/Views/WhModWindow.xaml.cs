using System.Windows;
using System.Windows.Input;
using WarehouseData.Services;

using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse.Views
{
    public partial class WhModWindow : Window
    {
        private readonly int _mode;
        private readonly WhModel? _wh;
        private readonly ServiceWhs _service;

        public WhModWindow(string name, string address, int mode,
            WhModel? wh, ServiceWhs service)
        {
            InitializeComponent();

            tbName.Text = name;
            tbAddress.Text = address;
            _mode = mode;
            _wh = wh;
            _service = service;

            this.Title = mode == 1 ? "Новый склад" : "Изменить склад";
            tbName.Focus();
            tbName.SelectAll();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            string name = tbName.Text.Trim();
            string address = tbAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите наименование склада.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbName.Focus();
                return;
            }

            if (_mode == 1)
            {
                var newWh = new WhModel(name, address, _service.Org.OrgId);
                _service.Add(newWh);
            }
            else if (_mode == 2 && _wh != null)
            {
                _service.Update(_wh.WhId, name, address);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnOk_Click(sender, e);
            else if (e.Key == Key.Escape)
                BtnCancel_Click(sender, e);
        }
    }
}