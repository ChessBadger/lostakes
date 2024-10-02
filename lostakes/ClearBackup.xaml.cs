using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            // Backup logic can be implemented here if necessary
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
