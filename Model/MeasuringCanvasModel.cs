using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ImageView.Model
{
    public class MeasuringCanvasModel
    {
        private Canvas mCanvas;
        private Line mLine;
        private TextBlock mTextBlock;

        private Point P1;
        private Point P2;

        public double Distance
        {
            get => Math.Sqrt((P1.X - P2.X) * (P1.X - P2.X) + (P1.Y - P2.Y) * (P2.Y - P2.Y));
        }

        public MeasuringCanvasModel(Canvas canvas) 
        {
            mCanvas = canvas;

            MeasuringCanvasInit();
        }

        public void SetStartPoint(Point p)
        {
            P1 = p;

            mLine.X1 = p.X;
            mLine.Y1 = p.Y;
        }

        public void SetEndPoint(Point p)
        {
            P2 = p;

            mLine.X2 = p.X;
            mLine.Y2 = p.Y;

            Canvas.SetLeft(mTextBlock, p.X + 3);
            Canvas.SetTop(mTextBlock, p.Y + 3);
            mTextBlock.Text = string.Format("{0:F1}", Distance);
        }

        public void SetVisibility(Visibility visibility)
        {
            mCanvas.Visibility = visibility;
        }

        private void MeasuringCanvasInit()
        {
            mCanvas.Visibility = Visibility.Hidden;

            mLine = new Line();
            mTextBlock = new TextBlock();

            Brush redBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

            mLine.X1 = 0;
            mLine.Y1 = 0;
            mLine.X2 = 10;
            mLine.Y2 = 10;
            mLine.Stroke = redBrush;
            mLine.StrokeThickness = 2;

            mTextBlock.Text = string.Format("{0:F1}", Distance);
            mTextBlock.Foreground = redBrush;
            Canvas.SetLeft(mTextBlock, 10);
            Canvas.SetTop(mTextBlock, 10);

            mCanvas.Children.Add(mLine);
            mCanvas.Children.Add(mTextBlock);
        }
    }
}
