using ImageView.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private Area mCurrentArea;
        public Area CurrentArea
        {
            get { return mCurrentArea; }
            set
            {
                mCurrentArea = value;
                OnPropertyChanged();
            }
        }

        private double LengthDisplay
        {
            set => InfoBottomRight = string.Format("{0:F2}", value);
        }

        public Point CurrentPosDisplay
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
                Cursor = value ? Cursors.Cross : Cursors.Arrow;
                OnPropertyChanged();
            }
        }

        public RelayCommand MouseLeftButtonDownCommand { get; set; }
        public RelayCommand MouseRightButtonUpCommand { get; set; }
        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand SizeChangedCommand { get; set; }

        public MainWindowViewModel()
        {
            InfoBottomLeft = "Left";
            LengthDisplay = 0;
            IsMeasuringMode = true;
            CurrentArea = new Area();

            LoadedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                Grid grid = (Grid)e.Source;

                CurrentArea = new Area()
                {
                    Width = grid.ActualWidth,
                    Height = grid.ActualHeight
                };
            });

            SizeChangedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                Grid grid = (Grid)e.Source;

                CurrentArea = new Area()
                {
                    Width = grid.ActualWidth,
                    Height = grid.ActualHeight
                };
            });

            MouseMoveCommand = new RelayCommand(o =>
            {
                MouseEventArgs e = (MouseEventArgs)o;
            });
        }
    }

    public class Area
    {
        public double Height { get; set; }
        public double Width { get; set; }

        public Area()
        {
            Height = 0;
            Width = 0;
        }
    }
}