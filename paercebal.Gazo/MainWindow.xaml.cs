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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace paercebal.Gazo
{
    // Sources: https://stackoverflow.com/questions/1163761/capture-screenshot-of-active-window

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Utils.CapturingImage captureWindow;
        List<Utils.Image> imageWindows = new List<Utils.Image>();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Screen Capture Handling
        private void CaptureFromScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.captureWindow == null)
            {
                using (var screenshot = new Utils.Movable<System.Drawing.Bitmap>(Utils.ScreenShooting.CaptureImageFromFullScreen()))
                {
                    this.captureWindow = new Utils.CapturingImage();
                    this.captureWindow.Owner = this;
                    this.captureWindow.SetImage(screenshot.Release());
                    this.captureWindow.Show();
                }
            }
        }

        private void CloseCaptureWindowCore()
        {
            if (this.captureWindow != null)
            {
                this.captureWindow.Close();
                this.captureWindow = null;
            }
        }

        public void CloseCaptureWindow()
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.CloseCaptureWindowCore();
            }));
        }

        private void CloseCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            this.CloseCaptureWindow();
        }

        #endregion Screen Capture Handling
        #region Clipboard Capture Handling

        private void CaptureFromClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var screenshot = new Utils.Movable<System.Drawing.Bitmap>(Utils.ScreenShooting.CaptureImageFromClipboard()))
                {
                    var imageWindow = new Utils.Image();
                    this.imageWindows.Add(imageWindow);
                    imageWindow.Owner = this;
                    imageWindow.SetImage(screenshot.Release());
                    imageWindow.Show();
                }
            }
            catch(System.NullReferenceException)
            {
                MessageBox.Show("There is no valid image in the Clipboard", "Failure", MessageBoxButton.OK);
            }
        }

        #endregion Clipboard Capture Handling
    }
}
