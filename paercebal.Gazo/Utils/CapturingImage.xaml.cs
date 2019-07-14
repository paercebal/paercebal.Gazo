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
    public struct TwoPoints
    {
        public TwoPoints(Point one, Point two)
        {
            this.One = one;
            this.Two = two;
        }

        public Point One;
        public Point Two;
    }

    /// <summary>
    /// Interaction logic for CapturingImage.xaml
    /// </summary>
    public partial class CapturingImage : Window
    {
        private System.Windows.Controls.Image CopiedImage = new System.Windows.Controls.Image();
        //private System.Windows.Shapes.Rectangle Greyer;

        public CapturingImage()
        {
            InitializeComponent();

            //WindowChrome.SetWindowChrome(this, new WindowChrome());
            //this.Background = Utils.Globals.GreyShadesCheckeredBrush;

            // Full screen:
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
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

            this.CapturingCanvas.Children.Add(this.CopiedImage);
        }

        private bool IsDragging = false;
        private Point DragMouseStart;
        private Point DragMouseCurrent;

        private Rectangle GreyTop;
        private Rectangle GreyLeft;
        private Rectangle GreyRight;
        private Rectangle GreyBottom;

        static private TwoPoints DeduceTopLeftAndBottomRightPoints(TwoPoints twoPoints)
        {
            var result = new TwoPoints();

            result.One.X = twoPoints.One.X < twoPoints.Two.X ? twoPoints.One.X : twoPoints.Two.X;
            result.One.Y = twoPoints.One.Y < twoPoints.Two.Y ? twoPoints.One.Y : twoPoints.Two.Y;
            result.Two.X = twoPoints.One.X > twoPoints.Two.X ? twoPoints.One.X : twoPoints.Two.X;
            result.Two.Y = twoPoints.One.Y > twoPoints.Two.Y ? twoPoints.One.Y : twoPoints.Two.Y;

            return result;
        }

        private void UpdateAllGreyingRectanglesPosition()
        {
            if (this.GreyTop == null)
            {
                void CreateAllGreyingRectangles()
                {
                    Rectangle CreateOneGreyingRectanble()
                    {
                        var rectangle = new Rectangle();
                        rectangle.Fill = Utils.Globals.GreyAndAlphaBrush;
                        return rectangle;
                    }

                    this.GreyTop = CreateOneGreyingRectanble();
                    this.GreyLeft = CreateOneGreyingRectanble();
                    this.GreyRight = CreateOneGreyingRectanble();
                    this.GreyBottom = CreateOneGreyingRectanble();

                    this.CapturingCanvas.Children.Add(this.GreyTop);
                    this.CapturingCanvas.Children.Add(this.GreyLeft);
                    this.CapturingCanvas.Children.Add(this.GreyRight);
                    this.CapturingCanvas.Children.Add(this.GreyBottom);
                }

                CreateAllGreyingRectangles();
            }

            //this.DebugOutput.Text = string.Format("[{0},{1}] [{2},{3}]", DragMouseStart.X, DragMouseStart.Y, DragMouseCurrent.X, DragMouseCurrent.Y);

            TwoPoints openFrame = DeduceTopLeftAndBottomRightPoints(new TwoPoints(DragMouseStart, DragMouseCurrent));

            this.GreyTop.Width = this.CopiedImage.ActualWidth;
            this.GreyTop.Height = openFrame.One.Y;
            Canvas.SetLeft(this.GreyTop, 0);
            Canvas.SetTop(this.GreyTop, 0);

            this.GreyLeft.Width = openFrame.One.X;
            this.GreyLeft.Height = this.CopiedImage.ActualHeight - openFrame.One.Y;
            Canvas.SetLeft(this.GreyLeft, 0);
            Canvas.SetTop(this.GreyLeft, openFrame.One.Y);

            this.GreyRight.Width = this.CopiedImage.ActualWidth - openFrame.Two.X;
            this.GreyRight.Height = this.CopiedImage.ActualHeight - openFrame.One.Y;
            Canvas.SetLeft(this.GreyRight, openFrame.Two.X);
            Canvas.SetTop(this.GreyRight, openFrame.One.Y);

            this.GreyBottom.Width = openFrame.Two.X - openFrame.One.X;
            this.GreyBottom.Height = this.CopiedImage.ActualHeight - openFrame.Two.Y;
            Canvas.SetLeft(this.GreyBottom, openFrame.One.X);
            Canvas.SetTop(this.GreyBottom, openFrame.Two.Y);
        }

        private void CopyImageSelection()
        {
            TwoPoints openFrame = DeduceTopLeftAndBottomRightPoints(new TwoPoints(DragMouseStart, DragMouseCurrent));

            if(openFrame.One != openFrame.Two)
            {
                CroppedBitmap cb = new CroppedBitmap((BitmapSource)this.CopiedImage.Source, new Int32Rect((int)openFrame.One.X, (int)openFrame.One.Y, (int)(openFrame.Two.X - openFrame.One.X), (int)(openFrame.Two.Y - openFrame.One.Y)));

                ((MainWindow)this.Owner).CreateImageFromBitmap(cb);
                ((MainWindow)this.Owner).CloseCaptureWindow();
            }
        }

        #region WPF events
        private void TempButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).CloseCaptureWindow();
        }

        public bool IsClosed = false;

        private void CapturingImage_Closed(object sender, EventArgs e)
        {
            this.IsClosed = true;
        }

        private void CapturingImage_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                ((MainWindow)this.Owner).CloseCaptureWindow();
            }
        }

        private void CapturingImage_Activated(object sender, EventArgs e)
        {
            if(this.GreyTop == null)
            {
                this.UpdateAllGreyingRectanglesPosition();
            }
        }

        private void CapturingImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsDragging = true;
            var mousePosition = e.GetPosition(this.CapturingCanvas);
            this.DragMouseStart = mousePosition;
            this.DragMouseCurrent = mousePosition;
            this.UpdateAllGreyingRectanglesPosition();
        }

        private void CapturingImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsDragging = false;
            var mousePosition = e.GetPosition(this.CapturingCanvas);
            this.DragMouseCurrent = mousePosition;
            this.UpdateAllGreyingRectanglesPosition();
            this.CopyImageSelection();
        }

        private void CapturingImage_MouseMove(object sender, MouseEventArgs e)
        {
            if(this.IsDragging)
            {
                var mousePosition = e.GetPosition(this.CapturingCanvas);
                this.DragMouseCurrent = mousePosition;
                this.UpdateAllGreyingRectanglesPosition();
            }
        }
        #endregion WPF events
    }
}
