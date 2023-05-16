using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageView
{
    /// <summary>
    /// Interaction logic for ImageBoxView.xaml
    /// </summary>
    public partial class ImageBoxView : UserControl
    {
        public ImageBoxView()
        {
            InitializeComponent();
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageBoxView), new PropertyMetadata(null));
    }
}
