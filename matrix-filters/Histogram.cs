using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace matrix_filters {
    public class Histogram {
        static readonly int WIDTH = 258;
        static readonly int HEIGHT = 258;
        static readonly int GRID_SPACING = 32;
        static readonly int GRID_THICKNESS = 2;

        public static readonly UInt32 WHITE = 0xFFFFFFFF;
        public static readonly UInt32 BLACK = 0xFF000000;
        public static readonly UInt32 RED = 0xFFFF0000;
        public static readonly UInt32 GREEN = 0xFF00FF00;
        public static readonly UInt32 BLUE = 0xFF0000FF;

        UInt32 Mask;
        int ColorPosition;

        int[] Counts;
        double Scale;

        UInt32[] Pixels;

        public Histogram(int colorPosition) {
            Pixels = new UInt32[HEIGHT * WIDTH];
            Counts = new int[WIDTH];
            Scale = 0;
            ColorPosition = colorPosition;
            Mask = (UInt32)0xFF << ColorPosition;
        }

        private void CleanValues() {
            for(int i = 0; i < WIDTH; ++i) {
                Counts[i] = 0;
            }
        }

        private void UpdateValues(Texture texture) {
            for(int x = 0; x < texture.Width; ++x) {
                for(int y = 0; y < texture.Height; ++y) {
                    Vec3 color = texture.Pixels[x, y];
                    Counts[(color.ToColor() & Mask) >> ColorPosition] += 1;
                }
            }
        }

        private void UpdateScale() {
            int max = Counts.Max();
            Scale = max == 0 ? 0 : (double)HEIGHT / (double)max;
        }

        private void CleanImage() {
            for(int x = 0; x < WIDTH; ++x) {
                for(int y = 0; y < HEIGHT; ++y) {
                    Pixels[x + WIDTH * y] = WHITE;
                }
            }
        }

        private void DrawGrid() {
            for(int x = 0; x < WIDTH; ++x) {
                for(int y = 0; y < HEIGHT; ++y) {
                    if(x % GRID_SPACING < GRID_THICKNESS || y % GRID_SPACING < GRID_THICKNESS) {
                        Pixels[x + WIDTH * y] = BLACK;
                    }
                }
            }
        }

        private void DrawHistogram() {
            for(int x = 0; x < WIDTH; ++x) {
                for(int y = HEIGHT - (int)Math.Floor(Counts[x] * Scale); y < HEIGHT; ++y) {
                    Pixels[x + WIDTH * y] = Mask | 0xFF000000;
                }
            }
        }

        private void UpdateImage() {
            DrawGrid();
            DrawHistogram();
        }

        public void Update(Texture texture) {
            CleanValues();
            UpdateValues(texture);
            UpdateScale();
            CleanImage();
            UpdateImage();
        }

        public BitmapSource CreateBitmapSource() {
            return BitmapSource.Create(
                WIDTH, HEIGHT,
                96, 96,
                PixelFormats.Bgra32,
                null,
                Pixels,
                WIDTH * 4
            );
        }
    }
}
