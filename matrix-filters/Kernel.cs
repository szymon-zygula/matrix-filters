using System;

namespace matrix_filters {
    public class Kernel {
        static readonly int DEFAULT_SIZE = 3;

        public double[,] Coefficients { get; set; }
        public int Width { get { return Coefficients.GetLength(0); } }
        public int Height { get { return Coefficients.GetLength(1); } }

        public int AnchorX { get; set; }
        public int AnchorY { get; set; }

        public double Divisor { get; set; }

        public Kernel(int width, int height) {
            Coefficients = new double[width, height];
            AnchorX = 0;
            AnchorY = 0;
            Divisor = 1.0;
        }

        public Vec3 Apply(Texture texture, int x, int y) {
            Vec3 color = new Vec3();

            for(int i = 0; i < Coefficients.GetLength(0); ++i) {
                for(int j = 0; j < Coefficients.GetLength(1); ++j) {
                    int X = x + i - AnchorX;
                    int Y = y + j - AnchorY;

                    if (X < 0) X = 0;
                    else if (X >= texture.Width) X = texture.Width - 1;

                    if (Y < 0) Y = 0;
                    else if (Y >= texture.Height) Y = texture.Height - 1;

                    color += texture.Pixels[X, Y] * Coefficients[i, j] / Divisor;
                }
            }

            return color;
        }

        public void NormalizeDivisor() {
            double sum = 0.0;
            for(int i = 0; i < Width; ++i) {
                for(int j = 0; j < Height; ++j) {
                    sum += Coefficients[i, j];
                }
            }

            Divisor = sum;
        }

        public static Kernel Identity() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            ker.AnchorX = 1;
            ker.AnchorY = 1;
            return ker;
        }

        public static Kernel Blur() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            for(int i = 0; i < ker.Width; ++i) {
                for (int j = 0; j < ker.Height; ++j) {
                    ker.Coefficients[i, j] = 1.0;
                }
            }

            ker.Divisor = 9.0;
            ker.AnchorX = 1;
            ker.AnchorY = 1;
            return ker;
        }

        public static Kernel Sharpen() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            for(int i = 0; i < ker.Width; ++i) {
                for (int j = 0; j < ker.Height; ++j) {
                    ker.Coefficients[i, j] = -1.0;
                }
            }
            ker.Coefficients[1, 1] = 9.0;
            ker.Divisor = 1.0;
            ker.AnchorX = 1;
            ker.AnchorY = 1;
            return ker;
        }

        public static Kernel Relief() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            ker.AnchorX = 1;
            ker.AnchorY = 1;
            return ker;
        }

        public static Kernel EdgeDetection() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            ker.AnchorX = 1;
            ker.AnchorY = 1;
            return ker;
        }
    }
}
