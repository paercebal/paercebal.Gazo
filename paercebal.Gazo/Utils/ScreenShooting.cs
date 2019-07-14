using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace paercebal.Gazo.Utils
{
    using Drawing = System.Drawing;
    using Forms = System.Windows.Forms;

    public class ScreenShooting
    {
        static public Drawing.Rectangle GetScreenSize()
        {
            return Forms.Screen.GetBounds(Drawing.Point.Empty);
        }

        static public Drawing.Bitmap CaptureImageFromFullScreen()
        {
            var bounds = GetScreenSize();
            using (var bitmap = new Movable<Drawing.Bitmap>(new Drawing.Bitmap(bounds.Width, bounds.Height)))
            {
                using (var g = Drawing.Graphics.FromImage(bitmap.Get()))
                {
                    g.CopyFromScreen(Drawing.Point.Empty, Drawing.Point.Empty, bounds.Size);
                }
                return bitmap.Release();
            }
        }

        static public Drawing.Bitmap CaptureImageFromClipboard()
        {
            return new Drawing.Bitmap(Forms.Clipboard.GetImage());
        }

        static public void SaveImageIntoClipboard(Image image)
        {
            double width = image.ActualWidth;
            double height = image.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(image);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }

        static public Drawing.Bitmap LoadImage()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif;*.tiff";

            if (openFileDialog.ShowDialog() == true)
            {
                string file = openFileDialog.FileName;

                if (!string.IsNullOrWhiteSpace(file))
                {
                    try
                    {
                        using (var image = Drawing.Image.FromFile(file))
                        {
                            return new Drawing.Bitmap(image);
                        }
                    }
                    catch (OutOfMemoryException ex)
                    {
                        MessageBox.Show(string.Format("Out of memory/Pixel format not supported:\n{0}", ex.Message), "Error when loading image", MessageBoxButton.OK);
                    }
                    catch (FileNotFoundException ex)
                    {
                        MessageBox.Show(string.Format("File was not found:\n{0}", ex.Message), "Error when loading image", MessageBoxButton.OK);
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(string.Format("File was not found:\n{0}", ex.Message), "Error when loading image", MessageBoxButton.OK);
                    }
                }
            }

            return null;
        }


        static public void SaveImage(string filename, BitmapSource bitmap)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = filename;
            saveFileDialog.DefaultExt = ".png";
            saveFileDialog.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif;*.tiff";

            while(true)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    var definitiveFileName = saveFileDialog.FileName;
                    var extension = ((definitiveFileName.LastIndexOf('.') >= 0) ? definitiveFileName.Substring(definitiveFileName.LastIndexOf('.') + 1) : "").ToLower();

                    if(string.IsNullOrWhiteSpace(extension) || ((extension != "bmp") && (extension != "png") && (extension != "jpg") && (extension != "gif") && (extension != "tiff")))
                    {
                        MessageBox.Show(string.Format("Invalid extension: [{0}].\n\nPlease retry or abort.", extension), "Error: Invalid Format", MessageBoxButton.OK);
                        continue;
                    }

                    try
                    {
                        // Source: https://stackoverflow.com/a/5709472/14089
                        int width = bitmap.PixelWidth;
                        int height = bitmap.PixelHeight;
                        int stride = width * ((bitmap.Format.BitsPerPixel + 7) / 8);

                        using (var safeHandle = new Utils.SafeIntPtrHandle(Marshal.AllocHGlobal(height * stride)))
                        {
                            bitmap.CopyPixels(new Int32Rect(0, 0, width, height), safeHandle.Handle, height * stride, stride);
                            using (var btm = new System.Drawing.Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, safeHandle.Handle))
                            {
                                btm.Save(definitiveFileName);
                            }
                        }
                    }
                    catch (ArgumentNullException ex)
                    {
                        MessageBox.Show(string.Format("Unexpected: Image name is null????\n{0}", ex.Message), "Error when saving image", MessageBoxButton.OK);
                    }
                    catch (ExternalException ex)
                    {
                        MessageBox.Show(string.Format("File format is wrong:\n{0}", ex.Message), "Error when saving image", MessageBoxButton.OK);
                    }
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
