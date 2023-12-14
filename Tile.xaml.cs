using Remaster;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace remake
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        public int X;
        public int Y;
        public bool IsPlayerPointTile = false;
        public bool IsPlayerTile = false;
        public double AnimationSpeed = 0.2;
        public Tile()
        {
            InitializeComponent();
        }
        private void OnLoad(object sender, EventArgs e)
        {
            SetTileColour(Colours.BlueGradient());
            SetPlayerColour(Colours.VioletGradient());
        }
        public bool IsEmpty()
        {
            return (!IsPlayerPointTile && !IsPlayerTile) ? true : false;
        }
        public void SetTileColour(LinearGradientBrush colour)
        {
            TileRec.Fill = colour;
        }
        public void SetPlayerColour(LinearGradientBrush colour)
        {
            TilePlayer.Fill = colour;
        }
        public void SetPlayerPoint()
        {
            IsPlayerPointTile = true;
            TileCirc.Visibility = Visibility.Visible;
        }
        public void CollectPlayerPoint()
        {
            IsPlayerPointTile = false;
            TileCirc.Visibility = Visibility.Collapsed;
        }
        public void MovePlayerTo(Direction direction)
        {
            IsPlayerTile = true;
            ReappearAndMoveToCenter(direction);
        }
        public void MovePlayerAway(Direction direction)
        {
            IsPlayerTile = false;
            MoveAndDisappear(direction);
        }

        private void MoveAndDisappear(Direction direction)
        {
            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                To = direction == Direction.Left || direction == Direction.Up ? -20 : 20,
                FillBehavior = FillBehavior.Stop
            };

            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                To = 0 
            };

            if (direction == Direction.Left || direction == Direction.Right)
            {
                playerTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            }
            else
            {
                playerTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation);
            }
            
            TilePlayer.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            var timer = new DispatcherTimer { Interval = moveAnimation.Duration.TimeSpan };
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                TilePlayer.Visibility = Visibility.Collapsed;
                playerTransform.X = playerTransform.Y = 0;
                TilePlayer.Opacity = 1;
            };
            timer.Start();
        }
        private void ReappearAndMoveToCenter(Direction direction)
        {
            playerTransform.X = direction == Direction.Left ? -20 : direction == Direction.Right ? 20 : 0;
            playerTransform.Y = direction == Direction.Up ? -20 : direction == Direction.Down ? 20 : 0;

            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                To = 0,
            };

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                From = 0, 
                To = 1  
            };

            TilePlayer.Visibility = Visibility.Visible;
            TilePlayer.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            if (direction == Direction.Left || direction == Direction.Right)
            {
                playerTransform.BeginAnimation(TranslateTransform.XProperty, moveAnimation);
            }
            else 
            {
                playerTransform.BeginAnimation(TranslateTransform.YProperty, moveAnimation); 
            }
            
        }
    }
}
