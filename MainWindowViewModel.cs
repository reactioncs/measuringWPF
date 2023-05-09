using ImageView.Core;
using System;
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
        //private BitmapSource mImgSource;
        //public BitmapSource ImgSource
        //{
        //    get { return mImgSource; }
        //    set
        //    {
        //        mImgSource = value;
        //        OnPropertyChanged();
        //    }
        //}

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

        private bool mIsMeasuring;
        public bool IsMeasuring
        {
            get { return mIsMeasuring; }
            set 
            { 
                mIsMeasuring = value;
                if (value == false)
                    mMeasureLine.Visibility = Visibility.Hidden;
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
            InfoBottomLeft = "Left";
            LengthDisplay = 0;
            IsMeasuring = true;

            LoadedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                mBorder = (Border)e.Source;
                mMeasureLine = (Line)mBorder.FindName("MeasureLine");
            });

            MouseLeftButtonDownCommand = new RelayCommand(o =>
            {
                MouseButtonEventArgs e = (MouseButtonEventArgs)o;
                Point p = e.GetPosition(mBorder);

                if (IsMeasuring)
                {
                    mMeasureLine.Visibility = Visibility.Visible;
                    mMeasureLine.X1 = p.X;
                    mMeasureLine.Y1 = p.Y;
                    mMeasureLine.X2 = p.X;
                    mMeasureLine.Y2 = p.Y;
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
                    mMeasureLine.X2 = p.X;
                    mMeasureLine.Y2 = p.Y;

                    LengthDisplay = distance(new Point(mMeasureLine.X1, mMeasureLine.Y1), p);
                }
            });
        }

        public static double distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }
}
