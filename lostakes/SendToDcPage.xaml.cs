using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;  // For FolderBrowserDialog
using DbfDataReader;
using static lostakes.DcSetupPage;
using MessageBox = System.Windows.MessageBox;  // Use WPF MessageBox

namespace lostakes
{
    public partial class SendToDcPage : Page
    {


        public SendToDcPage()
        {
            InitializeComponent();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Window.GetWindow(this).Close();
        }


        private void CreateCardsButton_Click(object sender, RoutedEventArgs e)
        {
            // Define paths to the DBF files
            string areaDbfPath = @"C:\Wintakes\Data\area.dbf";
            string categoryDbfPath = @"C:\Wintakes\Data\category.dbf";
            string locationDbfPath = @"C:\Wintakes\Data\location.dbf";
            string itemastDbfPath = @"C:\Wintakes\Data\itemast.dbf"; // itemast.dbf path

            // Define paths to the existing DLF files
            string areaDlfPath = @"C:\Lostakes Data\area.dlf";
            string categoryDlfPath = @"C:\Lostakes Data\category.dlf";
            string locationDlfPath = @"C:\Lostakes Data\location.dlf";
            string hhConfigDlfPath = @"C:\Lostakes Data\HHConfig.dlf";
            string hhConfigDataDlfPath = @"C:\Lostakes Data\HHConfigData.dlf";

            // Define paths to the output DLF files
            string areaOutputDlfPath = @"C:\Lostakes Data\area_output.dlf";
            string categoryOutputDlfPath = @"C:\Lostakes Data\category_output.dlf";
            string locationOutputDlfPath = @"C:\Lostakes Data\location_output.dlf";
            string dataPath = @"C:\Wintakes\Data";

            // Paths of the files to copy as-is
            string lostakesDataPath = @"C:\Lostakes Data";
            string[] filesToCopy = { "auto.ini", "DC5.exe", "Scanners.ini", "Scard.don" };

            try
            {
                if (File.Exists(areaDbfPath))
                {
                    // Process area.dbf to generate area_output.dlf
                    GenerateOutputDlfFromDbf(areaDbfPath, areaOutputDlfPath, 2, 21, "area");

                    // Process category.dbf to generate category_output.dlf
                    GenerateOutputDlfFromDbf(categoryDbfPath, categoryOutputDlfPath, 2, 21, "category");

                    // Process location.dbf to generate location_output.dlf
                    GenerateOutputDlfFromDbf(locationDbfPath, locationOutputDlfPath, 3, 26, "location");
                }
              

                // Ask the user to select a folder where the combined DLF files should be saved
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select Folder to Save Card Setup";
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the selected folder path
                        string selectedFolder = folderDialog.SelectedPath;

                        if (File.Exists(areaDbfPath))
                        {
                            // Process and combine area.dlf
                            CombineDlfFiles(areaDlfPath, areaOutputDlfPath, selectedFolder, "Area");
                            CombineDlfFiles(areaDlfPath, areaOutputDlfPath, dataPath, "Area");

                            // Process and combine category.dlf
                            CombineDlfFiles(categoryDlfPath, categoryOutputDlfPath, selectedFolder, "Category");
                            CombineDlfFiles(categoryDlfPath, categoryOutputDlfPath, dataPath, "Category");

                            // Process and combine location.dlf
                            CombineDlfFiles(locationDlfPath, locationOutputDlfPath, selectedFolder, "Location");
                            CombineDlfFiles(locationDlfPath, locationOutputDlfPath, dataPath, "Location");
                        }

                        // Process and combine HHConfig.dlf
                        CombineDlfFiles(hhConfigDlfPath, hhConfigDataDlfPath, selectedFolder, "HHConfig");
                        CombineDlfFiles(hhConfigDlfPath, hhConfigDataDlfPath, dataPath, "HHConfig");

                        // Copy the specified files from C:\Lostakes Data to the selected folder
                        foreach (var fileName in filesToCopy)
                        {
                            string sourceFilePath = Path.Combine(lostakesDataPath, fileName);
                            string destinationFilePath = Path.Combine(selectedFolder, fileName);

                            try
                            {
                                if (File.Exists(sourceFilePath))
                                {
                                    File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                                }
                                else
                                {
                                    // Display which file wasn't found
                                    MessageBox.Show($"File not found: {sourceFilePath}");
                                }
                            }
                            catch (Exception fileCopyEx)
                            {
                                MessageBox.Show($"An error occurred while copying the file '{fileName}': {fileCopyEx.Message}");
                            }
                        }

                        string itemastPath = Path.Combine(@"C:\Wintakes\Data\itemast.dbf");

                        if (File.Exists(itemastPath))
                        {
                            // Now process itemast.dbf and generate output.dlf
                            string outputDlfPath = Path.Combine(selectedFolder, "SKUFILE.DLF");
                            string outputDataPath = Path.Combine(dataPath, "SKUFILE.DLF");
                            // Process itemast.dbf to generate SKUFILE.DLF
                            GenerateOutputDlfFromItemast(itemastDbfPath, outputDlfPath);
                            GenerateOutputDlfFromItemast(itemastDbfPath, outputDataPath);
                        }

                        // Notify the user that the process is complete
                        MessageBox.Show("Cards Created!");
                        // Clean up non-default files
                        DeleteNonDefaultFiles(areaOutputDlfPath, categoryOutputDlfPath, locationOutputDlfPath);
                    }
                    else
                    {
                        // If the user cancels folder selection
                        MessageBox.Show("No folder selected. Operation cancelled.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and show the error message
                MessageBox.Show($"An error occurred: {ex.Message}");
            }


            ClearBackup clearBackupPage = new ClearBackup();
            clearBackupPage.RunBackup();
        }

        private void DeleteNonDefaultFiles(string areaOutputDlfPath, string categoryOutputDlfPath, string locationOutputDlfPath)
        {
            // After the files have been copied to the selected folder, delete the temporary output files
            try
            {
                // Delete areaOutputDlfPath
                if (File.Exists(areaOutputDlfPath))
                {
                    File.Delete(areaOutputDlfPath);
                }

                // Delete categoryOutputDlfPath
                if (File.Exists(categoryOutputDlfPath))
                {
                    File.Delete(categoryOutputDlfPath);
                }

                // Delete locationOutputDlfPath
                if (File.Exists(locationOutputDlfPath))
                {
                    File.Delete(locationOutputDlfPath);
                }

                // Delete skufile.dbf from C:\Lostakes Data
                string skuFileDbfPath = @"C:\Lostakes Data\skufile.dbf";
                if (File.Exists(skuFileDbfPath))
                {
                    File.Delete(skuFileDbfPath);
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the file deletion process
                MessageBox.Show($"An error occurred while deleting files: {ex.Message}");
            }

        }

        // Generate the _output.dlf file from the dbf file, with sorting, padding, and adjustments
        private void GenerateOutputDlfFromDbf(string dbfPath, string outputDlfPath, int numberOfColumns, int fixedWidth, string fileType)
        {
            List<(string Key, string Line)> records = new List<(string, string)>();

            using (var dbfTable = new DbfTable(dbfPath))
            {
                var dbfRecord = new DbfRecord(dbfTable);
                while (dbfTable.Read(dbfRecord))
                {
                    List<string> columns = new List<string>();
                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        var value = dbfRecord.Values[i]?.ToString().Trim() ?? "";
                        columns.Add(value);
                    }
                    string line = string.Join("", columns);

                    // Adjust lines for category, area, or location
                    if (fileType == "category")
                    {
                        // Ensure the line has at least 21 characters and set the 20th and 21st characters to "0"
                        line = line.PadRight(21);
                        line = line.Remove(19, 2).Insert(19, "00");
                    }
                    else
                    {
                        // For area and location, just pad the lines
                        line = line.PadRight(fixedWidth);
                    }

                    string key = columns[0]; // Sorting by the first column
                    records.Add((key, line));
                }
            }

            // Sort the records by Key
            records = records.OrderBy(r => r.Key).ToList();

            // Write the sorted records to the output file
            using (var writer = new StreamWriter(outputDlfPath))
            {
                foreach (var record in records)
                {
                    writer.WriteLine(record.Line); // Lines already padded
                }
            }
        }


        // Combines the original .dlf file with the new output file and saves it to the selected folder
        private void CombineDlfFiles(string originalDlfPath, string outputDlfPath, string destinationFolder, string fileType)
        {
            // Path to save the combined file in the selected folder
            string combinedFilePath = Path.Combine(destinationFolder, $"{fileType.ToUpper()}.dlf");

            using (var writer = new StreamWriter(combinedFilePath))
            {
                // Write contents of the original .dlf file
                using (var originalReader = new StreamReader(originalDlfPath))
                {
                    string line;
                    while ((line = originalReader.ReadLine()) != null)
                    {
                        writer.WriteLine(line);
                    }
                }

                // Write contents of the output .dlf file (append to the original)
                using (var outputReader = new StreamReader(outputDlfPath))
                {
                    string line;
                    while ((line = outputReader.ReadLine()) != null)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private void DcSetupButton_Click(object sender, RoutedEventArgs e)
        {
            // Define the path to the Accounts folder
            string accountsFolderPath = @"C:\Lostakes Data\Accounts";

            // Check if the folder exists and contains any files
            if (Directory.Exists(accountsFolderPath) && Directory.GetFiles(accountsFolderPath).Any())
            {
                // If there are account files, navigate to the Accounts page
                NavigationService.Navigate(new Accounts());
            }
            else
            {
                // Otherwise, navigate to the DC Setup Page
                NavigationService.Navigate(new DcSetupPage());
            }
        }


        private void GenerateOutputDlfFromItemast(string itemastDbfPath, string outputDlfPath)
        {

            // Step 1: Read the DBF file and extract SKU and Price
            List<Record> records = new List<Record>();
            using (var dbfTable = new DbfTable(itemastDbfPath))
            {
                var dbfRecord = new DbfRecord(dbfTable);
                while (dbfTable.Read(dbfRecord))
                {
                    var skuValue = dbfRecord.Values[0];
                    var priceValue =  dbfRecord.Values[10];

                    if (skuValue != null && priceValue != null)
                    {
                        string sku = skuValue.ToString().Trim();
                        string priceStr = priceValue.ToString();

                        if (decimal.TryParse(priceStr, out decimal price))
                        {
                            records.Add(new Record { SKU = sku, Price = price });
                        }
                    }
                }
            }

            // Step 2: Sort records Z to A by SKU, without ignoring leading zeros, and with symbols after numbers but before letters
            records = records.OrderByDescending(r => r.SKU, StringComparer.Ordinal).ToList();

            // Step 3: Determine the maximum integer part length of the price
            decimal maxPrice = records.Max(r => r.Price);
            int maxIntegerPart = (int)Math.Floor(maxPrice); // Get the integer part of the max price
            int maxIntegerDigits = maxIntegerPart.ToString().Length;

            // Calculate the price length for the DLF (maxIntegerDigits + 4)
            int priceLength = maxIntegerDigits + 4;

            // Prepare the prices according to the new format
            foreach (var record in records)
            {
                record.PriceString = GetPriceString(record.Price, maxIntegerDigits);
            }

            // Step 3.1: Add placeholder records if needed to make the count a multiple of 5
            int remainder = records.Count % 5;
            if (remainder != 0)
            {
                int placeholdersToAdd = 5 - remainder; // How many placeholders we need
                for (int i = 0; i < placeholdersToAdd; i++)
                {
                    records.Add(new Record { SKU = "99999999999999999", Price = 0, PriceString = GetPriceString(0, maxIntegerDigits) });
                }
            }

            // Step 4: Prepare the DLF data
            List<DlfRow> dlfRows = new List<DlfRow>();

            // Step 5: Process records in groups of 5
            for (int i = 0; i < records.Count; i += 5)
            {
                DlfRow row = new DlfRow();

                // Col1 (first SKU)
                row.Col1 = PadSku(records[i].SKU);

                // Col2 (price of first SKU + second SKU)
                if (i + 1 < records.Count)
                {
                    row.Col2 = records[i].PriceString + PadSku(records[i + 1].SKU);
                }

                // Col3 (price of second SKU + third SKU)
                if (i + 2 < records.Count)
                {
                    row.Col3 = records[i + 1].PriceString + PadSku(records[i + 2].SKU);
                }

                // Col4 (price of third SKU + fourth SKU)
                if (i + 3 < records.Count)
                {
                    row.Col4 = records[i + 2].PriceString + PadSku(records[i + 3].SKU);
                }

                // Col5 (price of fourth SKU + fifth SKU)
                if (i + 4 < records.Count)
                {
                    row.Col5 = records[i + 3].PriceString + PadSku(records[i + 4].SKU);
                }

                // Col6 (price of fifth SKU)
                if (i + 4 < records.Count)
                {
                    row.Col6 = records[i + 4].PriceString;
                }

                dlfRows.Add(row);
            }

            // Step 6: Sort DLF rows A to Z by SKU (Col1), excluding the first row
            var sortedDlfRows = dlfRows.Skip(1).OrderBy(r => r.Col1.Trim()).ToList();

            // Append the first row at the end
            sortedDlfRows.Add(dlfRows[0]);

            // Step 7: Write to DLF file
            List<string> headerLines = new List<string>
    {
        "0,0,0",
        "17,15,0",
        $"{priceLength},14,0",
        "17,15,0",
        $"{priceLength},14,0",
        "17,15,0",
        $"{priceLength},14,0",
        "17,15,0",
        $"{priceLength},14,0",
        "17,15,0",
        $"{priceLength},14,0",
        "0,0,0"
    };

            using (StreamWriter writer = new StreamWriter(outputDlfPath))
            {
                // Write header lines
                foreach (var line in headerLines)
                {
                    writer.WriteLine(line);
                }

                // Write DLF data
                foreach (var row in sortedDlfRows)
                {
                    writer.WriteLine($"{row.Col1}{row.Col2}{row.Col3}{row.Col4}{row.Col5}{row.Col6}");
                }
            }
        }


        // Helper method to format the price string
        static string GetPriceString(decimal price, int maxIntegerDigits)
        {
            // Extract integer and decimal parts
            int integerPart = (int)Math.Floor(price);

            // Extract hundredths (cents), ensuring proper rounding
            int hundredths = (int)Math.Round((price - integerPart) * 100);

            // Ensure hundredths are between 0 and 99
            if (hundredths == 100)
            {
                integerPart += 1;
                hundredths = 0;
            }

            // Construct the price string with leading zero, padded integer part, hundredths, and a trailing zero
            string priceStr = "0" +
                              integerPart.ToString().PadLeft(maxIntegerDigits, '0') +
                              hundredths.ToString("D2") +
                              "0"; // Append "0" at the end

            return priceStr;
        }

        // Helper method to pad the SKU
        static string PadSku(string sku)
        {
            return sku.PadRight(17);
        }

        // Record class to hold SKU and Price information
        class Record
        {
            public string SKU { get; set; }
            public decimal Price { get; set; }
            public string PriceString { get; set; }
        }

        // DlfRow class to hold the columns for DLF file
        class DlfRow
        {
            public string Col1 { get; set; } = "";
            public string Col2 { get; set; } = "";
            public string Col3 { get; set; } = "";
            public string Col4 { get; set; } = "";
            public string Col5 { get; set; } = "";
            public string Col6 { get; set; } = "";
        }
    }
}
