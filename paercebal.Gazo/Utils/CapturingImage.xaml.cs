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
    /// Interaction logic for CapturingImage.xaml
    /// </summary>
    public partial class CapturingImage : Window
    {
        public CapturingImage()
        {
            InitializeComponent();
            WindowChrome.SetWindowChrome(this, new WindowChrome());
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
        private void TempButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).CloseCaptureWindow();
        }
        #endregion WPF events
    }
}
