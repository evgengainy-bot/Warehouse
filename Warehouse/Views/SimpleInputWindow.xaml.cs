using System.Windows;
using System.Windows.Input;

namespace Warehouse.Views
{
    public partial class SimpleInputWindow : Window
    {
        public string InputValue { get; private set; } = "";

        public SimpleInputWindow(string title, string prompt)
        {
            InitializeComponent();
            this.Title = title;
            lblPrompt.Content = prompt;
            tbInput.Focus();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            InputValue = tbInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(InputValue))
            {
                MessageBox.Show("Поле не может быть пустым.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (e.Key == Key.Enter) BtnOk_Click(sender, e);
            else if (e.Key == Key.Escape) BtnCancel_Click(sender, e);
        }
    }
}