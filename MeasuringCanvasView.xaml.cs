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

        public double CurrentAbsoluteLength
        {
            get { return (double)GetValue(CurrentAbsoluteLengthProperty); }
            set { SetValue(CurrentAbsoluteLengthProperty, value); }
        }

        public static readonly DependencyProperty CurrentAbsoluteLengthProperty =
            DependencyProperty.Register("CurrentAbsoluteLength", typeof(double), typeof(MeasuringCanvasView), new PropertyMetadata(0.0));

        public bool IsMeasuringMode
        {
            get { return (bool)GetValue(IsMeasuringModeProperty); }
            set { SetValue(IsMeasuringModeProperty, value); }
        }

        public static readonly DependencyProperty IsMeasuringModeProperty =
            DependencyProperty.Register("IsMeasuringMode", typeof(bool), typeof(MeasuringCanvasView), new PropertyMetadata(true, new PropertyChangedCallback((sender, e) =>
            {
                MeasuringCanvasView measuringCanvasView = (MeasuringCanvasView)sender;
                if ((bool)e.NewValue)
                {
                    measuringCanvasView.ModeIndicator.Visibility = Visibility.Visible;
                    measuringCanvasView.CountIndicator.Visibility = Visibility.Visible;
                }
                else
                {
                    measuringCanvasView.ModeIndicator.Visibility = Visibility.Hidden;
                    if (measuringCanvasView.Measurements.Count <= 1 && !measuringCanvasView.IsMeasuringMode)
                        measuringCanvasView.CountIndicator.Visibility = Visibility.Hidden;
                }
            })));

        public byte ClearMeasurementCount
        {
            get { return (byte)GetValue(ClearMeasurementCountProperty); }
            set { SetValue(ClearMeasurementCountProperty, value); }
        }

        public static readonly DependencyProperty ClearMeasurementCountProperty =
            DependencyProperty.Register("ClearMeasurementCount", typeof(byte), typeof(MeasuringCanvasView), new PropertyMetadata((byte)0, new PropertyChangedCallback((sender, e) =>
            {
                MeasuringCanvasView measuringCanvasView = (MeasuringCanvasView)sender;
                measuringCanvasView.Reset();
                if (measuringCanvasView.Measurements.Count <= 1 && !measuringCanvasView.IsMeasuringMode)
                    measuringCanvasView.CountIndicator.Visibility = Visibility.Hidden;
            })));

        public Area CurrentArea
        {
            private get { return (Area)GetValue(CurrentAreaProperty); }
            set { SetValue(CurrentAreaProperty, value); }
        }

        public static readonly DependencyProperty CurrentAreaProperty =
            DependencyProperty.Register("CurrentArea", typeof(Area), typeof(MeasuringCanvasView), new PropertyMetadata(new Area(), new PropertyChangedCallback((sender, e) => OnCurrentAreaChanging(sender, e))));

        #endregion

        public bool IsMeasuring { get; set; }

        private readonly Brush Brush0;
        private readonly Brush Brush1;

        private readonly ObservableCollection<Measurement> Measurements;

        public MeasuringCanvasView()
        {
            InitializeComponent();

            Measurements = new ObservableCollection<Measurement>();
            Brush0 = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Brush1 = new SolidColorBrush(Color.FromRgb(200, 150, 150));

            Reset();
        }

        public static void OnCurrentAreaChanging(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Area newArea = (Area)e.NewValue;
            MeasuringCanvasView measuringCanvasView = (MeasuringCanvasView)sender;

            foreach (Measurement measurement in measuringCanvasView.Measurements)
            {
                foreach (MeasuringLine line in measurement.Lines)
                {
                    line.UpdateCanvasPosition(newArea);
                }

                if (measurement.Lines.Count == 0) continue;
                MeasuringLine lastLine = measurement.Lines.Last();
                Canvas.SetLeft(measurement.LengthIndicator, lastLine.Line.X2 + 3);
                Canvas.SetTop(measurement.LengthIndicator, lastLine.Line.Y2 + 3);
            }
        }

        private void DoMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMeasuringMode) 
                return;

            Point p = e.GetPosition(sender as FrameworkElement);
            CurrentAbsolutePosition = new()
            {
                X = p.X / CurrentArea.Width,
                Y = p.Y / CurrentArea.Height
            };

            if (!IsMeasuring) 
                return;

            Measurement measurement = Measurements.Last();
            MeasuringLine lastLine = measurement.Lines.Last();

            lastLine.P2 = p;
            lastLine.UpdateAbsolutePosition(CurrentArea);
            measurement.UpdateLength();
            CurrentAbsoluteLength = measurement.TotalLength;

            Canvas.SetLeft(measurement.LengthIndicator, p.X + 3);
            Canvas.SetTop(measurement.LengthIndicator, p.Y + 3);
        }

        private void DoMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsMeasuringMode || !IsMeasuring) 
                return;

            Measurement measurement = Measurements.Last();
            MeasuringLine lastLine = measurement.Lines.Last();
            MeasuringCanvas.Children.Remove(lastLine.Line);
            measurement.Lines.Remove(lastLine);

            if (measurement.Lines.Count > 0)
            {
                MeasuringLine nextLastLine = measurement.Lines.Last();
                lastLine.UpdateAbsolutePosition(CurrentArea);
                measurement.UpdateLength();
                CurrentAbsoluteLength = measurement.TotalLength;
                Canvas.SetLeft(measurement.LengthIndicator, nextLastLine.P2.X);
                Canvas.SetTop(measurement.LengthIndicator, nextLastLine.P2.Y);
            }
            else
            {
                measurement.RemoveFromCanvas(MeasuringCanvas);
                Measurements.Remove(measurement);
            }

            IsMeasuring = false;

            e.Handled = true;
        }

        private void DoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMeasuringMode)
                return;

            if (!IsMeasuring)
            {
                Measurements.Add(new(Brush0));
                CountIndicator.Text = $"Total Measurement: {Measurements.Count - 1}";
            }

            Point p = e.GetPosition(sender as FrameworkElement);

            Measurement measurement = Measurements.Last();

            Point P1, P2;

            if (measurement.Lines.Count > 0)
            {
                MeasuringLine lastLine = measurement.Lines.Last();
                P1 = lastLine.P2;

                lastLine.Line.Stroke = Brush0;
                lastLine.UpdateAbsolutePosition(CurrentArea);
            }
            else
            {
                P1 = p;

                Canvas.SetLeft(measurement.LengthIndicator, p.X + 3);
                Canvas.SetTop(measurement.LengthIndicator, p.Y + 3);
                MeasuringCanvas.Children.Add(measurement.LengthIndicator);
            }
            P2 = p;

            MeasuringLine newLine = new(P1, P2, Brush1);
            newLine.UpdateAbsolutePosition(CurrentArea);
            measurement.Lines.Add(newLine);
            MeasuringCanvas.Children.Add(newLine.Line);

            IsMeasuring = true;
        }

        public void Reset()
        {
            IsMeasuring = false;

            foreach (Measurement measurement in Measurements)
            {
                measurement.RemoveFromCanvas(MeasuringCanvas);
            }

            Measurements.Clear();

            Measurements.Add(new(Brush0));

            CountIndicator.Text = "Total Measurement: 0";
        }
    }

    public class Measurement
    {
        public ObservableCollection<MeasuringLine> Lines;
        public TextBlock LengthIndicator;

        public readonly Canvas MeasuringCanvas;
        public readonly Brush Brush0;
        public readonly Brush Brush1;

        public double TotalLength
        {
            get
            {
                double len = 0;
                foreach (MeasuringLine line in Lines)
                {
                    len += line.DistanceAbsolute;
                }
                return len;
            }
        }

        public Measurement(Brush brush)
        {
            Lines = new();
            LengthIndicator = new()
            {
                Text = "0.000",
                Foreground = brush,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 16
            };
        }

        public void UpdateLength()
        {
            LengthIndicator.Text = string.Format("{0:F3}", TotalLength);
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            canvas.Children.Remove(LengthIndicator);
            foreach (MeasuringLine line in Lines)
            {
                canvas.Children.Remove(line.Line);
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

        public double Distance => DistanceBetweenTwoPoints(P1, P2);
        public double DistanceAbsolute => DistanceBetweenTwoPoints(P1a, P2a);

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

        private static double DistanceBetweenTwoPoints(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }
}
