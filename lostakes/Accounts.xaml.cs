using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace lostakes
{
    public partial class Accounts : Page
    {
        private List<string> allAccounts = new List<string>();

        public Accounts()
        {
            InitializeComponent();

            // Load the account names when the page is initialized
            LoadAccountNames();
        }

        private void LoadAccountNames()
        {
            string accountsFolderPath = @"C:\Lostakes Data\Accounts";

            // Check if the accounts folder exists
            if (Directory.Exists(accountsFolderPath))
            {
                // Get all files in the Accounts folder
                string[] accountFiles = Directory.GetFiles(accountsFolderPath, "*_HHConfigData.dlf");

                // Check if there are files
                if (accountFiles.Length == 0)
                {
                    MessageBox.Show("No accounts found.");
                    return;
                }

                // Extract the account names (excluding "_HHConfig")
                allAccounts = accountFiles.Select(file => Path.GetFileNameWithoutExtension(file).Replace("_HHConfigData", "")).ToList();

                // Populate the ListBox with account names
                AccountsListBox.ItemsSource = allAccounts;
            }
            else
            {
                MessageBox.Show("No accounts folder found.");
            }
        }

        // Event handler for the search box text changed
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            // Hide or show the placeholder text based on the input
            PlaceholderTextBlock.Visibility = string.IsNullOrEmpty(searchText) ? Visibility.Visible : Visibility.Hidden;

            // Filter the account names based on the search text
            var filteredAccounts = allAccounts.Where(account => account.ToLower().Contains(searchText)).ToList();

            // Update the ListBox with the filtered accounts
            AccountsListBox.ItemsSource = filteredAccounts;

            // Auto-select if there's an exact match in the filtered list
            var exactMatch = filteredAccounts.FirstOrDefault(account => account.Equals(searchText, StringComparison.OrdinalIgnoreCase));
            if (exactMatch != null)
            {
                AccountsListBox.SelectedItem = exactMatch;
            }
        }

        // Event handler for when an account is selected from the list
        private void AccountsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountsListBox.SelectedItem != null)
            {
                // Set the text in the SearchTextBox to the selected account
                SearchTextBox.Text = AccountsListBox.SelectedItem.ToString();
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the DC Setup Page when the 'New' button is clicked
            NavigationService.Navigate(new DcSetupPage());
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if an account is selected
            if (AccountsListBox.SelectedItem != null)
            {
                string selectedAccount = AccountsListBox.SelectedItem.ToString();
                string accountsFolderPath = @"C:\Lostakes Data\Accounts";
                string selectedAccountFilePath = Path.Combine(accountsFolderPath, $"{selectedAccount}_HHConfigData.dlf");
                string destinationFilePath = @"C:\Lostakes Data\HHConfigData.dlf";

                try
                {
                    // Check if the selected account file exists
                    if (File.Exists(selectedAccountFilePath))
                    {
                        // Overwrite the HHConfigData.dlf file with the selected account's file content
                        File.Copy(selectedAccountFilePath, destinationFilePath, overwrite: true);

                        MessageBox.Show($"Account '{selectedAccount}' has been applied successfully.");

                        // Navigate back to the previous page (assuming it's the main window or another page)
                        NavigationService.GoBack(); // or NavigationService.Navigate(new PreviousPage());
                    }
                    else
                    {
                        MessageBox.Show($"The file for the selected account '{selectedAccount}' does not exist.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    MessageBox.Show($"An error occurred while applying the account: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select an account first.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page when 'Cancel' is clicked
            NavigationService.GoBack();
        }


    }
}
