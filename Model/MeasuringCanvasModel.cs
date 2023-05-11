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
        public double Distance { get; set; }
        public bool IsMeasuring { get; set; }

        private Canvas mCanvas;
        private Brush Brush0;
        private Brush Brush1;
        private ObservableCollection<Line> Lines;
        private ObservableCollection<TextBlock> TextBlocks;
        private int CurrentLineCount;
        private double DistanceFixed;

        public MeasuringCanvasModel(Canvas canvas)
        {
            mCanvas = canvas;
            MeasuringCanvasInit();
        }

        public void MouseMove(Point p) 
        {
            Line lastLine = Lines.Last();
            TextBlock lastTextBlock = TextBlocks.Last();

            lastLine.X2 = p.X;  
            lastLine.Y2 = p.Y;

            Distance = DistanceFixed + GetDistance(lastLine);

            Canvas.SetLeft(lastTextBlock, p.X + 3);
            Canvas.SetTop(lastTextBlock, p.Y + 3);
            lastTextBlock.Text = string.Format("{0:F1} um", Distance);
        }

        public void MouseDown(Point p)
        {
            Point P1, P2;

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
                Canvas.SetLeft(newTextBlock, -50);
                Canvas.SetTop(newTextBlock, -50);
                mCanvas.Children.Add(newTextBlock);
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
            mCanvas.Children.Add(newLine);

            CurrentLineCount += 1;
            IsMeasuring = true;
            SetVisibility(Visibility.Visible);
        }

        public void MouseUp()
        {
            Line lastLine = Lines.Last();
            mCanvas.Children.Remove(lastLine);
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
                mCanvas.Children.Remove(lastTextBlock);
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
                mCanvas.Children.Remove(line);
            }
            Lines.Clear();
            foreach (TextBlock textBlock in TextBlocks)
            {
                mCanvas.Children.Remove(textBlock);
            }
            TextBlocks.Clear();

            SetVisibility(Visibility.Hidden);
        }

        public void SetVisibility(Visibility visibility)
        {
            mCanvas.Visibility = visibility;
        }

        private void MeasuringCanvasInit()
        {
            Lines = new ObservableCollection<Line>();
            TextBlocks = new ObservableCollection<TextBlock>();
            Brush0 = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            Brush1 = new SolidColorBrush(Color.FromRgb(200, 150, 150));

            Reset();
        }

        private double GetDistance(Line line) 
        {
            return Math.Sqrt((line.X1 - line.X2) * (line.X1 - line.X2) + (line.Y1 - line.Y2) * (line.Y1 - line.Y2));
        }
    }
}

