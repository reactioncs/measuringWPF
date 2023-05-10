using ImageView.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageView.Model;

namespace ImageView
{
    public class MainWindowViewModel : ObservableObject
    {
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

        private double LengthDisplay
        {
            set => InfoBottomRight = string.Format("{0:F1}", value);
        }

        private Point CurrentPosDisplay
        {
            set => InfoBottomLeft = $"{(int)value.X}, {(int)value.Y}";
        }

        public LineModel MeasuringLine { get; set; }

        private bool mIsMeasuring;
        public bool IsMeasuring
        {
            get { return mIsMeasuring; }
            set 
            { 
                mIsMeasuring = value;
                if (value == false)
                    MeasuringLine.Visibility = Visibility.Hidden;
            }
        }

        private Border mBorder;

        public RelayCommand MouseLeftButtonDownCommand { get; set; }
        public RelayCommand MouseLeftButtonUpCommand { get; set; }
        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }

        public MainWindowViewModel()
        {
            InfoBottomLeft = "Left";
            LengthDisplay = 0;
            IsMeasuring = true;
            MeasuringLine = new LineModel();

            LoadedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                mBorder = (Border)e.Source;
            });

            MouseLeftButtonDownCommand = new RelayCommand(o =>
            {
                MouseButtonEventArgs e = (MouseButtonEventArgs)o;
                Point p = e.GetPosition(mBorder);

                if (IsMeasuring)
                {
                    MeasuringLine.P1 = p;
                    MeasuringLine.P2 = p;
                    MeasuringLine.Visibility = Visibility.Visible;
                }
            });

            MouseLeftButtonUpCommand = new RelayCommand(o =>
            {
            });

            MouseMoveCommand = new RelayCommand(o =>
            {
                MouseEventArgs e = (MouseEventArgs)o;
                Point p = e.GetPosition(mBorder);

                CurrentPosDisplay = p;

                if (e.LeftButton == MouseButtonState.Pressed && IsMeasuring)
                {
                    MeasuringLine.P2 = p;

                    LengthDisplay = MeasuringLine.Distance;
                }
            });
        }
    }
}
