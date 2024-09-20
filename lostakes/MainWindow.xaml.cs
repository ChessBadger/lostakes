using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lostakes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendToDcButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the SendToDcPage
            MainFrame.Navigate(new SendToDcPage());
        }

        private void ReceiveFromDcButton_Click(object sender, RoutedEventArgs e)
        {
            // Code for receiving from DC
            MessageBox.Show("Receiving from DC...");
        }

    }

}