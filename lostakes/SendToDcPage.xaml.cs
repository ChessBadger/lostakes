using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;  // For FolderBrowserDialog
using DbfDataReader;
using MessageBox = System.Windows.Forms.MessageBox;   // For DBF file processing

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

            // Define paths to the existing DLF files
            string areaDlfPath = @"C:\Lostakes Data\area.dlf";
            string categoryDlfPath = @"C:\Lostakes Data\category.dlf";
            string locationDlfPath = @"C:\Lostakes Data\location.dlf";

            // Define paths to the output DLF files
            string areaOutputDlfPath = @"C:\Lostakes Data\area_output.dlf";
            string categoryOutputDlfPath = @"C:\Lostakes Data\category_output.dlf";
            string locationOutputDlfPath = @"C:\Lostakes Data\location_output.dlf";

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

                        // Notify the user that the process is complete
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

            MessageBox.Show("Cards Created!");
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
    }
}
