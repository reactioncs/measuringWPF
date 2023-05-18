using ImageView.Core;
using System.Windows;
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

        public double LengthDisplay
        {
            set => InfoBottomRight = string.Format("{0:F2}", value);
        }

        public Point CurrentPosDisplay
        {
            set => InfoBottomLeft = $"{(int)(value.X * 1000)}‰, {(int)(value.Y * 1000)}‰";
        }

        private bool mMeasuringMode;
        public bool IsMeasuringMode
        {
            get { return mMeasuringMode; }
            set
            {
                mMeasuringMode = value;
                Cursor = value ? Cursors.Cross : Cursors.Arrow;
                OnPropertyChanged();
            }
        }

        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand SizeChangedCommand { get; set; }

        public MainWindowViewModel()
        {
            LengthDisplay = 0;
            IsMeasuringMode = true;
            CurrentArea = new Area();

            LoadedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                FrameworkElement element = (FrameworkElement)e.Source;
                if (element == null) return;

                CurrentArea = new()
                {
                    Width = element.ActualWidth,
                    Height = element.ActualHeight
                };
            });

            SizeChangedCommand = new RelayCommand(o =>
            {
                RoutedEventArgs e = (RoutedEventArgs)o;
                FrameworkElement element = (FrameworkElement)e.Source;
                if (element == null) return;

                CurrentArea = new()
                {
                    Width = element.ActualWidth,
                    Height = element.ActualHeight
                };
            });

            // this will trigger after Children's MouseMoveCommand
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