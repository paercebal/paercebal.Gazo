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
            //this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/paercebal.Gazo;component/Icons8-Windows-8-Editing-Screenshot - Black-Background.ico"));
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

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch(e.Key)
                {
                    case Key.C:
                        {
                            Utils.ScreenShooting.SaveImageIntoClipboard(this.CopiedImage);
                            break;
                        }
                    case Key.V:
                        {
                            try
                            {
                                this.SetImage(ScreenShooting.CaptureImageFromClipboard());
                            }
                            catch(System.NullReferenceException)
                            {
                                MessageBox.Show("There is no valid image in the Clipboard", "Failure", MessageBoxButton.OK);
                            }
                            break;
                        }
                }
            }
        }

        private void ZoomOriginalSizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.ZoomBorder.Reset();
        }

        private void ImageStretchButton_Click(object sender, RoutedEventArgs e)
        {
            this.ZoomBorder.Reset();

            if (this.CopiedImage.Stretch == Stretch.Uniform)
            {
                this.CopiedImage.Stretch = Stretch.None;
                this.ZoomOriginalSizeButton.Content = "Original";
                this.ImageStretchButton.Content = "Stretch";
            }
            else
            {
                this.CopiedImage.Stretch = Stretch.Uniform;
                this.ZoomOriginalSizeButton.Content = "Fill";
                this.ImageStretchButton.Content = "No Stretch";
            }

        }

        #endregion WPF events
    }
}
