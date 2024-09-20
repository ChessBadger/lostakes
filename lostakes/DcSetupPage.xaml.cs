using System.Windows;
using System.Windows.Controls;

namespace lostakes
{
    public partial class DcSetupPage : Page
    {
        public DcSetupPage()
        {
            InitializeComponent();
        }

        // Enable or disable dependent checkboxes when "Lookup SKUs" is checked
        private void LookupSkus_Checked(object sender, RoutedEventArgs e)
        {
            LookupPricesCheckbox.IsEnabled = true;
            AllowNotFoundCheckbox.IsEnabled = true;
            SkipFoundPricesCheckbox.IsEnabled = true;
        }

        // Disable dependent checkboxes when "Lookup SKUs" is unchecked
        private void LookupSkus_Unchecked(object sender, RoutedEventArgs e)
        {
            LookupPricesCheckbox.IsEnabled = false;
            AllowNotFoundCheckbox.IsEnabled = false;
            SkipFoundPricesCheckbox.IsEnabled = false;

            // Optionally, uncheck them when disabled
            LookupPricesCheckbox.IsChecked = false;
            AllowNotFoundCheckbox.IsChecked = false;
            SkipFoundPricesCheckbox.IsChecked = false;
        }

        private void UseDollarLimitsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            // Show the Dollar Limits popup when the checkbox is checked
            DollarLimitsWindow dollarLimitsWindow = new DollarLimitsWindow();
            dollarLimitsWindow.Owner = Window.GetWindow(this); // Set the owner to the parent window
            bool? result = dollarLimitsWindow.ShowDialog();

            // Process the result if needed
            if (result == true)
            {
                // Logic to handle the confirmed values, if necessary
                MessageBox.Show("Dollar limits set.");
            }
        }

        private void ChooseSymbologiesCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            // Show the Choose Symbology popup when the checkbox is checked
            ChooseSymbologyWindow chooseSymbologyWindow = new ChooseSymbologyWindow();
            chooseSymbologyWindow.Owner = Window.GetWindow(this); // Set the owner to the main window
            bool? result = chooseSymbologyWindow.ShowDialog();

            // Process the result if needed
            if (result == true)
            {
                // Logic to handle the confirmed values, if necessary
            }
            else
            {
                // Handle if user cancels or closes the popup
            }
        }

        private void ChooseLaserGunOrWand_Checked(object sender, RoutedEventArgs e)
        {
            // Show the Laser Gun or Wand window when the checkbox is checked
            LaserGunOrWandWindow laserGunOrWandWindow = new LaserGunOrWandWindow();
            laserGunOrWandWindow.Owner = Window.GetWindow(this); // Set the owner to the main window
            bool? result = laserGunOrWandWindow.ShowDialog();

            // Process the result if needed
            if (result == true)
            {
                // Logic to handle the confirmed values, if necessary
            }
            else
            {
                // Handle if the user cancels or closes the popup
            }
        }

        // Enable or disable the "Set SKU Length" textbox based on whether "Restrict SKU Length" is checked
        private void RestrictSkuLength_Checked(object sender, RoutedEventArgs e)
        {
            SetSkuLengthTextbox.IsEnabled = true;
        }

        private void RestrictSkuLength_Unchecked(object sender, RoutedEventArgs e)
        {
            SetSkuLengthTextbox.IsEnabled = false;
            SetSkuLengthTextbox.Text = string.Empty; // Optionally clear the text when disabled
        }
    }
}
