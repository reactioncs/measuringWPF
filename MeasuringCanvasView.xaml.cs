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
        public Point CurrentPosition
        {
            get { return (Point)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        public static readonly DependencyProperty CurrentPositionProperty =
            DependencyProperty.Register("CurrentPosition", typeof(Point), typeof(MeasuringCanvasView), new PropertyMetadata(new Point(0, 0)));

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
        private TextBlock TextBlock;

        public MeasuringCanvasView()
        {
            InitializeComponent();

            Lines = new ObservableCollection<MeasuringLine>();
            TextBlock = null;
            Brush0 = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Brush1 = new SolidColorBrush(Color.FromRgb(200, 150, 150));

            Reset();
        }

        public static void OnCurrentChanging(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Area newArea = (Area)e.NewValue;
            Area oldArea = (Area)e.OldValue;
            MeasuringCanvasView measuringCanvasView = (MeasuringCanvasView)sender;

            foreach (var line in measuringCanvasView.Lines) 
            {
                line.Line.X1 = line.Line.X1 * newArea.Width / oldArea.Width;
                line.Line.X2 = line.Line.X2 * newArea.Width / oldArea.Width;
                line.Line.Y1 = line.Line.Y1 * newArea.Height / oldArea.Height;
                line.Line.Y2 = line.Line.Y2 * newArea.Height / oldArea.Height;
            }

            if (measuringCanvasView.Lines.Count == 0)
                return;
            Canvas.SetLeft(measuringCanvasView.TextBlock, measuringCanvasView.Lines.Last().Line.X2 + 3);
            Canvas.SetTop(measuringCanvasView.TextBlock, measuringCanvasView.Lines.Last().Line.Y2 + 3);
        }

        private void DoMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsMeasuringMode) 
                return;

            Point p = e.GetPosition(sender as Canvas);
            CurrentPosition = p;

            if (!IsMeasuring) 
                return;

            MeasuringLine lastLine = Lines.Last();

            lastLine.P2 = p;

            DistanceRefresh = DistanceFixed + lastLine.Distance;

            Canvas.SetLeft(TextBlock, p.X + 3);
            Canvas.SetTop(TextBlock, p.Y + 3);
            TextBlock.Text = string.Format("{0:F1} um", DistanceRefresh);
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
                TextBlock.Text = string.Format("{0:F1} um", DistanceFixed);
                Canvas.SetLeft(TextBlock, nextLastLine.P2.X);
                Canvas.SetTop(TextBlock, nextLastLine.P2.Y);
            }
            else
            {
                MeasuringCanvas.Children.Remove(TextBlock);
                TextBlock = null;
            }

            IsMeasuring = false;

            e.Handled = true;
        }

        private void DoMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsMeasuringMode) 
                return;

            Point p = e.GetPosition(sender as Canvas);

            Point P1;

            if (Lines.Count > 0)
            {
                MeasuringLine lastLine = Lines.Last();
                P1 = lastLine.P2;
                DistanceFixed += lastLine.Distance;

                lastLine.Line.Stroke = Brush0;
            }
            else
            {
                P1 = p;

                TextBlock = new TextBlock()
                {
                    Text = "0.0 um",
                    Foreground = Brush0,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 16
                };
                Canvas.SetLeft(TextBlock, p.X + 3);
                Canvas.SetTop(TextBlock, p.Y + 3);
                MeasuringCanvas.Children.Add(TextBlock);
            }
            Point P2 = p;

            MeasuringLine newLine = new MeasuringLine(P1, P2, Brush1);
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

            if (TextBlock != null) 
            {
                MeasuringCanvas.Children.Remove(TextBlock);
                TextBlock = null;
            }
        }
    }

    public class MeasuringLine
    {
        public Line Line { get; set; }

        public Point P1
        {
            get { return new Point(Line.X1, Line.Y1); }
            set
            {
                Line.X1 = value.X;
                Line.Y1 = value.Y;
            }
        }

        public Point P2
        {
            get { return new Point(Line.X2, Line.Y2); }
            set
            {
                Line.X2 = value.X;
                Line.Y2 = value.Y;
            }
        }

        public Point P1Real { get; set; }
        public Point P2Real { get; set; }

        public double Distance => Math.Sqrt((Line.X1 - Line.X2) * (Line.X1 - Line.X2) + (Line.Y1 - Line.Y2) * (Line.Y1 - Line.Y2));

        public MeasuringLine(Point P1, Point P2, Brush brush)
        {
            Line = new Line()
            {
                Stroke = brush,
                StrokeThickness = 3
            };
            this.P1 = P1;
            this.P2 = P2;
        }

        //private Point CanvasToReal(Point P, Area area, double canvasHeight, double canvasWidth)
        //{
        //}
    }
}
