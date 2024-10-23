using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace lostakes
{
    /// <summary>
    /// Interaction logic for SaveAccount.xaml
    /// </summary>
    public partial class SaveAccount : Window
    {
        public string AccountName { get; private set; }

        public SaveAccount()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            AccountName = AccountNameTextBox.Text;
            string chosenAccountFilePath = @"C:\Lostakes Data\ChosenAccount.txt";  // Path to store the chosen account

            // Save the chosen account name to the ChosenAccount.txt file
            File.WriteAllText(chosenAccountFilePath, AccountName);

            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
