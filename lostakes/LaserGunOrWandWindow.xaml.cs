using System.Windows;

namespace lostakes
{
    public partial class LaserGunOrWandWindow : Window
    {

        public LaserGunOrWandData LaserGunOrWandData { get; private set; }

        public LaserGunOrWandWindow(LaserGunOrWandData existingLaserGunOrWandData)
        {
            InitializeComponent();
            LaserGunOrWandData = existingLaserGunOrWandData;
            this.Loaded += LaserGunOrWandWindow_Loaded;
        }

        // Wand checkbox is checked
        private void Wand_Checked(object sender, RoutedEventArgs e)
        {
            LaserCheckbox.IsChecked = false; // Uncheck Laser
            PolarityOptionsPanel.Visibility = Visibility.Collapsed; // Hide polarity options
            StandardPolarityCheckbox.IsChecked = false;
            ReversePolarityCheckbox.IsChecked = false;
        }



        // Laser checkbox is checked
        private void Laser_Checked(object sender, RoutedEventArgs e)
        {
            WandCheckbox.IsChecked = false; // Uncheck Wand
            PolarityOptionsPanel.Visibility = Visibility.Visible; // Show polarity options
        }

        // Laser checkbox is unchecked
        private void Laser_Unchecked(object sender, RoutedEventArgs e)
        {
            PolarityOptionsPanel.Visibility = Visibility.Collapsed; // Hide polarity options
            StandardPolarityCheckbox.IsChecked = false;
            ReversePolarityCheckbox.IsChecked = false;
        }

        private void Standard_Checked(object sender, RoutedEventArgs e)
        {
            ReversePolarityCheckbox.IsChecked = false;

        }

        private void Reverse_Checked(object sender, RoutedEventArgs e)
        {
            StandardPolarityCheckbox.IsChecked = false;

        }


        // OK button handler
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            LaserGunOrWandData = new LaserGunOrWandData
            {
                Wand = WandCheckbox.IsChecked == true,
                Reverse = ReversePolarityCheckbox.IsChecked == true,
                Standard = StandardPolarityCheckbox.IsChecked == true
            };


            this.DialogResult = true;
            this.Close();
        }

        // Cancel button handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void LaserGunOrWandWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WandCheckbox.IsChecked = LaserGunOrWandData.Wand;
            LaserCheckbox.IsChecked = !LaserGunOrWandData.Wand; // Assume Laser is checked when Wand is not

            if (LaserGunOrWandData.Wand)
            {
                PolarityOptionsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                PolarityOptionsPanel.Visibility = Visibility.Visible;
                StandardPolarityCheckbox.IsChecked = LaserGunOrWandData.Standard;
                ReversePolarityCheckbox.IsChecked = LaserGunOrWandData.Reverse;
            }
        }

    }


    public class LaserGunOrWandData
    {
        public bool Wand { get; set; }
        public bool Reverse{ get; set; }
        public bool Standard { get; set; }
    }
}
