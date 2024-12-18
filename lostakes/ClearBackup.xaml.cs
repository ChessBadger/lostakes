﻿using System;
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
        private readonly string backupPath = @"C:\Wintakes\Backup";

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
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all upload files?",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                BackupBeforeClear();
                try
                {
                    var files = Directory.GetFiles(directoryPath, "*UPLD*");
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
            else
            {
                MessageBox.Show("Operation canceled.");
            }
        }

        private void ClearDirectoriesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear selected directory files?",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                BackupBeforeClear();
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
            else
            {
                MessageBox.Show("Operation canceled.");
            }
        }

        private void ClearItemButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the item master file?",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                BackupBeforeClear();
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
            else
            {
                MessageBox.Show("Operation canceled.");
            }
        }

        private void ClearEverythingButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all files?",
                                                      "Confirmation",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                BackupBeforeClear();
                try
                {
                    var files = Directory.GetFiles(directoryPath);
                    foreach (var file in files)
                    {
                        File.Delete(file);
                    }
                    MessageBox.Show("Files have been cleared!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Operation canceled.");
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

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open a dialog for the user to select the zip file
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Zip Files (*.zip)|*.zip",
                    Title = "Select the backup zip file"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string zipFilePath = openFileDialog.FileName;

                    // Define the paths where the content should be extracted
                    string extractPath = Path.Combine(Path.GetTempPath(), "WintakesRestore");
                    string dataExtractPath = @"C:\Wintakes\Data";
                    string backupExtractPath = @"C:\Wintakes\Backup";

                    // Extract the zip file to a temporary directory
                    ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                    // Check if the extracted folder contains 'Wintakes', 'Data', and 'Backup' directories
                    string wintakesFolder = Path.Combine(extractPath, "Wintakes");
                    string extractedDataFolder = Path.Combine(wintakesFolder, "Data");
                    string extractedBackupFolder = Path.Combine(wintakesFolder, "Backup");

                    if (!Directory.Exists(wintakesFolder) || !Directory.Exists(extractedDataFolder) || !Directory.Exists(extractedBackupFolder))
                    {
                        MessageBox.Show("The selected zip file is not correctly formatted. Please try another file.");
                        return;
                    }

                    // Ensure the target directories exist
                    if (!Directory.Exists(dataExtractPath))
                    {
                        Directory.CreateDirectory(dataExtractPath);
                    }
                    if (!Directory.Exists(backupExtractPath))
                    {
                        Directory.CreateDirectory(backupExtractPath);
                    }

                    // Clear the existing contents in the destination directories
                    ClearDirectory(dataExtractPath);
                    ClearDirectory(backupExtractPath);

                    // Copy the extracted 'Data' folder contents to C:\Wintakes\Data
                    CopyDirectory(extractedDataFolder, dataExtractPath);

                    // Copy the extracted 'Backup' folder contents to C:\Wintakes\Backup
                    CopyDirectory(extractedBackupFolder, backupExtractPath);

                    // Now handle the HHConfig.dlf file
                    string sourceHHConfigPath = Path.Combine(dataExtractPath, "HHConfig.dlf");
                    string destinationHHConfigPath = @"C:\Lostakes Data\HHConfigData.dlf";

                    if (File.Exists(sourceHHConfigPath))
                    {
                        // Read all lines from the HHConfig.dlf file
                        string[] lines = File.ReadAllLines(sourceHHConfigPath);

                        // Find the last non-empty line
                        string lastNonEmptyLine = lines.Reverse().FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));

                        if (!string.IsNullOrEmpty(lastNonEmptyLine))
                        {
                            // Write the last non-empty line to HHConfigData.dlf, replacing the file's content
                            File.WriteAllText(destinationHHConfigPath, lastNonEmptyLine);
                        }
                        else
                        {
                            MessageBox.Show("HHConfig.dlf file contains only empty lines.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("HHConfig.dlf file not found in the extracted Data folder.");
                    }

                    // Cleanup the temporary extraction directory
                    Directory.Delete(extractPath, true);

                    MessageBox.Show("Restore completed successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during restore: {ex.Message}");
            }
        }

        /// Helper method to clear all contents of a directory.
        private void ClearDirectory(string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                File.Delete(file);
            }
            foreach (var directory in Directory.GetDirectories(directoryPath))
            {
                Directory.Delete(directory, true);
            }
        }

        /// Backup current data before clearing.
        private void BackupBeforeClear()
        {
            // Clear the backup directory first
            ClearDirectory(backupPath);

            // Copy everything from the data directory to the backup directory
            CopyDirectory(directoryPath, backupPath);
        }

        private void RestoreBackupButton_Click(object sender, RoutedEventArgs e)
        {
            // Warn the user that this will clear their current data
            MessageBoxResult result = MessageBox.Show(
                "This will clear all your current data. Are you sure you want to proceed?",
                "Warning: Restore Data",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Ensure the backup directory exists
                    if (!Directory.Exists(backupPath))
                    {
                        MessageBox.Show("Backup folder not found. Unable to restore data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Ensure the data directory exists (create it if not)
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Clear the data directory
                    ClearDirectory(directoryPath);

                    // Copy the content from Backup to Data
                    CopyDirectory(backupPath, directoryPath);

                    MessageBox.Show("Data successfully restored.", "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred during restore: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Restore canceled.", "Cancel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
