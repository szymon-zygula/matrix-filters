using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace matrix_filters {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        private void ButtonLoadImage_Click(object sender, RoutedEventArgs e) {
            Bitmap bmp = InterfaceUtils.GetBitmapFromDialog();
            if(bmp == null) {
                return;
            }

            ImagePicture.Source = BitmapUtils.BitmapToSource(bmp);
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

        private void RadioIdentity_Checked(object sender, RoutedEventArgs e) {

        }

        private void RadioBlur_Checked(object sender, RoutedEventArgs e) {

        }

        private void RadioSharpen_Checked(object sender, RoutedEventArgs e) {

        }

        private void RadioRelief_Checked(object sender, RoutedEventArgs e) {

        }

        private void RadioEdgeDetection_Checked(object sender, RoutedEventArgs e) {

        }

        private void RadioCustom_Checked(object sender, RoutedEventArgs e) {

        }

        private void CheckboxAutomaticDivisor_Checked(object sender, RoutedEventArgs e) {
            TextboxDivisor.IsEnabled = false;
        }

        private void CheckboxAutomaticDivisor_Unchecked(object sender, RoutedEventArgs e) {
            TextboxDivisor.IsEnabled = true;
        }
    }
}
