using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace paercebal.Gazo.Utils
{
    public class ScreenShooting
    {
        static public System.Drawing.Bitmap CaptureImageFromFullScreen()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (var bitmap = new Movable<System.Drawing.Bitmap>(new Bitmap(bounds.Width, bounds.Height)))
            {
                using (Graphics g = Graphics.FromImage(bitmap.Get()))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                return bitmap.Release();
            }
        }
        static public System.Drawing.Bitmap CaptureImageFromClipboard()
        {
            return new Bitmap(Clipboard.GetImage());
        }
    }
}
