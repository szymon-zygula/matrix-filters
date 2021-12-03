using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace matrix_filters {
    public partial class MainWindow : Window {
        Histogram RedHistogram;
        Histogram GreenHistogram;
        Histogram BlueHistogram;
        Texture Image;
        Kernel Filter;
        Texture DrawingBuffer;
        List<Ellipse> PolygonVertices;
        Polygon DrawingPolygon;

        public MainWindow() {
            InitializeComponent();
            CreateHistograms();
            SetNewFilter(Kernel.Identity());
            ToggleCoefficientBoxes(false);
            PolygonVertices = new List<Ellipse>();
        }

        private void CreateHistograms() {
            RedHistogram = new Histogram(16);
            GreenHistogram = new Histogram(8);
            BlueHistogram = new Histogram(0);
        }

        private void ToggleCoefficientBoxes(bool value) {
            Grid kernelGrid = ScrollViewerKernelContainer.Content as Grid;
            foreach(UIElement uie in kernelGrid.Children) {
                uie.IsEnabled = value;
            }
        }

        private void KernelGrid_TextChanged(int x, int y, double val) {
            if (!RadioCustom.IsChecked.Value) return;

            double oldVal = Filter.Coefficients[x, y];
            Filter.Coefficients[x, y] = val;

            if(CheckboxAutomaticDivisor.IsChecked.Value) {
                Filter.Divisor += val - oldVal;
                if (Filter.Divisor == 0.0) Filter.Divisor = 1.0;
                TextboxDivisor.Text = Filter.Divisor.ToString();
            }
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e) {
            CleanPolygon();
            if(PolygonFilterArea.IsChecked.Value) {
                CreateNewPolygon();
            }

            Bitmap bmp = InterfaceUtils.GetBitmapFromDialog();
            if(bmp == null) {
                return;
            }

            ButtonSaveImage.IsEnabled = true;

            Image = new Texture(bmp);
            UpdatePicture();
        }

        private void UpdatePicture() {
            ImagePicture.Source = Image.CreateBitmapSource();
            ImagePicture.Width = Image.Width;
            ImagePicture.Height = Image.Height;
            CanvasImage.Width = Image.Width;
            CanvasImage.Height = Image.Height;
            UpdateHistograms();
        }

        private void UpdateHistograms() {
            RedHistogram.Update(Image);
            ImageRedHistogram.Source = RedHistogram.CreateBitmapSource();

            GreenHistogram.Update(Image);
            ImageGreenHistogram.Source = GreenHistogram.CreateBitmapSource();

            BlueHistogram.Update(Image);
            ImageBlueHistogram.Source = BlueHistogram.CreateBitmapSource();
        }

        private void ButtonSaveImage_Click(object sender, RoutedEventArgs e) {
            InterfaceUtils.SaveImageWithDialog(ImagePicture);
        }

        private void SliderBrushRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(LabelBrushRadius != null) {
                LabelBrushRadius.Content = ((int)e.NewValue).ToString();
                EllipseBrush.Width = e.NewValue;
                EllipseBrush.Height = e.NewValue;
            }
        }

        private void ButtonCleanPolygon_Click(object sender, RoutedEventArgs e) {
            CleanPolygon();
            CreateNewPolygon();
        }

        private void CircularBrushFilterArea_Checked(object sender, RoutedEventArgs e) {
            SliderBrushRadius.IsEnabled = true;
            EllipseBrush.Visibility = Visibility.Visible;
        }

        private void CircularBrushFilterArea_Unchecked(object sender, RoutedEventArgs e) {
            SliderBrushRadius.IsEnabled = false;
            EllipseBrush.Visibility = Visibility.Hidden;
        }

        private void UpdateDrawingBuffer() {
            if (DrawingBuffer == null || Image == null) return;
            double r = SliderBrushRadius.Value / 2;
            double x0 = Canvas.GetLeft(EllipseBrush) + r;
            double y0 = Canvas.GetTop(EllipseBrush) + r;
            Parallel.For((int)Math.Round(x0 - r), (int)Math.Round(x0 + r), (x) => {
                int localRadius = (int)Math.Round(Math.Sqrt(r * r - (x - x0) * (x - x0)));
                int lowerBound = (int)Math.Round(y0) - localRadius;
                int upperBound = (int)Math.Round(y0) + localRadius;
                for (int y = lowerBound; y <= upperBound; ++y) {
                    int X = x;
                    int Y = y;
                    if (X < 0) X = 0;
                    if (Y < 0) Y = 0;
                    if (X >= Image.Width) X = Image.Width - 1;
                    if (Y >= Image.Height) Y = Image.Height - 1;
                    Image.Pixels[X, Y] = Filter.Apply(DrawingBuffer, X, Y);
                }
            });

            UpdatePicture();
        }

        private void CanvasImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (CircularBrushFilterArea.IsChecked.Value) BrushMouseDown();
            if (PolygonFilterArea.IsChecked.Value) PolygonMouseDown(e);
        }

        private void PolygonMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            const int POINT_SIZE = 10;
            System.Windows.Point point = e.GetPosition(CanvasImage);
            Ellipse newVertex = new Ellipse();
            newVertex.Fill = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("Red");
            newVertex.Width = POINT_SIZE;
            newVertex.Height = POINT_SIZE;
            Canvas.SetLeft(newVertex, point.X - POINT_SIZE / 2);
            Canvas.SetTop(newVertex, point.Y - POINT_SIZE / 2);
            CanvasImage.Children.Add(newVertex);
            PolygonVertices.Add(newVertex);
            DrawingPolygon.Points.Add(point);
        }

        private void CleanPolygon() {
            foreach(Ellipse el in PolygonVertices) {
                CanvasImage.Children.Remove(el);
            }

            PolygonVertices.Clear();
            CanvasImage.Children.Remove(DrawingPolygon);
        }

        private void CreateNewPolygon() {
            DrawingPolygon = new Polygon();
            DrawingPolygon.Stroke = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("Green");
            DrawingPolygon.StrokeThickness = 3;
            CanvasImage.Children.Add(DrawingPolygon);
        }

        private void BrushMouseDown() {
            DrawingBuffer = Image.Clone();
            UpdateDrawingBuffer();
        }

        private void CanvasImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (CircularBrushFilterArea.IsChecked.Value) BrushMouseUp();
        }

        private void BrushMouseUp() {
            UpdateDrawingBuffer();
            DrawingBuffer = null;
        }

        private void CanvasImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            if (CircularBrushFilterArea.IsChecked.Value) BrushMouseMove(e);
        }

        private void BrushMouseMove(System.Windows.Input.MouseEventArgs e) {
            System.Windows.Point mouse = e.GetPosition(CanvasImage);
            Canvas.SetLeft(EllipseBrush, mouse.X - SliderBrushRadius.Value / 2);
            Canvas.SetTop(EllipseBrush, mouse.Y - SliderBrushRadius.Value / 2);
            if (DrawingBuffer == null) return;
            UpdateDrawingBuffer();
        }

        private void PolygonFilterArea_Checked(object sender, RoutedEventArgs e) {
            ButtonCleanPolygon.IsEnabled = true;
            ButtonApplyPolygon.IsEnabled = true;
            CreateNewPolygon();
        }

        private void PolygonFilterArea_Unchecked(object sender, RoutedEventArgs e) {
            CleanPolygon();
            ButtonCleanPolygon.IsEnabled = false;
            ButtonApplyPolygon.IsEnabled = false;
        }

        private void SetNewFilter(Kernel filter) {
            Filter = filter;
            ScrollViewerKernelContainer.Content =
                InterfaceUtils.CreateKernelGrid(Filter.Coefficients, KernelGrid_TextChanged);
            TextBoxFilterSizeX.Text = Filter.Width.ToString();
            TextBoxFilterSizeY.Text = Filter.Height.ToString();
            TextboxDivisor.Text = Filter.Divisor.ToString();
            TextboxShift.Text = Filter.AnchorX.ToString();
            ToggleCoefficientBoxes(false);
        }

        private void RadioIdentity_Checked(object sender, RoutedEventArgs e) {
            if (ScrollViewerKernelContainer == null) return;
            SetNewFilter(Kernel.Identity());
        }

        private void RadioBlur_Checked(object sender, RoutedEventArgs e) {
            SetNewFilter(Kernel.Blur());
        }

        private void RadioSharpen_Checked(object sender, RoutedEventArgs e) {
            SetNewFilter(Kernel.Sharpen());
        }

        private void RadioRelief_Checked(object sender, RoutedEventArgs e) {
            SetNewFilter(Kernel.Relief());
        }

        private void RadioEdgeDetection_Checked(object sender, RoutedEventArgs e) {
            SetNewFilter(Kernel.EdgeDetection());
        }

        private void RadioCustom_Checked(object sender, RoutedEventArgs e) {
            TextBoxFilterSizeX.IsEnabled = true;
            TextBoxFilterSizeY.IsEnabled = true;
            TextboxShift.IsEnabled = true;
            CheckboxAutomaticDivisor.IsEnabled = true;
            ToggleCoefficientBoxes(true);
        }

        private void RadioCustom_Unchecked(object sender, RoutedEventArgs e) {
            TextBoxFilterSizeX.IsEnabled = false;
            TextBoxFilterSizeY.IsEnabled = false;
            TextboxShift.IsEnabled = false;
            CheckboxAutomaticDivisor.IsEnabled = false;
            ToggleCoefficientBoxes(false);
        }

        private void CheckboxAutomaticDivisor_Checked(object sender, RoutedEventArgs e) {
            TextboxDivisor.IsEnabled = false;
            if (Filter == null) return;
            Filter.NormalizeDivisor();
            TextboxDivisor.Text = Filter.Divisor.ToString();
        }

        private void CheckboxAutomaticDivisor_Unchecked(object sender, RoutedEventArgs e) {
            TextboxDivisor.IsEnabled = true;
        }

        private void ResizeCustomFilter() {
            try {
                SetNewFilter(new Kernel(
                    int.Parse(TextBoxFilterSizeX.Text),
                    int.Parse(TextBoxFilterSizeY.Text)
                ));
                ToggleCoefficientBoxes(true);
            }
            catch(System.FormatException) { }
        }

        private void TextBoxFilterSizeX_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            if (!TextBoxFilterSizeX.IsEnabled || ScrollViewerKernelContainer == null) return;
            ResizeCustomFilter();
        }

        private void TextBoxFilterSizeY_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            if (!TextBoxFilterSizeY.IsEnabled || ScrollViewerKernelContainer == null) return;
            ResizeCustomFilter();
        }

        private void TextboxDivisor_TextChanged(object sender, TextChangedEventArgs e) {
            if (!TextboxDivisor.IsEnabled || Filter == null) return;
            try {
                double divisor = double.Parse(TextboxDivisor.Text);
                if (divisor == 0.0) return;
                Filter.Divisor = divisor;
            }
            catch(System.FormatException) { }
        }

        private void TextboxShift_TextChanged(object sender, TextChangedEventArgs e) {
            if (Filter == null || !TextboxShift.IsEnabled) return;
            try {
                Filter.AnchorX = int.Parse(TextboxShift.Text);
                Filter.AnchorY = int.Parse(TextboxShift.Text);
            }
            catch(System.FormatException) { }
        }

        private void ButtonApplyPolygon_Click(object sender, RoutedEventArgs e) {
            Texture newImage = Image.Clone();
            if (Image == null) return;
            for(int x = 0; x < Image.Width; ++x) {
                for(int y = 0; y < Image.Height; ++y) {
                    if(DrawingPolygon.RenderedGeometry.FillContains(new System.Windows.Point(x, y))) {
                        newImage.Pixels[x, y] = Filter.Apply(Image, x, y);
                    }
                }
            }

            Image = newImage;
            UpdatePicture();
        }

        private void ButtonApplyWholeImage_Click(object sender, RoutedEventArgs e) {
            if (Image == null) return;
            Texture buffer = Image.Clone();
            Parallel.For(0, Image.Width, (x) => {
                for (int y = 0; y < Image.Height; ++y) {
                    buffer.Pixels[x, y] = Filter.Apply(Image, x, y);
                }
            });

            Image = buffer;
            UpdatePicture();
        }

        private void WholeImageFilterArea_Checked(object sender, RoutedEventArgs e) {
            if (ButtonApplyWholeImage == null) return;
            ButtonApplyWholeImage.IsEnabled = true;
        }

        private void WholeImageFilterArea_Unchecked(object sender, RoutedEventArgs e) {
            ButtonApplyWholeImage.IsEnabled = false;
        }
    }
}
