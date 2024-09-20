using System.Windows;

namespace lostakes
{
    public partial class DollarLimitsWindow : Window
    {
        // Properties to hold the limits
        public string EntryLimit { get; set; }
        public string PriceLimit { get; set; }
        public string QtyLimit { get; set; }
        public string CombinedLimit { get; set; }

        public DollarLimitsWindow()
        {
            InitializeComponent();
        }

        // OK button handler
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the values to the properties
            EntryLimit = EntryLimitTextbox.Text;
            PriceLimit = PriceLimitTextbox.Text;
            QtyLimit = QtyLimitTextbox.Text;
            CombinedLimit = CombinedLimitTextbox.Text;

            // Close the window and return DialogResult as true
            this.DialogResult = true;
            this.Close();
        }

        // Cancel button handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window without setting the limits
            this.DialogResult = false;
            this.Close();
        }
    }
}
