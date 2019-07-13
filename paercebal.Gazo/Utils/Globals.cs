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
    static public class Globals
    {
        static public readonly Brush BlackAndWhiteCheckeredBrush = CheckeredBrush(Colors.Black, Colors.White);
        static public readonly Brush GreyShadesCheckeredBrush = CheckeredBrush(Color.FromArgb(255, 96, 96, 96), Color.FromArgb(255, 160, 160, 160));
        static public readonly Brush GreyShadesAndAlphaCheckeredBrush = CheckeredBrush(Color.FromArgb(64, 0, 0, 0), Color.FromArgb(64, 255, 255, 255));

        static Brush CheckeredBrush(Color colorOne, Color colorTwo)
        {
            // Source: https://stackoverflow.com/questions/25442616/how-to-draw-chess-board-pattern-with-brush-on-canvas

            GeometryDrawing createGeometryDrawing(bool version, Color color)
            {
                int positionShift = version ? 0 : 10;

                var group = new GeometryGroup();
                group.Children.Add(new RectangleGeometry(new Rect(0, positionShift, 10, 10)));
                group.Children.Add(new RectangleGeometry(new Rect(10, 10 - positionShift, 10, 10)));

                var checkersDrawing = new GeometryDrawing(new SolidColorBrush(color), null, group);
                return checkersDrawing;
            }

            // Create a Geometry with white background

            var checkersDrawingGroup = new DrawingGroup();
            checkersDrawingGroup.Children.Add(createGeometryDrawing(true, colorOne));
            checkersDrawingGroup.Children.Add(createGeometryDrawing(false, colorTwo));

            // Create a DrawingBrush
            var checkeredBrush = new DrawingBrush();
            checkeredBrush.Drawing = checkersDrawingGroup;

            // Set Viewport and TileMode
            checkeredBrush.Viewport = new Rect(0, 0, 0.025, 0.025);
            checkeredBrush.TileMode = TileMode.Tile;
            checkeredBrush.Stretch = Stretch.UniformToFill;

            return checkeredBrush;
        }
    }
}
