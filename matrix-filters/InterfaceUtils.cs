using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace matrix_filters {
    public class InterfaceUtils {
        public static Bitmap GetBitmapFromDialog() {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Title = "Open image";
            dlg.Filter = "Image files (*.bmp, *.png, *.tga, *.jpg)|*.bmp;*.png;*.tga;*.jpg";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) {
                return null;
            }

            return new Bitmap(dlg.FileName);
        }

        public delegate void OnChangeDelegate(int x, int y, double val);

        private static TextBox CreateKernelCoefficientTextBox(int x, int y, double val, OnChangeDelegate onChange) {
            TextBox textBox = new TextBox();
            textBox.BeginInit();
            Grid.SetColumn(textBox, x);
            Grid.SetRow(textBox, y);
            textBox.Width = 75;
            textBox.Height = 20;
            textBox.Margin = new System.Windows.Thickness(5);
            textBox.Text = val.ToString();
            textBox.TextAlignment = System.Windows.TextAlignment.Center;
            textBox.TextChanged += (object sender, TextChangedEventArgs e) => {
                try {
                    onChange(x, y, double.Parse(textBox.Text));
                }
                catch (System.FormatException) { }
            };
            textBox.EndInit();
            return textBox;
        }

        public static Grid CreateKernelGrid(double[,] values, OnChangeDelegate onChange) {
            Grid kernelGrid = new Grid();
            kernelGrid.BeginInit();

            for(int i = 0; i < values.GetLength(0); ++i) {
                kernelGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for(int i = 0; i < values.GetLength(1); ++i) {
                kernelGrid.RowDefinitions.Add(new RowDefinition());
            }

            for(int i = 0; i < values.GetLength(0); ++i) {
                for(int j = 0; j < values.GetLength(1); ++j) {
                    kernelGrid.Children.Add(CreateKernelCoefficientTextBox(i, j, values[i, j], onChange));
                }
            }

            kernelGrid.EndInit();
            return kernelGrid;
        }
    }
}
