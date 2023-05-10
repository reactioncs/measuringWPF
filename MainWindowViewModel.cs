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

        private Cursor mCursor;
        public Cursor Cursor
        {
            get { return mCursor; }
            set
            {
                mCursor = value;
                OnPropertyChanged();
            }
        }

        private double LengthDisplay
        {
            set => InfoBottomRight = string.Format("{0:F2}", value);
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
                {
                    mMeasuringCanvasModel?.SetVisibility(Visibility.Hidden);
                    Cursor = Cursors.Arrow;
                }
                else
                {
                    Cursor = Cursors.Cross;
                }
            }
        }

        private Border mBorder;
        private Canvas mMeasuringCanvas;
        private MeasuringCanvasModel mMeasuringCanvasModel;

        public RelayCommand MouseLeftButtonDownCommand { get; set; }
        public RelayCommand MouseLeftButtonUpCommand { get; set; }
        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }

        public MainWindowViewModel()
        {
            InfoBottomLeft = "Left";
            LengthDisplay = 0;
            IsMeasuring = false;
            Cursor = Cursors.Arrow;

            LoadedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                Grid grid = (Grid)e.Source;

                mBorder = (Border)grid.FindName("Border");
                mMeasuringCanvas = (Canvas)grid.FindName("MeasuringCanvas");
                mMeasuringCanvasModel = new MeasuringCanvasModel(mMeasuringCanvas);
            });

            MouseLeftButtonDownCommand = new RelayCommand(o =>
            {
                MouseButtonEventArgs e = (MouseButtonEventArgs)o;
                Point p = e.GetPosition(mBorder);

                if (IsMeasuring)
                {
                    mMeasuringCanvasModel.SetVisibility(Visibility.Visible);
                    mMeasuringCanvasModel.SetStartPoint(p);
                    mMeasuringCanvasModel.SetEndPoint(p);
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
                    mMeasuringCanvasModel.SetEndPoint(p);

                    LengthDisplay = mMeasuringCanvasModel.Distance;
                }

                //Thread.Sleep(100);
            });
        }
    }
}
