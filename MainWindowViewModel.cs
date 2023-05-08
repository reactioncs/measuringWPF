using ImageView.Core;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageView
{
    public class MainWindowViewModel : ObservableObject
    {
        private BitmapSource mImgSource;
        public BitmapSource ImgSource
        {
            get { return mImgSource; }
            set
            {
                mImgSource = value;
                OnPropertyChanged();
            }
        }

        private string mInfoBottomLeft;
        public string InfoBottomLeft
        {
            get { return mInfoBottomLeft; }
            set 
            {
                mInfoBottomLeft = value;
                OnPropertyChanged();
            }
        }
        private string mInfoBottomRight;
        public string InfoBottomRight
        {
            get { return mInfoBottomRight; }
            set
            {
                mInfoBottomRight = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand MouseLeftButtonDownCommand { get; set; }
        public RelayCommand MouseLeftButtonUpCommand { get; set; }
        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }

        private Border mBorder;
        private Line mMeasureLine;

        public MainWindowViewModel()
        {
            ImgSource = new BitmapImage(new System.Uri("C:/Users/zzz/Desktop/tt/IA.png"));

            InfoBottomLeft = "Left";
            InfoBottomRight = "Right";

            LoadedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                mBorder = (Border)e.Source;
                mMeasureLine = (Line)mBorder.FindName("MeasureLine");
            });

            MouseLeftButtonDownCommand = new RelayCommand(o =>
            {
                InfoBottomRight = "MouseLeftButtonDownCommand";

                MouseButtonEventArgs e = (MouseButtonEventArgs)o;
                Point p = e.GetPosition(mBorder);

                mMeasureLine.Visibility = Visibility.Visible;
                mMeasureLine.X1 = p.X;
                mMeasureLine.Y1 = p.Y;
                mMeasureLine.X2 = p.X;
                mMeasureLine.Y2 = p.Y;
            });

            MouseLeftButtonUpCommand = new RelayCommand(o =>
            {
                InfoBottomRight = "MouseLeftButtonUpCommand";

                MouseButtonEventArgs e = (MouseButtonEventArgs)o;
                Point p = e.GetPosition(mBorder);
            });

            MouseMoveCommand = new RelayCommand(o =>
            {
                MouseEventArgs e = (MouseEventArgs)o;
                if (e.LeftButton != MouseButtonState.Pressed)
                    return;

                Point p = e.GetPosition(mBorder);

                mMeasureLine.X2 = p.X;
                mMeasureLine.Y2 = p.Y;

                InfoBottomLeft = $"{(int)p.X}, {(int)p.Y}";
            });
        }
    }
}
