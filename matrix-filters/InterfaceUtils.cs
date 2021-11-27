using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace matrix_filters {
    public class InterfaceUtils {
        public static Bitmap GetBitmapFromDialog() {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Title = "Open image";
            dialog.Filter = "Image files (*.bmp, *.png, *.tga, *.jpg)|*.bmp;*.png;*.tga;*.jpg";
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) {
                return null;
            }

            return new Bitmap(dialog.FileName);
        }

        public static void SaveImageWithDialog(System.Windows.Controls.Image image) {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save image";
            dialog.Filter = "PNG files (*.png) |*.png";
            if(dialog.ShowDialog() == DialogResult.OK) {
                SaveToPng(image, dialog.FileName);
            }
        }

        public delegate void OnChangeDelegate(int x, int y, double val);

        private static System.Windows.Controls.TextBox CreateKernelCoefficientTextBox(int x, int y, double val, OnChangeDelegate onChange) {
            System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
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

        private static void SaveToPng(FrameworkElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder) {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName)) {
                encoder.Save(stream);
            }
        }
    }
}
