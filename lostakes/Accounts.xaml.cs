﻿using System;
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

                // Enable the 'Delete' button
                DeleteButton.IsEnabled = true;
            }
            else
            {
                // Disable the 'Delete' button if no account is selected
                DeleteButton.IsEnabled = false;
            }
        }


        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the DC Setup Page when the 'New' button is clicked
            NavigationService.Navigate(new DcSetupPage());
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsListBox.SelectedItem != null)
            {
                string selectedAccount = AccountsListBox.SelectedItem.ToString();
                string accountsFolderPath = @"C:\Lostakes Data\Accounts";
                string selectedAccountFilePath = Path.Combine(accountsFolderPath, $"{selectedAccount}_HHConfigData.dlf");
                string destinationFilePath = @"C:\Lostakes Data\HHConfigData.dlf";
                string chosenAccountFilePath = @"C:\Lostakes Data\ChosenAccount.txt";  // Path to store the chosen account

                try
                {
                    // Copy the selected account file
                    File.Copy(selectedAccountFilePath, destinationFilePath, overwrite: true);

                    // Copy the "Use Cost Price" state for the selected account
                    string useCostPriceSourcePath = Path.Combine(accountsFolderPath, $"{selectedAccount}_UseCostPrice.txt");
                    string useCostPriceDestinationPath = @"C:\Lostakes Data\UseCostPrice.txt";

                    if (File.Exists(useCostPriceSourcePath))
                    {
                        File.Copy(useCostPriceSourcePath, useCostPriceDestinationPath, overwrite: true);
                    }

                    // Save the chosen account name to the ChosenAccount.txt file
                    File.WriteAllText(chosenAccountFilePath, selectedAccount);

                    MessageBox.Show($"Account '{selectedAccount}' has been applied successfully.");
                    NavigationService.Navigate(new SendToDcPage());
                }
                catch (Exception ex)
                {
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

        // Event handler for the 'Delete' button click
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AccountsListBox.SelectedItem != null)
            {
                string selectedAccount = AccountsListBox.SelectedItem.ToString();
                string accountsFolderPath = @"C:\Lostakes Data\Accounts";
                string selectedAccountFilePath = Path.Combine(accountsFolderPath, $"{selectedAccount}_HHConfigData.dlf");

                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the account '{selectedAccount}'?",
                                                          "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Check if the file exists
                        if (File.Exists(selectedAccountFilePath))
                        {
                            // Delete the file
                            File.Delete(selectedAccountFilePath);

                            // Remove the account from the list and refresh the ListBox
                            allAccounts.Remove(selectedAccount);
                            AccountsListBox.ItemsSource = null;
                            AccountsListBox.ItemsSource = allAccounts;

                            MessageBox.Show($"Account '{selectedAccount}' has been deleted.");
                        }
                        else
                        {
                            MessageBox.Show($"The file for the selected account '{selectedAccount}' does not exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions
                        MessageBox.Show($"An error occurred while deleting the account: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an account first.");
            }
        }



    }
}
