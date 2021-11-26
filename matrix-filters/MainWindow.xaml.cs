using System.Drawing;
using System.Windows;
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
        }

        private void CreateHistograms() {
            RedHistogram = new Histogram(16);
            GreenHistogram = new Histogram(8);
            BlueHistogram = new Histogram(0);
        }

        private void KernelGrid_TextChanged(int x, int y, double val) {
            Filter.Coefficients[x, y] = val;
        }

        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e) {
            Bitmap bmp = InterfaceUtils.GetBitmapFromDialog();
            if(bmp == null) {
                return;
            }

            Image = new Texture(bmp);
            ImagePicture.Source = Image.CreateBitmapSource();
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
        }

        private void PolygonFilterArea_Unchecked(object sender, RoutedEventArgs e) {
            ButtonCleanPolygon.IsEnabled = false;
        }

        private void SetNewFilter(Kernel filter) {
            Filter = filter;
            ScrollViewerKernelContainer.Content =
                InterfaceUtils.CreateKernelGrid(Filter.Coefficients, KernelGrid_TextChanged);
            TextBoxFilterSizeX.Text = Filter.Width.ToString();
            TextBoxFilterSizeY.Text = Filter.Height.ToString();
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
        }

        private void RadioCustom_Unchecked(object sender, RoutedEventArgs e) {
            TextBoxFilterSizeX.IsEnabled = false;
            TextBoxFilterSizeY.IsEnabled = false;
        }

        private void CheckboxAutomaticDivisor_Checked(object sender, RoutedEventArgs e) {
            TextboxDivisor.IsEnabled = false;
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
    }
}
