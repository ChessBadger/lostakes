using System.Windows;

namespace lostakes
{
    public partial class ChooseSymbologyWindow : Window
    {
        public SymbologyData SymbologyData { get; private set; }

        public ChooseSymbologyWindow(SymbologyData existingSymbologyData)
        {
            InitializeComponent();
            SymbologyData = existingSymbologyData;
            this.Loaded += ChooseSymbologyWindow_Loaded;
        }

        // Enable or disable textboxes based on the check state of the checkboxes
        private void I2of5_Checked(object sender, RoutedEventArgs e)
        {
            I2Of5MinTextbox.IsEnabled = true;
            I2Of5MaxTextbox.IsEnabled = true;
        }

        private void I2of5_Unchecked(object sender, RoutedEventArgs e)
        {
            I2Of5MinTextbox.IsEnabled = false;
            I2Of5MaxTextbox.IsEnabled = false;
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

        // OK Button handler
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SymbologyData = new SymbologyData
            {
                UseUpc = UseUpcCheckbox.IsChecked == true,
                KeepUpc6 = KeepUpc6Checkbox.IsChecked == true,
                I2Of5 = I2Of5Checkbox.IsChecked == true,
                I2Of5Min = I2Of5MinTextbox.Text,
                I2Of5Max = I2Of5MaxTextbox.Text,
                Code39 = Code39Checkbox.IsChecked == true,
                Code39Min = Code39MinTextbox.Text,
                Code39Max = Code39MaxTextbox.Text,
                Code11 = Code11Checkbox.IsChecked == true,
                Code11Min = Code11MinTextbox.Text,
                Code11Max = Code11MaxTextbox.Text,
                Codebar = CodebarCheckbox.IsChecked == true,
                CodebarMin = CodebarMinTextbox.Text,
                CodebarMax = CodebarMaxTextbox.Text,
                Plessey = PlesseyCheckbox.IsChecked == true,
                PlesseyMin = PlesseyMinTextbox.Text,
                PlesseyMax = PlesseyMaxTextbox.Text,
                Ean8 = Ean8Checkbox.IsChecked == true,
                Ean13 = Ean13Checkbox.IsChecked == true,
                Code128 = Code128Checkbox.IsChecked == true,
                Code128Min = Code128MinTextbox.Text,
                Code128Max = Code128MaxTextbox.Text
            };

            DialogResult = true;
            this.Close();
        }

        // Cancel Button handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        // Populate the checkboxes and textboxes when the window is loaded
        private void ChooseSymbologyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Populate checkboxes
            UseUpcCheckbox.IsChecked = SymbologyData.UseUpc;
            KeepUpc6Checkbox.IsChecked = SymbologyData.KeepUpc6;
            I2Of5Checkbox.IsChecked = SymbologyData.I2Of5;
            Code39Checkbox.IsChecked = SymbologyData.Code39;
            Code11Checkbox.IsChecked = SymbologyData.Code11;
            CodebarCheckbox.IsChecked = SymbologyData.Codebar;
            PlesseyCheckbox.IsChecked = SymbologyData.Plessey;
            Ean8Checkbox.IsChecked = SymbologyData.Ean8;
            Ean13Checkbox.IsChecked = SymbologyData.Ean13;
            Code128Checkbox.IsChecked = SymbologyData.Code128;

            //// Populate textboxes
            //I2Of5MinTextbox.Text = SymbologyData.I2Of5Min;
            //I2Of5MaxTextbox.Text = SymbologyData.I2Of5Max;
            //Code39MinTextbox.Text = SymbologyData.Code39Min;
            //Code39MaxTextbox.Text = SymbologyData.Code39Max;
            //Code11MinTextbox.Text = SymbologyData.Code11Min;
            //Code11MaxTextbox.Text = SymbologyData.Code11Max;
            //CodebarMinTextbox.Text = SymbologyData.CodebarMin;
            //CodebarMaxTextbox.Text = SymbologyData.CodebarMax;
            //PlesseyMinTextbox.Text = SymbologyData.PlesseyMin;
            //PlesseyMaxTextbox.Text = SymbologyData.PlesseyMax;
            //Code128MinTextbox.Text = SymbologyData.Code128Min;
            //Code128MaxTextbox.Text = SymbologyData.Code128Max;

            // Enable/Disable textboxes based on the checkbox states
            I2Of5MinTextbox.IsEnabled = I2Of5Checkbox.IsChecked == true;
            I2Of5MaxTextbox.IsEnabled = I2Of5Checkbox.IsChecked == true;
            Code39MinTextbox.IsEnabled = Code39Checkbox.IsChecked == true;
            Code39MaxTextbox.IsEnabled = Code39Checkbox.IsChecked == true;
            Code11MinTextbox.IsEnabled = Code11Checkbox.IsChecked == true;
            Code11MaxTextbox.IsEnabled = Code11Checkbox.IsChecked == true;
            CodebarMinTextbox.IsEnabled = CodebarCheckbox.IsChecked == true;
            CodebarMaxTextbox.IsEnabled = CodebarCheckbox.IsChecked == true;
            PlesseyMinTextbox.IsEnabled = PlesseyCheckbox.IsChecked == true;
            PlesseyMaxTextbox.IsEnabled = PlesseyCheckbox.IsChecked == true;
            Code128MinTextbox.IsEnabled = Code128Checkbox.IsChecked == true;
            Code128MaxTextbox.IsEnabled = Code128Checkbox.IsChecked == true;
        }

    }

    // Data structure to hold the symbology information
    public class SymbologyData
    {
        public bool UseUpc { get; set; }
        public bool KeepUpc6 { get; set; }
        public bool I2Of5 { get; set; }
        public string I2Of5Min { get; set; }
        public string I2Of5Max { get; set; }
        public bool Code39 { get; set; }
        public string Code39Min { get; set; }
        public string Code39Max { get; set; }
        public bool Code11 { get; set; }
        public string Code11Min { get; set; }
        public string Code11Max { get; set; }
        public bool Codebar { get; set; }
        public string CodebarMin { get; set; }
        public string CodebarMax { get; set; }
        public bool Plessey { get; set; }
        public string PlesseyMin { get; set; }
        public string PlesseyMax { get; set; }
        public bool Ean8 { get; set; }
        public bool Ean13 { get; set; }
        public bool Code128 { get; set; }
        public string Code128Min { get; set; }
        public string Code128Max { get; set; }
    }



}
