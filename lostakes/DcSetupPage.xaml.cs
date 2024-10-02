using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static lostakes.ChooseSymbologyWindow;
using static lostakes.LaserGunOrWandWindow;

namespace lostakes
{
    public partial class DcSetupPage : Page
    {

        // Properties to store the limits
        private string entryLimit = "0000000";
        private string priceLimit = "0000";
        private string qtyLimit = "000000000";
        private string combinedLimit = "000000000";
        private SymbologyData symbologyData;
        private LaserGunOrWandData laserGunOrWandData;


        public DcSetupPage()
        {
            InitializeComponent();
            LoadSettings();
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

        private void ChooseSymbologiesCheckbox_Clicked(object sender, RoutedEventArgs e)
        {
            if (ChooseLaserGunOrWandCheckbox.IsChecked == true)
            {
                ChooseSymbologyWindow chooseSymbologyWindow = new ChooseSymbologyWindow(symbologyData);
                chooseSymbologyWindow.Owner = Window.GetWindow(this); // Set the owner to the main window
                bool? result = chooseSymbologyWindow.ShowDialog();

                if (result == true)
                {
                    symbologyData = chooseSymbologyWindow.SymbologyData;
                }
            }
        }

        private void ChooseLaserGunOrWand_Clicked(object sender, RoutedEventArgs e)
        {
            if (ChooseLaserGunOrWandCheckbox.IsChecked != true)
            {
                // Show the Laser Gun or Wand window when the checkbox is checked
                LaserGunOrWandWindow laserGunOrWandWindow = new LaserGunOrWandWindow(laserGunOrWandData);
                laserGunOrWandWindow.Owner = Window.GetWindow(this); // Set the owner to the main window
                bool? result = laserGunOrWandWindow.ShowDialog();

                // Process the result if needed
                if (result == true)
                {
                    laserGunOrWandData = laserGunOrWandWindow.LaserGunOrWandData;
                }
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the HHConfigData.dlf file
            CreateHHConfigDataFile();

            // Navigate back to the Send to DC page
            NavigationService?.Navigate(new SendToDcPage());
        }

        private void CreateHHConfigDataFile()
        {
            // Prepare the content for the file as per your specification
            StringBuilder fileContent = new StringBuilder();


            // Append 60 zeros (characters 1-60)
            fileContent.Append(new string('0', 60));

            // Append 10 spaces (characters 61-70)
            fileContent.Append(new string(' ', 10));

            // Append 59 zeros (characters 71-129)
            fileContent.Append(new string('0', 59));

            // Append space, 50, space, 30, space (characters 130-136)
            fileContent.Append(" 50 30 ");

            // Append 'R' from characters 137-186
            fileContent.Append(new string('R', 50));

            // Append 3 zeros (characters 187-189)
            fileContent.Append("000");

            fileContent[127] = '5'; // 128th character

            // 1. Set the 26th character (index 25) to 1 if UseDollarLimitsCheckbox is checked
            if (UseDollarLimitsCheckbox.IsChecked == true)
            {
                fileContent[25] = '1'; // 26th character (index 25)

                // Set Entry Limit (characters 27-33), index 26 to 32
                fileContent.Remove(26, 7).Insert(26, entryLimit);

                // Set Price Limit (characters 36-39), index 35 to 38
                fileContent.Remove(35, 4).Insert(35, priceLimit);

                // Set Quantity Limit (characters 42-50), index 41 to 49
                fileContent.Remove(41, 9).Insert(41, qtyLimit);

                // Set Combined Limit (characters 51-59), index 50 to 58
                fileContent.Remove(50, 9).Insert(50, combinedLimit);
            }

            // Additional checkbox and input conditions for new characters

            // 2. Inventory Type sets 71st character (index 70) based on selected index
            if (InventoryTypeComboBox.SelectedIndex >= 0)
            {
                fileContent[70] = (char)('1' + InventoryTypeComboBox.SelectedIndex); // Set 71st character (index 70)
            }

            // 3. Price Decimal sets 77th character (index 76)
            if (int.TryParse(PriceDecimalsTextbox.Text, out int priceDecimal) && priceDecimal >= 0 && priceDecimal <= 9)
            {
                fileContent[76] = priceDecimal.ToString()[0]; // Set 77th character (index 76)
            }

            // 4. Unit Decimal sets 80th character (index 79)
            if (int.TryParse(UnitDecimalsTextbox.Text, out int unitDecimal) && unitDecimal >= 0 && unitDecimal <= 9)
            {
                fileContent[79] = unitDecimal.ToString()[0]; // Set 80th character (index 79)
            }

            // 5. Set Length TextBox (formatted as two digits) sets 82nd and 83rd characters (indexes 81 and 82)
            if (int.TryParse(SetSkuLengthTextbox.Text, out int setLength) && setLength >= 0 && setLength <= 99)
            {
                string formattedSetLength = setLength.ToString("D2"); // Format as two digits
                fileContent.Remove(81, 2).Insert(81, formattedSetLength); // Set 82nd and 83rd characters (index 81 and 82)
            }

            // 6. Check Digit DropBox sets 118th character (index 117)
            if (ChooseCheckDigitComboBox.SelectedIndex >= 0)
            {
                fileContent[117] = (char)('1' + ChooseCheckDigitComboBox.SelectedIndex); // Set 118th character (index 117)
            }

            // 7. Price Rounding DropBox sets 127th character (index 126)
            if (PriceRoundingComboBox.SelectedIndex >= 0)
            {
                fileContent[126] = (char)('1' + PriceRoundingComboBox.SelectedIndex); // Set 127th character (index 126)
            }

            // Checkbox logic for various other fields

            // Set Category Discount (72nd char) if UseCategoryDiscountCheckbox is checked
            if (UseCategoryDiscountCheckbox.IsChecked == true)
            {
                fileContent[71] = '1'; // 72nd character (index 71)
            }

            // Set Standard Directories (73rd char) if StandardDirectoriesCheckbox is checked
            if (StandardDirectoriesCheckbox.IsChecked == true)
            {
                fileContent[72] = '1'; // 73rd character (index 72)
            }

            // Set Shelf Totals (74th char) if UseShelfTotalsCheckbox is checked
            if (UseShelfTotalsCheckbox.IsChecked == true)
            {
                fileContent[73] = '1'; // 74th character (index 73)
            }

            // Set Double Key Price (75th char) if DoubleKeyPriceCheckbox is checked
            if (DoubleKeyPriceCheckbox.IsChecked == true)
            {
                fileContent[74] = '1'; // 75th character (index 74)
            }

            // Set Quantity Constant (78th char) if UseQuantityConstantCheckbox is checked
            if (UseQuantityConstantCheckbox.IsChecked == true)
            {
                fileContent[77] = '1'; // 78th character (index 77)
            }

            // Set Numeric SKU (81st char) if NumericSkuOnlyCheckbox is checked
            if (NumericSkuOnlyCheckbox.IsChecked == true)
            {
                fileContent[80] = '1'; // 81st character (index 80)
            }

            // Set Key SKU Twice (119th char) if KeySkuTwiceCheckbox is checked
            if (KeySkuTwiceCheckbox.IsChecked == true)
            {
                fileContent[118] = '1'; // 119th character (index 118)
            }

            // Set Scan SKU Twice (120th char) if ScanSkuTwiceCheckbox is checked
            if (ScanSkuTwiceCheckbox.IsChecked == true)
            {
                fileContent[119] = '1'; // 120th character (index 119)
            }

            // Set Single SKU (122nd char) if UseSingleSkuCheckbox is checked
            if (UseSingleSkuCheckbox.IsChecked == true)
            {
                fileContent[121] = '1'; // 122nd character (index 121)
            }

            // Set SKU Consolidation (123rd char) if UseSkuConsolidationCheckbox is checked
            if (UseSkuConsolidationCheckbox.IsChecked == true)
            {
                fileContent[122] = '1'; // 123rd character (index 122)
            }

            // Set Block Financials (124th char) if BlockFinancialsCheckbox is checked
            if (BlockFinancialsCheckbox.IsChecked == true)
            {
                fileContent[123] = '1'; // 124th character (index 123)
            }

            // Set Lookup SKUs (125th char) if LookupSkusCheckbox is checked
            if (LookupSkusCheckbox.IsChecked == true)
            {
                fileContent[124] = '1'; // 125th character (index 124)
            }

            // Set Lookup Prices (126th char) if LookupPricesCheckbox is checked
            if (LookupPricesCheckbox.IsChecked == true)
            {
                fileContent[125] = '1'; // 126th character (index 125)
            }

            // Set Allow Not Found (187th char) if AllowNotFoundCheckbox is checked
            if (AllowNotFoundCheckbox.IsChecked == true)
            {
                fileContent[186] = '1'; // 187th character (index 186)
            }

            // Set Skip Found Prices > 0 (188th char) if SkipFoundPricesCheckbox is checked
            if (SkipFoundPricesCheckbox.IsChecked == true)
            {
                fileContent[187] = '1'; // 188th character (index 187)
            }

            // Set Expand UPC6 (189th char) if ExpandUpc6Checkbox is checked
            if (ExpandUpc6Checkbox.IsChecked == true)
            {
                fileContent[188] = '1'; // 189th character (index 188)
            }

            if (laserGunOrWandData != null)
            {
                if (laserGunOrWandData.Wand) fileContent[120] = '1'; // 121st character (index 121)
                if (laserGunOrWandData.Standard) fileContent[120] = '2'; 
                if (laserGunOrWandData.Reverse) fileContent[120] = '3'; 
            }

            // Handle Symbology Data from ChooseSymbologyPage
            if (symbologyData != null)
            {
                // UPC, Code 39, I 2 of 5, and other symbologies
                if (symbologyData.UseUpc) fileContent[83] = '1'; // 84th character (index 83)
                if (symbologyData.KeepUpc6) fileContent[84] = '1'; // 85th character (index 84)
                if (symbologyData.I2Of5)
                {
                    fileContent[85] = '1'; // 86th character (index 85)
                    fileContent.Remove(86, 2).Insert(86, int.Parse(symbologyData.I2Of5Min).ToString("D2")); // 87-88
                    fileContent.Remove(88, 2).Insert(88, int.Parse(symbologyData.I2Of5Max).ToString("D2")); // 89-90
                }

                if (symbologyData.Code39)
                {
                    fileContent[90] = '1'; // 91st character (index 90)
                    fileContent.Remove(91, 2).Insert(91, int.Parse(symbologyData.Code39Min).ToString("D2")); // 92-93
                    fileContent.Remove(93, 2).Insert(93, int.Parse(symbologyData.Code39Max).ToString("D2")); // 94-95
                }
              
                if (symbologyData.Code11)
                {
                    fileContent[95] = '1'; // 96th character (index 95)
                    fileContent.Remove(96, 2).Insert(96, int.Parse(symbologyData.Code11Min).ToString("D2")); // 97-98
                    fileContent.Remove(98, 2).Insert(98, int.Parse(symbologyData.Code11Max).ToString("D2")); // 99-100
                }
                
                if (symbologyData.Codebar)
                {
                    fileContent[100] = '1'; // 101st character (index 100)
                    fileContent.Remove(101, 2).Insert(101, int.Parse(symbologyData.CodebarMin).ToString("D2")); // 102-103
                    fileContent.Remove(103, 2).Insert(103, int.Parse(symbologyData.CodebarMax).ToString("D2")); // 104-105
                }
                
                if (symbologyData.Plessey)
                {
                    fileContent[105] = '1'; // 106th character (index 105)
                    fileContent.Remove(106, 2).Insert(106, int.Parse(symbologyData.PlesseyMin).ToString("D2")); // 107-108
                    fileContent.Remove(108, 2).Insert(108, int.Parse(symbologyData.PlesseyMax).ToString("D2")); // 109-110

                }
                

                if (symbologyData.Ean8) fileContent[110] = '1'; // 111th character (index 110)
                if (symbologyData.Ean13) fileContent[111] = '1'; // 112th character (index 111)

                if (symbologyData.Code128)
                {
                    fileContent[112] = '1'; // 113th character (index 112)
                    fileContent.Remove(113, 2).Insert(113, int.Parse(symbologyData.Code128Min).ToString("D2")); // 114-115
                    fileContent.Remove(115, 2).Insert(115, int.Parse(symbologyData.Code128Max).ToString("D2")); // 116-117
                }
                
            }

            // Write the content to the specified HHConfigData.dlf file path
            File.WriteAllText(@"C:\\Lostakes Data\\HHConfigData.dlf", fileContent.ToString());
        }


        // Event handler for Use Dollar Limits checkbox
        private void UseDollarLimitsCheckbox_Clicked(object sender, RoutedEventArgs e)
        {
            if (UseDollarLimitsCheckbox.IsChecked == true)
            {
                // Show the Dollar Limits window only when checked
                DollarLimitsWindow dollarLimitsWindow = new DollarLimitsWindow();
                bool? result = dollarLimitsWindow.ShowDialog();

                // Update the limits only if the window was opened and the user confirmed
                if (result == true)
                {
                    // If the user clicks OK, retrieve the values
                    entryLimit = dollarLimitsWindow.EntryLimit.PadLeft(7, '0'); // Ensure 7 digits
                    priceLimit = dollarLimitsWindow.PriceLimit.PadLeft(4, '0'); // Ensure 4 digits
                    qtyLimit = dollarLimitsWindow.QtyLimit.PadLeft(9, '0'); // Ensure 9 digits
                    combinedLimit = dollarLimitsWindow.CombinedLimit.PadLeft(9, '0'); // Ensure 9 digits
                }
            }
            // If the checkbox is unchecked, do not modify the dollar limits
            else
            {
                // The checkbox is unchecked, but do nothing to alter the current limits
                // (The existing limits remain unchanged in the file until the window is opened)
            }
        }


        private void LoadConfigFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            // Read the content from the file
            string fileContent = File.ReadAllText(filePath);

            // Ensure file length is as expected
            if (fileContent.Length < 189) return; // Minimum expected length, handle appropriately if not met

            // Initialize symbologyData if it's null
            if (symbologyData == null)
            {
                symbologyData = new SymbologyData();
            }

            // Initialize laserGunOrWandData if it's null
            if (laserGunOrWandData == null)
            {
                laserGunOrWandData = new LaserGunOrWandData();
            }

            // Extract values using Substring
            entryLimit = fileContent.Substring(26, 7);  // Entry Limit from characters 27-33 (index 26-32)
            priceLimit = fileContent.Substring(35, 4);  // Price Limit from characters 36-39 (index 35-38)
            qtyLimit = fileContent.Substring(41, 9);    // Quantity Limit from characters 42-50 (index 41-49)
            combinedLimit = fileContent.Substring(50, 9);  // Combined Limit from characters 51-59 (index 50-58)


            UseDollarLimitsCheckbox.IsChecked = fileContent[25] == '1'; //26th character
            LookupSkusCheckbox.IsChecked = fileContent[124] == '1'; // 125th character
            LookupPricesCheckbox.IsChecked = fileContent[125] == '1'; // 126th character
            AllowNotFoundCheckbox.IsChecked = fileContent[186] == '1'; // 187th character
            SkipFoundPricesCheckbox.IsChecked = fileContent[187] == '1'; // 188th character
            ExpandUpc6Checkbox.IsChecked = fileContent[188] == '1'; // 189th character
            UseCategoryDiscountCheckbox.IsChecked = fileContent[71] == '1'; // 72nd character
            StandardDirectoriesCheckbox.IsChecked = fileContent[72] == '1'; // 73rd character
            UseShelfTotalsCheckbox.IsChecked = fileContent[73] == '1'; // 74th character
            DoubleKeyPriceCheckbox.IsChecked = fileContent[74] == '1'; // 75th character
            UseQuantityConstantCheckbox.IsChecked = fileContent[77] == '1'; // 78th character
            NumericSkuOnlyCheckbox.IsChecked = fileContent[80] == '1'; // 81st character
            KeySkuTwiceCheckbox.IsChecked = fileContent[118] == '1'; // 119th character
            ScanSkuTwiceCheckbox.IsChecked = fileContent[119] == '1'; // 120th character
            UseSingleSkuCheckbox.IsChecked = fileContent[121] == '1'; // 122nd character
            UseSkuConsolidationCheckbox.IsChecked = fileContent[122] == '1'; // 123rd character
            BlockFinancialsCheckbox.IsChecked = fileContent[123] == '1'; // 124th character

            // Update textboxes
            SetSkuLengthTextbox.Text = fileContent.Substring(81, 2); // 82nd-83rd characters for SKU length
            PriceDecimalsTextbox.Text = fileContent[76].ToString(); // 77th character for price decimals
            UnitDecimalsTextbox.Text = fileContent[79].ToString(); // 80th character for unit decimals

            // Update comboboxes
            InventoryTypeComboBox.SelectedIndex = fileContent[70] - '1'; // 71st character (0-based index)
            ChooseCheckDigitComboBox.SelectedIndex = fileContent[117] - '1'; // 118th character (0-based index)
            PriceRoundingComboBox.SelectedIndex = fileContent[126] - '1'; // 127th character (0-based index)


            // Symbology data
            if (fileContent[83] == '1') symbologyData.UseUpc = true; // 84th character
            if (fileContent[83] == '1') ChooseSymbologiesCheckbox.IsChecked = true; // 84th character
            if (fileContent[84] == '1') symbologyData.KeepUpc6 = true; // 85th character
            if (fileContent[85] == '1') symbologyData.I2Of5 = true; // 86th character
            if (fileContent[90] == '1') symbologyData.Code39 = true; // 91st character
            if (fileContent[95] == '1') symbologyData.Code11 = true; // 96th character
            if (fileContent[100] == '1') symbologyData.Codebar = true; // 101st character
            if (fileContent[105] == '1') symbologyData.Plessey = true; // 106th character
            if (fileContent[110] == '1') symbologyData.Ean8 = true; // 111th character
            if (fileContent[111] == '1') symbologyData.Ean13 = true; // 112th character
            if (fileContent[112] == '1') symbologyData.Code128 = true; // 113th character


            // Laser Gun or Wand data
            if (fileContent[120] == '1') laserGunOrWandData.Wand = true; // 121st character
            if (fileContent[120] == '1') ChooseLaserGunOrWandCheckbox.IsChecked = true; // 121st character
            if (fileContent[120] == '2') laserGunOrWandData.Standard = true;
            if (fileContent[120] == '3') laserGunOrWandData.Reverse = true;
        }


        private void LoadSettings()
        {
            // Call this method when loading the file, for example when the user clicks a "Load" button
            LoadConfigFromFile(@"C:\\Lostakes Data\\HHConfigData.dlf");
        }

        private void InventoryTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if "Financial" is selected
            if (InventoryTypeComboBox.SelectedItem is ComboBoxItem selectedItem &&
                selectedItem.Content.ToString() == "Financial")
            {
                // Disable all options in the SKU Options groupbox
                SetSkuOptionsEnabled(false);
                LookupPricesCheckbox.IsEnabled = false;
                AllowNotFoundCheckbox.IsEnabled = false;
                SkipFoundPricesCheckbox.IsEnabled = false;
            }
            else
            {
                // Enable all options in the SKU Options groupbox
                SetSkuOptionsEnabled(true);
            }
  
        }
        private void SetSkuOptionsEnabled(bool isEnabled)
        {
            // Disable or enable all checkboxes in the SKU Options groupbox
            NumericSkuOnlyCheckbox.IsEnabled = isEnabled;
            RestrictSkuLengthCheckbox.IsEnabled = isEnabled;
            SetSkuLengthTextbox.IsEnabled = isEnabled && RestrictSkuLengthCheckbox.IsChecked == true;
            ChooseSymbologiesCheckbox.IsEnabled = isEnabled;
            ChooseCheckDigitComboBox.IsEnabled = isEnabled;
            KeySkuTwiceCheckbox.IsEnabled = isEnabled;
            ScanSkuTwiceCheckbox.IsEnabled = isEnabled;
            ChooseLaserGunOrWandCheckbox.IsEnabled = isEnabled;
            UseSingleSkuCheckbox.IsEnabled = isEnabled;
            UseSkuConsolidationCheckbox.IsEnabled = isEnabled;
            BlockFinancialsCheckbox.IsEnabled = isEnabled;
            LookupSkusCheckbox.IsEnabled = isEnabled;
            ExpandUpc6Checkbox.IsEnabled = isEnabled;
            LookupPricesCheckbox.IsEnabled = true;
            AllowNotFoundCheckbox.IsEnabled = true;
            SkipFoundPricesCheckbox.IsEnabled = true;
        }


    }
}
