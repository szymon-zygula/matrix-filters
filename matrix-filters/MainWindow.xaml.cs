using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace matrix_filters {
    public partial class MainWindow : Window {
        Histogram RedHistogram;
        Histogram GreenHistogram;
        Histogram BlueHistogram;
        Texture Image;
        Kernel Filter;

        public MainWindow() {
            InitializeComponent();
            CreateHistograms();
            SetNewFilter(Kernel.Identity());
            ToggleCoefficientBoxes(false);
        }

        private void CreateHistograms() {
            RedHistogram = new Histogram(16);
            GreenHistogram = new Histogram(8);
            BlueHistogram = new Histogram(0);
        }

        private void EnsureCustomMode() {
            if(!RadioCustom.IsChecked.Value) {
                RadioCustom.IsChecked = true;
            }
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

            EnsureCustomMode();

            if(CheckboxAutomaticDivisor.IsChecked.Value) {
                Filter.Divisor += val - oldVal;
                TextboxDivisor.Text = Filter.Divisor.ToString();
            }
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e) {
            Bitmap bmp = InterfaceUtils.GetBitmapFromDialog();
            if(bmp == null) {
                return;
            }

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

        }

        private void SliderBrushRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if(LabelBrushRadius != null) {
                LabelBrushRadius.Content = ((int)e.NewValue).ToString();
            }
        }

        private void ButtonCleanPolygon_Click(object sender, RoutedEventArgs e) {

        }

        private void CircularBrushFilterArea_Checked(object sender, RoutedEventArgs e) {
            SliderBrushRadius.IsEnabled = true;
        }

        private void CircularBrushFilterArea_Unchecked(object sender, RoutedEventArgs e) {
            SliderBrushRadius.IsEnabled = false;
        }

        private void PolygonFilterArea_Checked(object sender, RoutedEventArgs e) {
            ButtonCleanPolygon.IsEnabled = true;
            ButtonApplyPolygon.IsEnabled = true;
        }

        private void PolygonFilterArea_Unchecked(object sender, RoutedEventArgs e) {
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
                Filter.Divisor = double.Parse(TextboxDivisor.Text);
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

        private void CanvasImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {

        }

        private void CanvasImage_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {

        }

        private void CanvasImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {

        }

        private void ButtonApplyPolygon_Click(object sender, RoutedEventArgs e) {
            if (Image == null) return;

        }

        private void ButtonApplyWholeImage_Click(object sender, RoutedEventArgs e) {
            if (Image == null) return;
            Texture buffer = Image.Clone();
            for(int x = 0; x < Image.Width; ++x) {
                for (int y = 0; y < Image.Height; ++y) {
                    buffer.Pixels[x, y] = Filter.Apply(Image, x, y);
                }
            }

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
