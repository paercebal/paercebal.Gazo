using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace paercebal.Gazo.Utils
{
    /// <summary>
    /// Interaction logic for CapturedImage.xaml
    /// </summary>
    public partial class CapturedImage : Window
    {
        public CapturedImage()
        {
            InitializeComponent();
            this.Background = Utils.Globals.GreyShadesCheckeredBrush;
        }

        public void SetImage(System.Drawing.Bitmap bitmap_)
        {
            using (var bitmap = new Movable<System.Drawing.Bitmap>(bitmap_))
            {
                using (var safeHandle = new SafeHBitmapHandle(bitmap.Get().GetHbitmap()))
                {
                    this.CopiedImage.Source = Imaging.CreateBitmapSourceFromHBitmap(safeHandle.Handle, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        public void SetImage(System.Windows.Media.Imaging.BitmapSource bitmap)
        {
            this.CopiedImage.Source = bitmap;
        }

        public void SetName(string name)
        {
            this.TitleTextBox.Text = name;
            this.Title = name;
        }

        #region WPF events
        private void ToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.ScreenShooting.SaveImageIntoClipboard(this.CopiedImage);
        }

        private void FromClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetImage(ScreenShooting.CaptureImageFromClipboard());
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var image = Utils.ScreenShooting.LoadImage();
            if(image != null)
            {
                this.SetImage(image);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = this.CopiedImage.Source as BitmapSource;

            if(bitmap == null)
            {
                MessageBox.Show("For some reason, the bitmap cannot be saved. Please Debug. Sorry.", "Error when saving", MessageBoxButton.OK);
                return;
            }

            Utils.ScreenShooting.SaveImage(this.Title, bitmap);
        }

        private void TitleTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            this.Title = this.TitleTextBox.Text;
        }

        private void TitleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Title = this.TitleTextBox.Text;
        }

        #endregion WPF events
    }
}
