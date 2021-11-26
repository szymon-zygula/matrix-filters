using System;

namespace matrix_filters {
    public class Kernel {
        public double[,] Coefficients { get; set; }
        public int Width { get { return Coefficients.GetLength(0); } }
        public int Height { get { return Coefficients.GetLength(1); } }

        int AnchorX;
        int AnchorY;

        public Kernel(int width, int height) {
            Coefficients = new double[width, height];
            AnchorX = 0;
            AnchorY = 0;
        }

        public void Apply(Texture texture, int x, int y) {
            Vec3 color = new Vec3();

            for(int i = 0; i < Coefficients.GetLength(0); ++i) {
                for(int j = 0; j < Coefficients.GetLength(1); ++j) {
                    color += texture.Pixels[x + i - AnchorX, y + j - AnchorY] * Coefficients[i, j];
                }
            }
        }
    }
}
