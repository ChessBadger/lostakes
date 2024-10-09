using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // For the SaveFileDialog

namespace lostakes
{
    /// <summary>
    /// Interaction logic for ClearBackup.xaml
    /// </summary>
    public partial class ClearBackup : Page
    {
        private readonly string directoryPath = @"C:\Wintakes\Data";

        public ClearBackup()
        {
            InitializeComponent();
        }


        private readonly string zipFilenameFile = @"C:\Lostakes Data\lastZipFilename.txt"; // Only the filename is persisted

        public void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            RunBackup();
        }
        public void RunBackup()
        {
            try
            {
                // Ensure the Lostakes Data directory exists
                string lostakesDataDirectory = Path.GetDirectoryName(zipFilenameFile);
                if (!Directory.Exists(lostakesDataDirectory))
                {
                    Directory.CreateDirectory(lostakesDataDirectory);
                }

                // Load the last used zip filename from the text file
                string lastZipFilename = LoadLastZipFilename();
                string defaultFilename = string.IsNullOrEmpty(lastZipFilename) ? "WintakesBackup.zip" : lastZipFilename;

                // Show the SaveFileDialog to let the user choose the location and filename
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Zip Files (*.zip)|*.zip",
                    Title = "Choose where to save the backup",
                    FileName = defaultFilename // Pre-fill with the last saved file name or default
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string chosenZipFilePath = saveFileDialog.FileName; // Full path of the chosen file
                    string chosenZipFilename = Path.GetFileName(chosenZipFilePath); // Just the filename

                    // Save the selected filename to the text file (not the full path)
                    SaveLastZipFilename(chosenZipFilename);

                    // Continue with backup process using the chosen path
                    CreateBackup(chosenZipFilePath);
                }
                else
                {
                    MessageBox.Show("Backup canceled.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during backup: {ex.Message}");
            }
        }

        /// Method to perform the actual backup using the chosen zip file path.
        private void CreateBackup(string zipFilePath)
        {
            string wintakesFolderPath = @"C:\Wintakes";
            string dataFolderPath = Path.Combine(wintakesFolderPath, "Data");
            string backupFolderPath = Path.Combine(wintakesFolderPath, "Backup");

            // Ensure that both Data and Backup directories exist
            if (!Directory.Exists(dataFolderPath) || !Directory.Exists(backupFolderPath))
            {
                MessageBox.Show("Both Data and Backup directories must exist for backup.");
                return;
            }

            // Temporary directory to copy both folders under a single "Wintakes" folder structure
            string tempDirectory = Path.Combine(Path.GetTempPath(), "WintakesBackup");

            // Create Wintakes directory inside temp
            string tempWintakesFolder = Path.Combine(tempDirectory, "Wintakes");
            Directory.CreateDirectory(tempWintakesFolder);

            // Copy Data folder into Wintakes
            string tempDataFolder = Path.Combine(tempWintakesFolder, "Data");
            CopyDirectory(dataFolderPath, tempDataFolder);

            // Copy Backup folder into Wintakes
            string tempBackupFolder = Path.Combine(tempWintakesFolder, "Backup");
            CopyDirectory(backupFolderPath, tempBackupFolder);

            // Create the zip file
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath); // If a zip file already exists, delete it
            }

            // Zip the entire "Wintakes" folder structure
            ZipFile.CreateFromDirectory(tempDirectory, zipFilePath);

            // Clean up temporary directory
            Directory.Delete(tempDirectory, true);

            MessageBox.Show($"Backup successful!");
        }


        /// Helper method to save the last used zip file name to a text file.
        private void SaveLastZipFilename(string zipFilename)
        {
            File.WriteAllText(zipFilenameFile, zipFilename);
        }

        private string LoadLastZipFilename()
        {
            if (File.Exists(zipFilenameFile))
            {
                return File.ReadAllText(zipFilenameFile);
            }
            return null;
        }



        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Create the destination directory if it doesn't exist
            Directory.CreateDirectory(destinationDir);

            // Copy all files from the source to the destination
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            // Recursively copy subdirectories
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, destSubDir);
            }
        }

        private void ClearUploadsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var files = Directory.GetFiles(directoryPath, "*upload*");
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                MessageBox.Show("All upload files have been deleted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ClearDirectoriesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] dbfFiles = { "area.dbf", "location.dbf", "category.dbf" };
                foreach (string dbfFile in dbfFiles)
                {
                    string filePath = Path.Combine(directoryPath, dbfFile);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                MessageBox.Show("Selected directory files have been deleted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ClearItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string itemFile = Path.Combine(directoryPath, "itemast.dbf");
                if (File.Exists(itemFile))
                {
                    File.Delete(itemFile);
                }
                MessageBox.Show("Item master file has been deleted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void ClearEverythingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get all files in the directory
                var files = Directory.GetFiles(directoryPath);

                // Delete each file
                foreach (var file in files)
                {
                    File.Delete(file);
                }

                MessageBox.Show("All files in the directory have been deleted.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of MainWindow
            MainWindow mainWindow = new MainWindow();

            // Show the MainWindow
            mainWindow.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }

    }
}
