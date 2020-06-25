using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Rectangle mainRectangle;
        Key direction;
        DispatcherTimer moveTimer;
        DispatcherTimer pointsTimer;
        Random randomGenerator = new Random();
        List<Rectangle> pointsList = new List<Rectangle>();
        int score = 0;



        public MainWindow()
        {
            InitializeComponent();
            mainRectangle = AddRectangle(390, 390, Colors.Red);
            direction = Key.Up;
            ScoreText.Text = score.ToString();
        }

        private Rectangle AddRectangle(int x, int y, Color color)
        {
            Rectangle rect = new Rectangle();
            rect.Width = 20;
            rect.Height = 20;
            rect.Fill = new SolidColorBrush(color);

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            MainCanvas.Children.Add(rect);

            return rect;
        }

        private void MoveRectangle(Rectangle rect, Key keyPressed)
        {
            const int MoveLength = 10; 
            var actualPositionHorizontal = Canvas.GetLeft(mainRectangle);
            var actualPositionVertical = Canvas.GetTop(mainRectangle);

            if ( (actualPositionHorizontal < 0 ||
                actualPositionVertical < 0 ||
                actualPositionHorizontal > 760 ||
                actualPositionVertical > 730) )
            {
                moveTimer.Stop();
                MessageBox.Show("Przegrałeś!");
                direction = Key.Up;
                score = 0;
                ScoreText.Text = score.ToString();
                pointsList.ForEach(p => MainCanvas.Children.Remove(p));
                pointsList.Clear();
                Canvas.SetLeft(rect, 390);
                Canvas.SetTop(rect, 390);
                moveTimer.Start();
                return;
            }


            var intersections = pointsList.Where(p => Intersect(p,mainRectangle));
            if(intersections != null){
                moveTimer.Stop();
                for (int i = 0; i < intersections.Count(); i++)
                {
                    MainCanvas.Children.Remove(intersections.ElementAt(i));
                    pointsList.Remove(intersections.ElementAt(i));
                    score++;
                    ScoreText.Text = score.ToString();
                }
                moveTimer.Start();
            }

            switch(keyPressed)
            {
                case Key.A:
                case Key.Left:
                    Canvas.SetLeft(mainRectangle, actualPositionHorizontal - MoveLength);
                    break;
                case Key.S:
                case Key.Down:
                    Canvas.SetTop(mainRectangle, actualPositionVertical + MoveLength);
                    break;
                case Key.D:
                case Key.Right:
                    Canvas.SetLeft(mainRectangle, actualPositionHorizontal + MoveLength);
                    break;
                case Key.W:
                case Key.Up:
                    Canvas.SetTop(mainRectangle, actualPositionVertical - MoveLength);
                    break;

                default:
                    break;
            }
        }

        private void AddPoint()
        {
            if (pointsList.Count<3)
            {

            var x = randomGenerator.Next(0, 760);
            var y = randomGenerator.Next(0, 760);
            
                pointsList.Add(AddRectangle(x, y, Colors.LightBlue));
            }
        }

        private bool Intersect(Rectangle rect, Rectangle rect2)
        {
            var rectXPosition = Canvas.GetLeft(rect);
            var rectYPosition = Canvas.GetTop(rect);

            var rect2XPosition = Canvas.GetLeft(rect2);
            var rect2YPosition = Canvas.GetTop(rect2);

            if (rectXPosition + rect.Width >= rect2XPosition && rect2XPosition + rect2.Width >= rectXPosition &&
                rectYPosition + rect.Width >= rect2YPosition && rect2YPosition + rect2.Width >= rectYPosition)
                return true;

            return false;
        }

        private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(MainCanvas);
            moveTimer = new DispatcherTimer(new TimeSpan(500000), DispatcherPriority.Normal, (x,y) => MoveRectangle(mainRectangle, direction), Dispatcher.CurrentDispatcher);
            pointsTimer = new DispatcherTimer(new TimeSpan(0, 0, 3), DispatcherPriority.Normal, (x, y) => AddPoint(), Dispatcher.CurrentDispatcher);
            moveTimer.Start();
            pointsTimer.Start();
        }

        private void MainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            direction = e.Key;
        }
    }
}
