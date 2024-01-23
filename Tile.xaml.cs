using Remaster;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public bool IsEnemyTile = false;
        public Dispatcher UIDispatcher;
        public bool IsExplosiveTile = false;
        public int PointValue = 0;
        public double AnimationSpeed = 0.2;

        private List<Shape> OwnedShapes = new List<Shape>();

        private DispatcherTimer Timer;
        public Tile()
        {
            InitializeComponent();
        }
        private void OnLoad(object sender, EventArgs e)
        {
            SetShapeObjectColour(Colours.BlueGradient(), TileShapeObject.Tile);
            SetShapeObjectColour(Colours.VioletGradient(), TileShapeObject.Player);
        }
        public bool IsTSO(TileShapeObject tso)
        {
            switch (tso)
            {
                case TileShapeObject.Player:
                    return IsPlayerTile;
                case TileShapeObject.Point:
                    return IsPlayerPointTile;
                case TileShapeObject.Tile:
                    return true;
                case TileShapeObject.Enemy:
                    return IsEnemyTile;
                case TileShapeObject.ShockExplosive:
                    return IsExplosiveTile;
                default:
                    return false;
            }
        }
        public bool IsEmpty()
        {
            return (IsPlayerPointTile && IsPlayerTile && IsEnemyTile && IsExplosiveTile) ? false : true;
        }
        public void SetPoint(int value)
        {
            IsPlayerPointTile = true;
            TileCirc.Fill = Colours.GreenGradient();
            TileCirc.Visibility = Visibility.Visible;
            PointValue = value;
        }
        public void MovePlayerTo(Direction direction)
        {
            IsPlayerTile = true;
            ReappearAndMoveToCenter(direction, TilePlayer);
        }
        public void MovePlayerAway(Direction direction)
        {
            IsPlayerTile = false;
            MoveAndDisappear(direction, TilePlayer);
        }
        public void SetShapeObjectColour(LinearGradientBrush colour, TileShapeObject shape)
        {
            DecideShapeObject(shape).Fill = colour; 
        }
        public int CollectPlayerPoint()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                IsPlayerPointTile = false;
                TileCirc.Visibility = Visibility.Collapsed;
            }));
            return PointValue;
        }
        public void SetShapeObject(TileShapeObject shape, LinearGradientBrush colour)
        {
           UIDispatcher.Invoke(new Action(() =>
           {
               Shape shapeObj = DecideShapeObject(shape);
               shapeObj.Fill = colour;
               shapeObj.Visibility = Visibility.Visible;
               UpdateTileStateInfo(shape, true);
           }));
        }
        public void MoveShapeTo(Direction direction, TileShapeObject shape, GradientColour colour)
        {
            UIDispatcher.Invoke(new Action(() =>
            {
                Shape shapeObj = DecideShapeObject(shape);
                ReappearAndMoveToCenter(direction, shapeObj);
                SetShapeObjectColour(GetColour(colour), shape);
                UpdateTileStateInfo(shape, true);
            }));
        }
        public void MoveShapeAway(Direction direction, TileShapeObject shape)
        {
            UIDispatcher.Invoke(new Action(() =>
            {
                Shape shapeObj = DecideShapeObject(shape);
                MoveAndDisappear(direction, shapeObj);
                UpdateTileStateInfo(shape, false);
            })); 
        }
        public void MoveNewShapeTo(Direction direction, TileShapeObject shape, GradientColour colour)
        {
            UIDispatcher.Invoke(new Action(() =>
            {
                Shape shapeObj = CreateEllipse(colour);
                OwnedShapes.Add(shapeObj);
                ReappearAndMoveToCenter(direction, shapeObj);
                SetShapeObjectColour(GetColour(colour), shape);
                UpdateTileStateInfo(shape, true);
            }));
        }
        public void MoveNewShapeAway(Direction direction, TileShapeObject shape)
        {
            UIDispatcher.Invoke(new Action(() =>
            {
                Shape shapeObj = OwnedShapes[0];
                OwnedShapes.RemoveAt(0);
                MoveAndDisappear(direction, shapeObj);
                UpdateTileStateInfo(shape, false);
            }));
        }
        private Shape DecideShapeObject(TileShapeObject shape)
        {
            switch (shape)
            {
                case TileShapeObject.Player:
                    return TilePlayer;
                case TileShapeObject.Enemy:
                    return TileCircEnemy;
                case TileShapeObject.Tile: 
                    return TileRec;
                case TileShapeObject.Point:
                    return TileCirc;
                case TileShapeObject.ShockExplosive:
                    return TileTria;
            }
            return null;
        }
        private void UpdateTileStateInfo(TileShapeObject shape, bool state)
        {
            switch (shape)
            {
                case TileShapeObject.Player:
                    IsPlayerTile = state;
                    break;
                case TileShapeObject.Enemy:
                    IsEnemyTile = state;
                    break;
                case TileShapeObject.Point:
                    IsPlayerPointTile = state;
                    break;
                case TileShapeObject.ShockExplosive:
                    IsExplosiveTile = state;
                    break;
            }
        }
        private LinearGradientBrush GetColour(GradientColour colour)
        {
            switch (colour)
            {
                case GradientColour.Blue:
                    return Colours.BlueGradient();
                case GradientColour.Red:
                    return Colours.RedGradient();
                case GradientColour.Violet:
                    return Colours.VioletGradient();
                case GradientColour.Green:
                    return Colours.GreenGradient();
                case GradientColour.Orange:
                    return Colours.OrangeGradient();
                case GradientColour.GreenBrush:
                    return Colours.GreenGradientBrush();
            }
            return null;
        }
        private void MoveAndDisappear(Direction direction, Shape shape)
        {
            shape.RenderTransform = new TranslateTransform();
            TranslateTransform moveShape = shape.RenderTransform as TranslateTransform;
            DoubleAnimation moveAnimationX = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                To = (direction == Direction.Left || direction == Direction.UpLeft || direction == Direction.DownLeft) ? -20 : 20,
                FillBehavior = FillBehavior.Stop
            };

            DoubleAnimation moveAnimationY = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                To = (direction == Direction.Up || direction == Direction.UpLeft || direction == Direction.UpRight) ? -20 : 20,
                FillBehavior = FillBehavior.Stop
            };

            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                To = 0
            };

            if (direction == Direction.Left || direction == Direction.Right)
            {
                moveShape.BeginAnimation(TranslateTransform.XProperty, moveAnimationX);
            }
            else if (direction == Direction.Up || direction == Direction.Down)
            {
                moveShape.BeginAnimation(TranslateTransform.YProperty, moveAnimationY);
            }
            else 
            {
                moveShape.BeginAnimation(TranslateTransform.XProperty, moveAnimationX);
                moveShape.BeginAnimation(TranslateTransform.YProperty, moveAnimationY);
            }

            shape.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

            Timer = new DispatcherTimer { Interval = moveAnimationX.Duration.TimeSpan };
            Timer.Tick += (sender, e) =>
            {
                Timer.Stop();
                shape.Visibility = Visibility.Collapsed;
                moveShape.X = moveShape.Y = 0;
                shape.Opacity = 1;
                };
            Timer.Start();
        }

        private void ReappearAndMoveToCenter(Direction direction, Shape shape)
        {
            shape.RenderTransform = new TranslateTransform();
            TranslateTransform moveShape = shape.RenderTransform as TranslateTransform;

            moveShape.X = (direction == Direction.Left || direction == Direction.UpLeft || direction == Direction.DownLeft) ? -20 :
                          (direction == Direction.Right || direction == Direction.UpRight || direction == Direction.DownRight) ? 20 : 0;
            moveShape.Y = (direction == Direction.Up || direction == Direction.UpLeft || direction == Direction.UpRight) ? -20 :
                          (direction == Direction.Down || direction == Direction.DownLeft || direction == Direction.DownRight) ? 20 : 0;

            DoubleAnimation moveAnimationX = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(AnimationSpeed),
                To = 0,
            };

            DoubleAnimation moveAnimationY = new DoubleAnimation
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

            shape.Visibility = Visibility.Visible;
            shape.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);

            if (direction == Direction.Left || direction == Direction.Right)
            {
                moveShape.BeginAnimation(TranslateTransform.XProperty, moveAnimationX);
            }
            else if (direction == Direction.Up || direction == Direction.Down)
            {
                moveShape.BeginAnimation(TranslateTransform.YProperty, moveAnimationY);
            }
            else 
            {
                moveShape.BeginAnimation(TranslateTransform.XProperty, moveAnimationX);
                moveShape.BeginAnimation(TranslateTransform.YProperty, moveAnimationY);
            }
        }
        private Ellipse CreateEllipse(GradientColour gradient)
        {
            LinearGradientBrush brush = GetColour(gradient);

            Ellipse ellipse = new Ellipse
            {
                Visibility = Visibility.Visible,
                Fill = brush,
                Stroke = Brushes.Black,
                Width = 25,
                Height = 25
            };
            Canvas.SetLeft(ellipse, 12.5);
            Canvas.SetTop(ellipse, 12.5);
            alignment.Children.Add(ellipse);
            return ellipse;
        }
    }
}
