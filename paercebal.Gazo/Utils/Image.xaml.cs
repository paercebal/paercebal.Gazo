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
    /// Interaction logic for Image.xaml
    /// </summary>
    public partial class Image : Window
    {
        public Image()
        {
            InitializeComponent();
        }

        public void SetImage(System.Drawing.Bitmap bitmap_)
        {
            using (var bitmap = new Movable<System.Drawing.Bitmap>(bitmap_))
            {
                var handle = bitmap.Get().GetHbitmap();
                using (var safeHandle = new SafeHBitmapHandle(handle))
                {
                    this.CopiedImage.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }

        #region WPF events
        private void ToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            double width = this.CopiedImage.ActualWidth;
            double height = this.CopiedImage.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(this.CopiedImage);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }

        private void FromClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetImage(ScreenShooting.CaptureImageFromClipboard());
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
