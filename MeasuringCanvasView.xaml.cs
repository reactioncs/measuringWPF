using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageView
{
    /// <summary>
    /// Interaction logic for MeasuringCanvasView.xaml
    /// </summary>
    public partial class MeasuringCanvasView : UserControl
    {
        #region Properties
        public Point CurrentAbsolutePosition
        {
            get { return (Point)GetValue(CurrentAbsolutePositionProperty); }
            set { SetValue(CurrentAbsolutePositionProperty, value); }
        }

        public static readonly DependencyProperty CurrentAbsolutePositionProperty =
            DependencyProperty.Register("CurrentAbsolutePosition", typeof(Point), typeof(MeasuringCanvasView), new PropertyMetadata(new Point(0, 0)));

        public bool IsMeasuringMode
        {
            get { return (bool)GetValue(IsMeasuringModeProperty); }
            set { SetValue(IsMeasuringModeProperty, value); }
        }

        public static readonly DependencyProperty IsMeasuringModeProperty =
            DependencyProperty.Register("IsMeasuringMode", typeof(bool), typeof(MeasuringCanvasView), new PropertyMetadata(true, new PropertyChangedCallback((sender, e) =>
            {
                Visibility visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Hidden;
                MeasuringCanvasView measuringCanvasView = (MeasuringCanvasView)sender;
                measuringCanvasView.Visibility = visibility;
                measuringCanvasView.Reset();
            })));

        public Area CurrentArea
        {
            private get { return (Area)GetValue(CurrentAreaProperty); }
            set { SetValue(CurrentAreaProperty, value); }
        }

        public static readonly DependencyProperty CurrentAreaProperty =
            DependencyProperty.Register("CurrentArea", typeof(Area), typeof(MeasuringCanvasView), new PropertyMetadata(new Area(), new PropertyChangedCallback((sender, e) => OnCurrentChanging(sender, e))));
        #endregion

        public double DistanceRefresh { get; set; }
        public double DistanceFixed { get; set; }
        public bool IsMeasuring { get; set; }

        private readonly Brush Brush0;
        private readonly Brush Brush1;
        private readonly ObservableCollection<MeasuringLine> Lines;
        private TextBlock DistanceIndicator;

        public MeasuringCanvasView()
        {
            InitializeComponent();

            Lines = new ObservableCollection<MeasuringLine>();
            DistanceIndicator = null;
            Brush0 = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Brush1 = new SolidColorBrush(Color.FromRgb(200, 150, 150));

            Reset();
        }

        public static void OnCurrentChanging(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Area newArea = (Area)e.NewValue;
            MeasuringCanvasView measuringCanvasView = (MeasuringCanvasView)sender;

            if (!measuringCanvasView.IsMeasuringMode) return;

            foreach (var line in measuringCanvasView.Lines)
            {
                line.UpdateCanvasPosition(newArea);
            }

            if (measuringCanvasView.Lines.Count == 0) return;
            MeasuringLine lastLine = measuringCanvasView.Lines.Last();
            Canvas.SetLeft(measuringCanvasView.DistanceIndicator, lastLine.Line.X2 + 3);
            Canvas.SetTop(measuringCanvasView.DistanceIndicator, lastLine.Line.Y2 + 3);
        }

        private void DoMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMeasuringMode) 
                return;

            Point p = e.GetPosition(sender as Canvas);
            CurrentAbsolutePosition = new()
            {
                X = p.X / CurrentArea.Width,
                Y = p.Y / CurrentArea.Height
            };

            if (!IsMeasuring) 
                return;

            MeasuringLine lastLine = Lines.Last();

            lastLine.P2 = p;

            DistanceRefresh = DistanceFixed + lastLine.Distance;

            Canvas.SetLeft(DistanceIndicator, p.X + 3);
            Canvas.SetTop(DistanceIndicator, p.Y + 3);
            DistanceIndicator.Text = string.Format("{0:F1}", DistanceRefresh);
        }

        private void DoMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsMeasuringMode || !IsMeasuring) 
                return;

            MeasuringLine lastLine = Lines.Last();
            MeasuringCanvas.Children.Remove(lastLine.Line);
            Lines.Remove(lastLine);

            if (Lines.Count > 0)
            {
                MeasuringLine nextLastLine = Lines.Last();
                DistanceIndicator.Text = string.Format("{0:F1}", DistanceFixed);
                Canvas.SetLeft(DistanceIndicator, nextLastLine.P2.X);
                Canvas.SetTop(DistanceIndicator, nextLastLine.P2.Y);
            }
            else
            {
                MeasuringCanvas.Children.Remove(DistanceIndicator);
                DistanceIndicator = null;
            }

            IsMeasuring = false;

            e.Handled = true;
        }

        private void DoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMeasuringMode)
                return;

            if (!IsMeasuring)
                Reset();

            Point p = e.GetPosition(sender as FrameworkElement);

            Point P1, P2;

            if (Lines.Count > 0)
            {
                MeasuringLine lastLine = Lines.Last();
                P1 = lastLine.P2;
                DistanceFixed += lastLine.Distance;

                lastLine.Line.Stroke = Brush0;
                lastLine.UpdateAbsolutePosition(CurrentArea);
            }
            else
            {
                P1 = p;

                DistanceIndicator = new TextBlock()
                {
                    Text = "0.0",
                    Foreground = Brush0,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 16
                };
                Canvas.SetLeft(DistanceIndicator, p.X + 3);
                Canvas.SetTop(DistanceIndicator, p.Y + 3);
                MeasuringCanvas.Children.Add(DistanceIndicator);
            }
            P2 = p;

            MeasuringLine newLine = new(P1, P2, Brush1);
            newLine.UpdateAbsolutePosition(CurrentArea);
            Lines.Add(newLine);
            MeasuringCanvas.Children.Add(newLine.Line);

            IsMeasuring = true;
        }

        public void Reset()
        {
            IsMeasuring = false;
            DistanceFixed = 0;

            foreach (MeasuringLine line in Lines)
            {
                MeasuringCanvas.Children.Remove(line.Line);
            }
            Lines.Clear();

            if (DistanceIndicator != null) 
            {
                MeasuringCanvas.Children.Remove(DistanceIndicator);
                DistanceIndicator = null;
            }
        }
    }

    public class MeasuringLine
    {
        public Line Line { get; set; }

        public Point P1
        {
            get => new Point(Line.X1, Line.Y1);
            set
            {
                Line.X1 = value.X;
                Line.Y1 = value.Y;
            }
        }

        public Point P2
        {
            get => new Point(Line.X2, Line.Y2);
            set
            {
                Line.X2 = value.X;
                Line.Y2 = value.Y;
            }
        }

        // Absolute Position
        public Point P1a { get; set; }
        public Point P2a { get; set; }

        public double Distance => Math.Sqrt((Line.X1 - Line.X2) * (Line.X1 - Line.X2) + (Line.Y1 - Line.Y2) * (Line.Y1 - Line.Y2));

        public MeasuringLine(Point P1, Point P2, Brush brush)
        {
            Line = new ()
            {
                Stroke = brush,
                StrokeThickness = 3
            };
            this.P1 = P1;
            this.P2 = P2;
        }

        public void UpdateAbsolutePosition(Area area)
        {
            P1a = CanvasToAbsolute(P1, area);
            P2a = CanvasToAbsolute(P2, area);
        }

        public void UpdateCanvasPosition(Area area)
        {
            P1 = AbsoluteToCanvas(P1a, area);
            P2 = AbsoluteToCanvas(P2a, area);
        }

        private static Point AbsoluteToCanvas(Point p, Area area) => new()
        {
            X = area.Width * p.X,
            Y = area.Height * p.Y
        };

        private static Point CanvasToAbsolute(Point p, Area area) => new()
        {
            X = p.X / area.Width,
            Y = p.Y / area.Height
        };
    }
}
