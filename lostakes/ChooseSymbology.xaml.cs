using System.Windows;

namespace lostakes
{
    public partial class ChooseSymbologyWindow : Window
    {
        public ChooseSymbologyWindow()
        {
            InitializeComponent();
        }

        // Enable or disable textboxes based on the check state of the checkboxes
        private void I2of5_Checked(object sender, RoutedEventArgs e)
        {
            I2of5MinTextbox.IsEnabled = true;
            I2of5MaxTextbox.IsEnabled = true;
        }

        private void I2of5_Unchecked(object sender, RoutedEventArgs e)
        {
            I2of5MinTextbox.IsEnabled = false;
            I2of5MaxTextbox.IsEnabled = false;
        }

        private void Code39_Checked(object sender, RoutedEventArgs e)
        {
            Code39MinTextbox.IsEnabled = true;
            Code39MaxTextbox.IsEnabled = true;
        }

        private void Code39_Unchecked(object sender, RoutedEventArgs e)
        {
            Code39MinTextbox.IsEnabled = false;
            Code39MaxTextbox.IsEnabled = false;
        }

        private void Code11_Checked(object sender, RoutedEventArgs e)
        {
            Code11MinTextbox.IsEnabled = true;
            Code11MaxTextbox.IsEnabled = true;
        }

        private void Code11_Unchecked(object sender, RoutedEventArgs e)
        {
            Code11MinTextbox.IsEnabled = false;
            Code11MaxTextbox.IsEnabled = false;
        }

        private void Codebar_Checked(object sender, RoutedEventArgs e)
        {
            CodebarMinTextbox.IsEnabled = true;
            CodebarMaxTextbox.IsEnabled = true;
        }

        private void Codebar_Unchecked(object sender, RoutedEventArgs e)
        {
            CodebarMinTextbox.IsEnabled = false;
            CodebarMaxTextbox.IsEnabled = false;
        }

        private void Plessey_Checked(object sender, RoutedEventArgs e)
        {
            PlesseyMinTextbox.IsEnabled = true;
            PlesseyMaxTextbox.IsEnabled = true;
        }

        private void Plessey_Unchecked(object sender, RoutedEventArgs e)
        {
            PlesseyMinTextbox.IsEnabled = false;
            PlesseyMaxTextbox.IsEnabled = false;
        }

        private void Code128_Checked(object sender, RoutedEventArgs e)
        {
            Code128MinTextbox.IsEnabled = true;
            Code128MaxTextbox.IsEnabled = true;
        }

        private void Code128_Unchecked(object sender, RoutedEventArgs e)
        {
            Code128MinTextbox.IsEnabled = false;
            Code128MaxTextbox.IsEnabled = false;
        }

        // OK button handler
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
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
