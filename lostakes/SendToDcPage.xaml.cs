using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;  // For FolderBrowserDialog
using DbfDataReader;
using MessageBox = System.Windows.MessageBox;  // Use WPF MessageBox

namespace lostakes
{
    public partial class SendToDcPage : Page
    {
        public SendToDcPage()
        {
            InitializeComponent();
        }

        private void CreateCardsButton_Click(object sender, RoutedEventArgs e)
        {
            // Define paths to the DBF files
            string areaDbfPath = @"C:\Wintakes\Data\area.dbf";
            string categoryDbfPath = @"C:\Wintakes\Data\category.dbf";
            string locationDbfPath = @"C:\Wintakes\Data\location.dbf";
            string itemastDbfPath = @"C:\Wintakes\Data\itemast.dbf"; // Add itemast.dbf path

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
            string hhConfigOutputDlfPath = @"C:\Lostakes Data\HHConfigData.dlf"; // HHConfigData.dlf as output

            string lostakesDataPath = @"C:\Lostakes Data";
            string[] filesToCopy = { "auto.ini", "DC5.exe", "Scanners.ini", "Scard.don" };

            try
            {
                // Ask the user to select a folder where the combined DLF files should be saved
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select Folder to Save Card Setup";
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get the selected folder path
                        string selectedFolder = folderDialog.SelectedPath;

                        // Process and combine area.dlf
                        CombineDlfFiles(areaDlfPath, areaOutputDlfPath, selectedFolder, "Area");

                        // Process and combine category.dlf
                        CombineDlfFiles(categoryDlfPath, categoryOutputDlfPath, selectedFolder, "Category");

                        // Process and combine location.dlf
                        CombineDlfFiles(locationDlfPath, locationOutputDlfPath, selectedFolder, "Location");

                        // Process and combine HHConfig.dlf
                        CombineDlfFiles(hhConfigDlfPath, hhConfigDataDlfPath, selectedFolder, "HHConfig");

                        // Copy the specified files from C:\Lostakes Data to the selected folder
                        foreach (var fileName in filesToCopy)
                        {
                            string sourceFilePath = Path.Combine(lostakesDataPath, fileName);
                            string destinationFilePath = Path.Combine(selectedFolder, fileName);

                            if (File.Exists(sourceFilePath))
                            {
                                File.Copy(sourceFilePath, destinationFilePath, overwrite: true);
                            }
                            else
                            {
                                MessageBox.Show($"File not found: {sourceFilePath}");
                            }
                        }

                        // Now process itemast.dbf and generate output.dlf
                        // We'll define the output file path in the selected folder
                        string outputDlfPath = Path.Combine(selectedFolder, "SKUFILE.DLF");

                        // Step 1: Read the DBF file and extract SKU and Price
                        List<Record> records = new List<Record>();
                        using (var dbfTable = new DbfTable(itemastDbfPath))
                        {
                            var dbfRecord = new DbfRecord(dbfTable);
                            while (dbfTable.Read(dbfRecord))
                            {
                                var skuValue = dbfRecord.Values[0];
                                var priceValue = dbfRecord.Values[10];

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

                        // Step 2: Sort records Z to A by SKU
                        records = records.OrderByDescending(r => r.SKU).ToList();

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

                        // Step 4: Prepare the DLF data
                        List<DlfRow> dlfRows = new List<DlfRow>();

                        // First row special case
                        if (records.Count >= 4)
                        {
                            DlfRow firstRow = new DlfRow();
                            string firstRowSKU = "99999999999999999";
                            firstRow.Col1 = PadSku(firstRowSKU);

                            // Col2
                            string col2Value = GetPriceString(0, maxIntegerDigits) + PadSku(records[0].SKU);
                            firstRow.Col2 = col2Value;

                            // Col3
                            string col3Value = records[0].PriceString + PadSku(records[1].SKU);
                            firstRow.Col3 = col3Value;

                            // Col4
                            string col4Value = records[1].PriceString + PadSku(records[2].SKU);
                            firstRow.Col4 = col4Value;

                            // Col5
                            string col5Value = records[2].PriceString + PadSku(records[3].SKU);
                            firstRow.Col5 = col5Value;

                            // Col6
                            firstRow.Col6 = records[3].PriceString;

                            dlfRows.Add(firstRow);
                        }
                        else
                        {
                            MessageBox.Show("Not enough records to process.");
                            return;
                        }

                        // Process remaining records in groups of 5
                        for (int i = 4; i < records.Count; i += 5)
                        {
                            DlfRow row = new DlfRow();
                            int remaining = records.Count - i;

                            // Col1
                            row.Col1 = PadSku(records[i].SKU);

                            // Col2
                            if (remaining >= 2)
                            {
                                string col2Value = records[i].PriceString + PadSku(records[i + 1].SKU);
                                row.Col2 = col2Value;
                            }
                            else if (remaining == 1)
                            {
                                row.Col2 = records[i].PriceString;
                            }

                            // Col3
                            if (remaining >= 3)
                            {
                                string col3Value = records[i + 1].PriceString + PadSku(records[i + 2].SKU);
                                row.Col3 = col3Value;
                            }
                            else if (remaining == 2)
                            {
                                row.Col3 = records[i + 1].PriceString;
                            }

                            // Col4
                            if (remaining >= 4)
                            {
                                string col4Value = records[i + 2].PriceString + PadSku(records[i + 3].SKU);
                                row.Col4 = col4Value;
                            }
                            else if (remaining == 3)
                            {
                                row.Col4 = records[i + 2].PriceString;
                            }

                            // Col5
                            if (remaining >= 5)
                            {
                                string col5Value = records[i + 3].PriceString + PadSku(records[i + 4].SKU);
                                row.Col5 = col5Value;
                            }
                            else if (remaining == 4)
                            {
                                row.Col5 = records[i + 3].PriceString;
                            }

                            // Col6
                            if (remaining >= 5)
                                row.Col6 = records[i + 4].PriceString;
                            else if (remaining == 4)
                                row.Col6 = records[i + 3].PriceString;
                            else if (remaining == 3)
                                row.Col6 = records[i + 2].PriceString;
                            else if (remaining == 2)
                                row.Col6 = records[i + 1].PriceString;
                            else if (remaining == 1)
                                row.Col6 = records[i].PriceString;
                            else
                                row.Col6 = "";

                            dlfRows.Add(row);
                        }

                        // Step 5: Sort DLF rows A to Z by SKU (Col1), excluding the first row
                        var sortedDlfRows = dlfRows.Skip(1).OrderBy(r => r.Col1.Trim()).ToList();

                        // Append the first row at the end
                        sortedDlfRows.Add(dlfRows[0]);

                        // Step 6: Write to DLF file

                        // Prepare the header lines with dynamic priceLength
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

                        // Notify the user that the process is complete
                        MessageBox.Show("Cards Created!");
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
            // Navigate to the DC Setup Page
            NavigationService.Navigate(new DcSetupPage());
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
