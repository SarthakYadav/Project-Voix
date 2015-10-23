using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Project_Voix
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void addImgBtnClick(object sender, RoutedEventArgs e)
        {
            /*
                Sets the Background image for the Image Button
            */
            BitmapImage userImage = null;
            OpenFileDialog openImageFile = new OpenFileDialog();
            openImageFile.Title = "User Image Selection";
            openImageFile.Filter = "Image Files (*.jpg,*.gif,*.png,*.jpe,*.bmp)|*.jpg;*.gif;*.png;*.jpe;*.bmp";
            openImageFile.FilterIndex = 1;
            openImageFile.Multiselect = false;
            if (openImageFile.ShowDialog() == true)
            {
                userImage = new BitmapImage(new Uri(openImageFile.FileName));
                imgBrush.ImageSource = userImage;
            }
        }
    }
}
