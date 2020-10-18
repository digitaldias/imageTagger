using ImageTagger.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageTagger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => DataContext = new MainWindowViewModel();
        }

        // Allow the window to be dragged around by click-drag
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ConfidentResultControl_MouseEnter(object sender, MouseEventArgs e)
        {
            var listItem = sender as ImageTagger.Controls.ConfidentResultControl;
            if(listItem.DataContext is VisionClientApi.ObjectResult)
            {
                var result            = (VisionClientApi.ObjectResult)listItem.DataContext;
                var imageScaleFactorX = TheImage.ActualWidth / TheImage.Source.Width;
                var imageScaleFactorY = TheImage.ActualHeight / TheImage.Source.Height;
                var marginLeft        = (DrawCanvas.ActualWidth - TheImage.ActualWidth) / 2;
                var marginTop         = (DrawCanvas.ActualHeight - TheImage.ActualHeight) / 2;

                var rect    = new Rectangle();
                rect.Width = imageScaleFactorX * result.W;
                rect.Height =imageScaleFactorY * result.H;
                rect.Margin = new Thickness(
                    left  : (imageScaleFactorX * result.X) + marginLeft,
                    top   : imageScaleFactorY * result.Y + marginTop,
                    right : 0,
                    bottom: 0);
                rect.Fill = new SolidColorBrush(Colors.Yellow);
                rect.Opacity = 0.7;
                DrawCanvas.Children.Add(rect);
            }            
        }

        private void ConfidentResultControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DrawCanvas.Children.Clear();
        }
    }
}
