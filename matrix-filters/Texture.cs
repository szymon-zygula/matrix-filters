using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace matrix_filters {
    // This class has to be used instead of System.Drawing.Bitmap,
    // because System.Drawing.Bitmap.GetPixel too slow to be used in real time rendering
    public class Texture {
        public Vec3[,] Pixels;
        public int Width;
        public int Height;
        public Texture(Bitmap bitmap) {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            Width = (int)bounds.Width;
            Height = (int)bounds.Height;

            Pixels = new Vec3[Width, Height];

            for(int x = 0; x < Width; ++x) {
                for(int y = 0; y < Height; ++y) {
                    int color = bitmap.GetPixel(x, y).ToArgb();
                    UInt32 convertedColor;
                    unsafe {
                        convertedColor = *(UInt32*)(void*)&color;
                    }

                    Pixels[x, y] = new Vec3(convertedColor);
                }
            }
        }

        public BitmapSource CreateBitmapSource() {
            UInt32[] pixels = new UInt32[Width * Height];
            for(int x = 0; x < Width; ++x) {
                for(int y = 0; y < Height; ++y) {
                    pixels[x + y * Width] = Pixels[x, y].ToColor();
                }
            }

            return BitmapSource.Create(
                Width, Height,
                96, 96,
                PixelFormats.Bgra32,
                null,
                pixels,
                Width * 4
            );
        }
    }
}
