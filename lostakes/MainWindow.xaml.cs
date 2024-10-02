using System.IO;
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
using Path = System.IO.Path;

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
            CheckAndCreateDirectory();
            CreateAndPopulateDlfFiles();
        }

        private void SendToDcButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the SendToDcPage
            MainFrame.Navigate(new SendToDcPage());
        }

        private void CheckAndCreateDirectory()
        {
            string path = @"C:\Lostakes Data";

            // Check if the directory exists
            if (!Directory.Exists(path))
            {
                // Create the directory if it does not exist
                Directory.CreateDirectory(path);
            }
            else
            {
            }
        }

        private void CreateAndPopulateDlfFiles()
        {
            string directoryPath = @"C:\Lostakes Data";

            string areaContent = "0,0,0\n5,15,0\n16,15,0\n0,0,0";

            // File content for Location.dlf
            string locationContent = "0,0,0\n5,15,0\n5,15,0\n16,15,0\n0,0,0";

            // File content for Category.dlf
            string categoryContent = "0,0,0\n3,15,0\n16,15,0\n2,15,0\n0,0,0";

            // File content for HHConfig
            string hhConfigContent = "0,0,0\n6,15,0\n\n5,15,0\n5,15,0\n4,15,0\n5,15,0\n1,15,0\n9,15,0\n6,15,0\n9,15,0\n9,15,0\n1,15,0\n5,15,0\n5,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n2,15,0\n1,15,0\n2,15,0\n1,15,0\n2,15,0\n1,15,0\n1,15,0\n1,15,0\n2,15,0\n2,15,0\n1,15,0\n2,15,0\n2,15,0\n1,15,0\n2,15,0\n2,15,0\n1,15,0\n2,15,0\n2,15,0\n1,15,0\n2,15,0\n2,15,0\n1,15,0\n1,15,0\n1,15,0\n2,15,0\n2,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n1,15,0\n12,15,0\n8,15,0\n13,15,0\n17,15,0\n1,15,0\n1,15,0\n1,15,0\n0,0,0";

            // File content for Skufile
            string skuFileContent = "0,0,0\n17,15,0\n6,14,0\n17,15,0\n6,14,0\n17,15,0\n6,14,0\n17,15,0\n6,14,0\n17,15,0\n6,14,0\n0,0,0";

            // Create and populate each file
            CreateFileIfNotExists(Path.Combine(directoryPath, "AREA.dlf"), areaContent);
            CreateFileIfNotExists(Path.Combine(directoryPath, "LOCATION.dlf"), locationContent);
            CreateFileIfNotExists(Path.Combine(directoryPath, "CATEGORY.dlf"), categoryContent);
            CreateFileIfNotExists(Path.Combine(directoryPath, "HHConfig.dlf"), hhConfigContent);
            CreateFileIfNotExists(Path.Combine(directoryPath, "SKUFILE.dlf"), skuFileContent);
        }

        private void CreateFileIfNotExists(string filePath, string content)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // Create the file and populate it with the provided content
                File.WriteAllText(filePath, content);
            }
       
        }

        private void ReceiveFromDcButton_Click(object sender, RoutedEventArgs e)
        {
            // Code for receiving from DC
            MessageBox.Show("Receiving from DC...");
        }

        private void ClearBackupButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClearBackup());
        }
    }

}