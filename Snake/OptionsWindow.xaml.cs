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

namespace Snake
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        static int currentVolume = 50;
        System.Media.SoundPlayer sound = new System.Media.SoundPlayer("Hover.wav");

        public OptionsWindow()
        {
            InitializeComponent();
        }

        private void ButtonVolume_Click(object sender, RoutedEventArgs e)
        {
            if (sl_volume.Value == 0)
            {
                img_volume.Source = new BitmapImage(new Uri("/assets/Volume.png", UriKind.Relative));
                sl_volume.Value = currentVolume;
            }
            else
            {
                img_volume.Source = new BitmapImage(new Uri("/assets/VolumeRed.png", UriKind.Relative));
                currentVolume = (int)sl_volume.Value;
                sl_volume.Value = 0;
            }
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainWindow.player.Play();
            MainWindow.player.Volume = sl_volume.Value;

            lb_volume.Content = (int)sl_volume.Value + "%";
            if (sl_volume.Value == 0)
            {
                img_volume.Source = new BitmapImage(new Uri("/assets/VolumeRed.png", UriKind.Relative));
            }
            else
            {
                img_volume.Source = new BitmapImage(new Uri("/assets/Volume.png", UriKind.Relative));
            }
        }

        private void ImageVolume_MouseEnter(object sender, MouseEventArgs e)
        {
            sound.Play();
        }

        private void ButtonHome_MouseEnter(object sender, MouseEventArgs e)
        {
            img_home.Source = new BitmapImage(new Uri("/assets/HomeActive.png", UriKind.Relative));
            sound.Play();
        }

        private void ButtonHome_MouseLeave(object sender, MouseEventArgs e)
        {
            img_home.Source = new BitmapImage(new Uri("/assets/Home.png", UriKind.Relative));
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            sl_volume.Value = currentVolume;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            currentVolume = (int)sl_volume.Value;
        }
    }
}
