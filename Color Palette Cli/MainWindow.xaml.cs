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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OokiiTsuki.Palette;

namespace Color_Palette_Cli
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

        private void ImportImage(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Images";
            dialog.DefaultExt = ".png";
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tif;";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                image.Source = null;
                var bm = new BitmapImage(new Uri(dialog.FileName));
                image.Source = bm;
                SetColors();
            }
        }

        private void SetColors()
        {
            Palette palette = Palette.Generate((BitmapSource)image.Source);

            var muted = palette.GetMutedColor(Colors.White);
            mutedBtn.Background = new SolidColorBrush(muted);
            mutedBtn.Foreground = new SolidColorBrush(muted.GetTitleTextColor());

            var darkMuted = palette.GetDarkMutedColor(Colors.White);
            darkMutedBtn.Background = new SolidColorBrush(darkMuted);
            darkMutedBtn.Foreground = new SolidColorBrush(darkMuted.GetTitleTextColor());

            var lightMuted = palette.GetLightMutedColor(Colors.White);
            lightMutedBtn.Background = new SolidColorBrush(lightMuted);
            lightMutedBtn.Foreground = new SolidColorBrush(lightMuted.GetTitleTextColor());

            var vibrant = palette.GetVibrantColor(Colors.White);
            vibrantBtn.Background = new SolidColorBrush(vibrant);
            vibrantBtn.Foreground = new SolidColorBrush(vibrant.GetTitleTextColor());

            var darkVibrant = palette.GetDarkVibrantColor(Colors.White);
            darkVibrantBtn.Background = new SolidColorBrush(darkVibrant);
            darkVibrantBtn.Foreground = new SolidColorBrush(darkVibrant.GetTitleTextColor());

            var lightVibrant = palette.GetLightVibrantColor(Colors.White);
            lightVibrantBtn.Background = new SolidColorBrush(lightVibrant);
            lightVibrantBtn.Foreground = new SolidColorBrush(lightVibrant.GetTitleTextColor());
        }

        private void ColorBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            Clipboard.SetText(btn.Background.ToString());
        }
    }
}
