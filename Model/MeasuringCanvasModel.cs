using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageView.Model
{
    public class MeasuringCanvasModel
    {
        public double DistanceRefresh { get; set; }
        public double DistanceFixed { get; set; }
        public bool IsMeasuring { get; set; }

        private readonly Canvas CurrentCanvas;
        private double CanvasHeight;
        private double CanvasWidth;
        private Brush Brush0;
        private Brush Brush1;
        private ObservableCollection<Line> Lines;
        private ObservableCollection<TextBlock> TextBlocks;
        private int CurrentLineCount;

        public MeasuringCanvasModel(Canvas canvas)
        {
            CurrentCanvas = canvas;
            CanvasHeight = canvas.ActualHeight;
            CanvasWidth = canvas.ActualWidth;

            Lines = new ObservableCollection<Line>();
            TextBlocks = new ObservableCollection<TextBlock>();
            Brush0 = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Brush1 = new SolidColorBrush(Color.FromRgb(200, 150, 150));

            Reset();
        }

        public void MouseMove(Point p) 
        {
            Line lastLine = Lines.Last();
            TextBlock lastTextBlock = TextBlocks.Last();

            lastLine.X2 = p.X;  
            lastLine.Y2 = p.Y;

            DistanceRefresh = DistanceFixed + GetDistance(lastLine);

            Canvas.SetLeft(lastTextBlock, p.X + 3);
            Canvas.SetTop(lastTextBlock, p.Y + 3);
            lastTextBlock.Text = string.Format("{0:F1} um", DistanceRefresh);
        }

        public void MouseDown(Point p)
        {
            Point P1, P2;

            if (IsMeasuring == false)
            {
                Reset();
            }

            if (CurrentLineCount > 0) 
            {
                Line lastLine = Lines.Last();
                P1.X = lastLine.X2;
                P1.Y = lastLine.Y2;
                DistanceFixed += GetDistance(lastLine);

                Lines.Last().Stroke = Brush0;
            }
            else
            {
                P1 = p;

                TextBlock newTextBlock = new()
                {
                    Text = "0.0 um",
                    Foreground = Brush0,
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 16
                };
                TextBlocks.Add(newTextBlock);
                Canvas.SetLeft(newTextBlock, p.X + 3);
                Canvas.SetTop(newTextBlock, p.Y + 3);
                CurrentCanvas.Children.Add(newTextBlock);
            }
            P2 = p;

            Line newLine = new() 
            {
                X1 = P1.X,
                Y1 = P1.Y,
                X2 = P2.X,
                Y2 = P2.Y,
                Stroke = Brush1,
                StrokeThickness = 3
            };
            Lines.Add(newLine);
            CurrentCanvas.Children.Add(newLine);

            CurrentLineCount += 1;
            IsMeasuring = true;
            SetVisibility(Visibility.Visible);
        }

        public void MouseUp()
        {
            Line lastLine = Lines.Last();
            CurrentCanvas.Children.Remove(lastLine);
            Lines.Remove(lastLine);
            CurrentLineCount -= 1; 

            if (CurrentLineCount > 0)
            {
                Line nextLastLine = Lines.Last();
                TextBlock lastTextBlock = TextBlocks.Last();
                lastTextBlock.Text = string.Format("{0:F1} um", DistanceFixed);
                Canvas.SetLeft(lastTextBlock, nextLastLine.X2);
                Canvas.SetTop(lastTextBlock, nextLastLine.Y2);
            }
            else
            {
                TextBlock lastTextBlock = TextBlocks.Last();
                CurrentCanvas.Children.Remove(lastTextBlock);
                TextBlocks.Remove(lastTextBlock);
            }

            IsMeasuring = false;
            CurrentLineCount = 0;
        }

        public void Reset()
        {
            IsMeasuring = false;
            CurrentLineCount = 0;
            DistanceFixed = 0;

            foreach (Line line in Lines) 
            {
                CurrentCanvas.Children.Remove(line);
            }
            Lines.Clear();
            foreach (TextBlock textBlock in TextBlocks)
            {
                CurrentCanvas.Children.Remove(textBlock);
            }
            TextBlocks.Clear();

            SetVisibility(Visibility.Hidden);
        }

        public void Resize()
        {
            double resizeRatioHeight = CurrentCanvas.ActualHeight / CanvasHeight;
            double resizeRatioWidth = CurrentCanvas.ActualWidth / CanvasWidth;

            foreach (Line line in Lines)
            {
                line.X1 *= resizeRatioWidth;
                line.Y1 *= resizeRatioHeight;
                line.X2 *= resizeRatioWidth;
                line.Y2 *= resizeRatioHeight;
            }
            foreach (TextBlock textBlock in TextBlocks)
            {
                double textBlockX = Canvas.GetLeft(textBlock);
                double textBlockY = Canvas.GetTop(textBlock);
                Canvas.SetLeft(textBlock, textBlockX * resizeRatioWidth);
                Canvas.SetTop(textBlock, textBlockY * resizeRatioHeight);
            }

            CanvasHeight = CurrentCanvas.ActualHeight;
            CanvasWidth = CurrentCanvas.ActualWidth;
        }

        public void SetVisibility(Visibility visibility) => CurrentCanvas.Visibility = visibility;

        private static double GetDistance(Line line) 
        {
            return Math.Sqrt((line.X1 - line.X2) * (line.X1 - line.X2) + (line.Y1 - line.Y2) * (line.Y1 - line.Y2));
        }
    }
}

