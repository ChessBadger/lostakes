using System.Windows;
using System.Windows.Controls;

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
            // Logic for Create Cards
            MessageBox.Show("Create Cards button clicked");
        }

        private void DcSetupButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the DC Setup Page
            NavigationService.Navigate(new DcSetupPage());
        }
    }
}
