﻿using System;
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
        private Utils.CapturingImage captureWindow;
        private List<Utils.CapturedImage> imageWindows = new List<Utils.CapturedImage>();
        private long counter = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private string GenerateNewImageName()
        {
            return string.Format("Image #{0}", this.counter++);
        }

        #region Screen Capture Handling
        private void CaptureFromScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if ((this.captureWindow != null) && (this.captureWindow.IsClosed))
            {
                this.captureWindow = null;
            }

            if (this.captureWindow == null)
            {
                using (var scopedHideShow = new Utils.ScopedHideShow(this))
                {
                    int delay = 1000 * int.Parse(((ComboBoxItem)this.DelayComboBox.SelectedItem)?.Tag?.ToString() ?? "0");
                    Thread.Sleep(500 + delay); // TODO This is both ugly and wrong. Wait for asynchronous "hidden" event instead. This will complicate scoped handling.
                    using (var screenshot = new Utils.Movable<System.Drawing.Bitmap>(Utils.ScreenShooting.CaptureImageFromFullScreen()))
                    {
                        this.captureWindow = new Utils.CapturingImage();
                        this.captureWindow.Owner = this;
                        this.captureWindow.SetImage(screenshot.Release());
                        this.captureWindow.Show();
                    }
                }
            }
        }

        private void CloseCaptureWindowCore()
        {
            if (this.captureWindow != null)
            {
                if(!this.captureWindow.IsClosed)
                {
                    this.captureWindow.Close();
                }
                this.captureWindow = null;
                this.Focus();
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

        private void CaptureFromClipboard()
        {
            try
            {
                using (var screenshot = new Utils.Movable<System.Drawing.Bitmap>(Utils.ScreenShooting.CaptureImageFromClipboard()))
                {
                    var imageWindow = new Utils.CapturedImage();
                    this.imageWindows.Add(imageWindow);
                    imageWindow.Owner = this;
                    imageWindow.SetImage(screenshot.Release());
                    imageWindow.SetName(this.GenerateNewImageName());
                    imageWindow.Show();
                }
            }
            catch (System.NullReferenceException)
            {
                MessageBox.Show("There is no valid image in the Clipboard", "Failure", MessageBoxButton.OK);
            }
        }

        #endregion Clipboard Capture Handling
        #region Disk Capture Handling

        private void CaptureFromDiskButton_Click(object sender, RoutedEventArgs e)
        {
            using (var screenshot = new Utils.Movable<System.Drawing.Bitmap>(Utils.ScreenShooting.LoadImage()))
            {
                if(screenshot.Get() != null)
                {
                    var imageWindow = new Utils.CapturedImage();
                    this.imageWindows.Add(imageWindow);
                    imageWindow.Owner = this;
                    imageWindow.SetImage(screenshot.Release());
                    imageWindow.SetName(this.GenerateNewImageName());
                    imageWindow.Show();
                }
            }
        }

        #endregion Disk Capture Handling
        #region Create Image From Screenshot Selection

        public void CreateImageFromBitmap(System.Windows.Media.Imaging.BitmapSource bitmap)
        {
            var imageWindow = new Utils.CapturedImage();
            this.imageWindows.Add(imageWindow);
            imageWindow.Owner = this;
            imageWindow.SetImage(bitmap);
            imageWindow.SetName(this.GenerateNewImageName());
            imageWindow.Show();
        }

        #endregion Create Image From Screenshot Selection
        #region WPF events
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.V:
                        {
                            this.CaptureFromClipboard();
                            break;
                        }
                }
            }
        }

        private void CaptureFromClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            this.CaptureFromClipboard();
        }

        #endregion WPF events
    }
}
