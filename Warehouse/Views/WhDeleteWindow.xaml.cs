using System.Windows;
using System.Windows.Input;

using WhModel = WarehouseData.Models.Warehouse;

namespace Warehouse.Views
{
    public partial class WhDeleteWindow : Window
    {
        private readonly WhModel _wh;

        public WhDeleteWindow(WhModel wh)
        {
            InitializeComponent();
            _wh = wh;
            tbWhName.Text = wh.WhName;
            tbInput.Focus();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string entered = tbInput.Text.Trim();

            if (entered != _wh.WhName)
            {
                MessageBox.Show(
                    "Введённое название не совпадает с названием склада.\nУдаление отменено.",
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