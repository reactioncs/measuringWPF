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

        private bool mIsMeasuringMode;
        public bool IsMeasuringMode
        {
            get { return mIsMeasuringMode; }
            set 
            { 
                mIsMeasuringMode = value;
                if (value == false)
                {
                    MeasuringCanvasModel?.SetVisibility(Visibility.Hidden);
                    MeasuringCanvasModel?.Reset();
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

        public MeasuringCanvasModel MeasuringCanvasModel { get; set; }

        public RelayCommand MouseLeftButtonDownCommand { get; set; }
        public RelayCommand MouseRightButtonUpCommand { get; set; }
        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }

        public MainWindowViewModel()
        {
            InfoBottomLeft = "Left";
            LengthDisplay = 0;
            IsMeasuringMode = true;

            LoadedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                Grid grid = (Grid)e.Source;

                mBorder = (Border)grid.FindName("Border");
                mMeasuringCanvas = (Canvas)grid.FindName("MeasuringCanvas");
                MeasuringCanvasModel = new MeasuringCanvasModel(mMeasuringCanvas);
            });

            MouseLeftButtonDownCommand = new RelayCommand(o =>
            {
                MouseButtonEventArgs e = (MouseButtonEventArgs)o;
                Point p = e.GetPosition(mBorder);

                if (IsMeasuringMode)
                {
                    MeasuringCanvasModel.MouseDown(p);
                }
            });

            MouseMoveCommand = new RelayCommand(o =>
            {
                MouseEventArgs e = (MouseEventArgs)o;
                Point p = e.GetPosition(mBorder);

                CurrentPosDisplay = p;

                if (MeasuringCanvasModel.IsMeasuring && IsMeasuringMode)
                {
                    MeasuringCanvasModel.MouseMove(p);

                    LengthDisplay = MeasuringCanvasModel.Distance;
                }
            });

            MouseRightButtonUpCommand = new RelayCommand(o =>
            {
                MouseButtonEventArgs e = (MouseButtonEventArgs)o;

                if (MeasuringCanvasModel.IsMeasuring && IsMeasuringMode)
                {
                    MeasuringCanvasModel.MouseUp();
                    e.Handled = true;
                }
            });
        }
    }
}
