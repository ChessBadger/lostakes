using System.Windows;
using System.IO;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

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
            LoadLimitsFromFile(@"C:\\Lostakes Data\\HHConfigData.dlf");

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

        private void LoadLimitsFromFile(string filePath)
        {
            try
            {
                // Read all content from the file
                string fileContent = File.ReadAllText(filePath);

                // Extract values using Substring
                EntryLimit = fileContent.Substring(26, 7);  // Entry Limit from characters 27-33 (index 26-32)
                PriceLimit = fileContent.Substring(35, 4);  // Price Limit from characters 36-39 (index 35-38)
                QtyLimit = fileContent.Substring(41, 9);    // Quantity Limit from characters 42-50 (index 41-49)
                CombinedLimit = fileContent.Substring(50, 9);  // Combined Limit from characters 51-59 (index 50-58)

                // Populate the textboxes with the extracted values
                EntryLimitTextbox.Text = EntryLimit.TrimStart('0');
                PriceLimitTextbox.Text = PriceLimit.TrimStart('0');
                QtyLimitTextbox.Text = QtyLimit.TrimStart('0');
                CombinedLimitTextbox.Text = CombinedLimit.TrimStart('0');
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
