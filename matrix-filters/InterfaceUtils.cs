using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace matrix_filters {
    public class InterfaceUtils {
        public static Bitmap GetBitmapFromDialog() {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open image";
            dlg.Filter = "Image files (*.bmp, *.png, *.tga, *.jpg)|*.bmp;*.png;*.tga;*.jpg";
            if (dlg.ShowDialog() != DialogResult.OK) {
                return null;
            }

            return new Bitmap(dlg.FileName);
        }
    }
}
