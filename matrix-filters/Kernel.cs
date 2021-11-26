using System;

namespace matrix_filters {
    public class Kernel {
        static readonly int DEFAULT_SIZE = 3;

        public double[,] Coefficients { get; set; }
        public int Width { get { return Coefficients.GetLength(0); } }
        public int Height { get { return Coefficients.GetLength(1); } }

        int AnchorX;
        int AnchorY;

        double Divisor;

        public Kernel(int width, int height) {
            Coefficients = new double[width, height];
            AnchorX = 0;
            AnchorY = 0;
            Divisor = 1.0;
        }

        public void Apply(Texture texture, int x, int y) {
            Vec3 color = new Vec3();

            for(int i = 0; i < Coefficients.GetLength(0); ++i) {
                for(int j = 0; j < Coefficients.GetLength(1); ++j) {
                    color += texture.Pixels[x + i - AnchorX, y + j - AnchorY] * Coefficients[i, j] / Divisor;
                }
            }
        }

        public static Kernel Identity() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            return ker;
        }

        public static Kernel Blur() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            return ker;
        }

        public static Kernel Sharpen() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            return ker;
        }

        public static Kernel Relief() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            return ker;
        }

        public static Kernel EdgeDetection() {
            Kernel ker = new Kernel(DEFAULT_SIZE, DEFAULT_SIZE);
            ker.Coefficients[DEFAULT_SIZE / 2, DEFAULT_SIZE / 2] = 1.0;
            ker.Divisor = 1.0;
            return ker;
        }
    }
}
