using ImageView.Core;
using System;
using System.Windows;

namespace ImageView.Model
{
    public class LineModel : ObservableObject
    {
        private Point mP1;
        public Point P1
        {
            get { return mP1; }
            set
            {
                mP1 = value;
                OnPropertyChanged();
            }
        }

        private Point mP2;
        public Point P2
        {
            get { return mP2; }
            set
            {
                mP2 = value;
                OnPropertyChanged();
            }
        }

        private Visibility mVisibility;
        public Visibility Visibility
        {
            get { return mVisibility; }
            set
            {
                mVisibility = value;
                OnPropertyChanged();
            }
        }

        public double Distance
        {
            get => Math.Sqrt((P1.X - P2.X) * (P1.X - P2.X) + (P1.Y - P2.Y) * (P2.Y - P2.Y));
        }

        public LineModel()
        {
            P1 = new Point(0, 0);
            P2 = new Point(0, 0);
            Visibility = Visibility.Hidden;
        }
    }
}
