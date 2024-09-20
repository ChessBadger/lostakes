using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace lostakes
{
    public partial class DollarLimitsWindow : Window
    {
        public DollarLimitsWindow()
        {
            InitializeComponent();
        }

        // Ensure only numeric input is allowed in the textboxes
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Only allow digits
            e.Handled = regex.IsMatch(e.Text);
        }

        // OK button handler
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // Optionally, you can process the values entered here
            this.DialogResult = true;
            this.Close();
        }

        // Cancel button handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
