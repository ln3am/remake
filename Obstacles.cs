using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using remake;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Remaster
{
    public abstract class BaseShapeObstacle
    {
        public int timerIntervalMS = 900;
        public int X;
        public int Y;

        protected DispatcherTimer timer = new DispatcherTimer();
        protected LinearGradientBrush ShapeColour;
        protected Direction previousDirection = Direction.Center;
        protected Tile obstacle;
        protected TileShapeObject Shape;
        public BaseShapeObstacle(TileShapeObject shape)
        {
            ShapeColour = Colours.RedGradient();
            Shape = shape;
            obstacle = PlayingField.GetRandomFreeCoordinate();
            obstacle.SetShapeObject(Shape, ShapeColour);
            X = obstacle.X;
            Y = obstacle.Y;
            SetTimerInterval(timerIntervalMS);
            timer.Tick += new EventHandler(MoveEnemy);
            timer.Start();
        }
        public abstract void MoveEnemy(object sender, EventArgs e);
        protected void SetTimerInterval(double timeInMS)
        {
            timer.Interval = TimeSpan.FromMilliseconds(timeInMS);
        }
    }
    public class ShapeEnemy : BaseShapeObstacle
    {
        public ShapeEnemy() : base(TileShapeObject.Enemy) { }
        public override void MoveEnemy(object sender, EventArgs e)
        {
            var directionDistance = PlayingField.DetermineDirectionBetweenTiles(X, Y, Player.X, Player.Y);
            Direction direction = directionDistance.Item1;

            (int setX, int setY, bool noMove, direction) = PlayingField.UpdateCoordinatesFromDirectionWithObstacles(X, Y, direction, previousDirection, new List<TileShapeObject>{ TileShapeObject.Point, TileShapeObject.Enemy });
            if (noMove) return;
            previousDirection = direction;
            obstacle.MoveShapeAway(direction, Shape);
            
            X = setX; 
            Y = setY;
            bool CloseDistance = directionDistance.Item2 + directionDistance.Item3 <= 10;
            if (CloseDistance) SetTimerInterval(timerIntervalMS/2); 
            else SetTimerInterval(timerIntervalMS);

            obstacle = PlayingField.GetTile(X, Y);
            obstacle.MoveShapeTo(PlayingField.OppositeDirection(direction), Shape, GradientColour.Red);
            if (obstacle.IsPlayerTile) Player.DecreaseHP(1);
        }
    }
    public class ShapeShockExplosive
    {
        public Tile Obstacle;
        public int Radius;
        private LinearGradientBrush ShapeColour;
        private GradientColour ShapeColourType;
        private TileShapeObject Shape;
        private int TimeTickMS;
        public int X;
        public int Y;
        public ShapeShockExplosive(int radius, int timeTickMS)
        {
            Tile center = PlayingField.GetRandomFreeCoordinate();
            TimeTickMS = timeTickMS;
            ShapeColour = Colours.OrangeGradient();
            ShapeColourType = GradientColour.Orange;
            X = center.X; 
            Y = center.Y;
            Obstacle = center;
            center.SetShapeObject(TileShapeObject.ShockExplosive, ShapeColour);
            if (center.IsPlayerPointTile) center.CollectPlayerPoint();
            ExplosionLayers(radius);
        }

        public void ExplosionLayers(int radius)
        {
            Shape = TileShapeObject.ShockExplosive;
            Dictionary<int, List<(Tile, Direction)>> Coords = GetExplosionLayersWithDirections(radius);
            Task.Run(() => {
                int lastKey = 0;
                foreach (int key in Coords.Keys)
                {
                    Thread.Sleep(TimeTickMS);
                    if (key > 1) LocalMoveShape(Coords, key - 1, false);
                    LocalMoveShape(Coords, key, true);
                    lastKey = key;
                }
                LocalMoveShape(Coords, lastKey, false);
                Obstacle.MoveShapeAway(Direction.Up, TileShapeObject.ShockExplosive);
            });

            void LocalMoveShape(Dictionary<int, List<(Tile, Direction)>> Coords, int key, bool MoveTo)
            {
                foreach (var tileDirection in Coords[key])
                {
                    Tile tile = tileDirection.Item1;
                    if (tile.IsPlayerPointTile) tile.CollectPlayerPoint();
                    if (tile.IsPlayerTile) Player.DecreaseHP(1);
                    if (MoveTo) tile.MoveNewShapeTo(PlayingField.OppositeDirection(tileDirection.Item2), TileShapeObject.ShockExplosive, ShapeColourType);
                    else tile.MoveNewShapeAway(tileDirection.Item2, TileShapeObject.ShockExplosive);
                }
            }
        }

        private List<Tile> GetExplosionOuterCoordinates(int radius)
        {
            List<Tile> outerCoordinates = new List<Tile>();

            for (int y = Y - radius; y <= Y + radius; y++)
            {
                for (int x = X - radius; x <= X + radius; x++)
                {
                    if (PlayingField.IsWithinGrid(x, y) && IsOnRadius(x, y, radius))
                    {
                        outerCoordinates.Add(PlayingField.GetTile(x, y));
                    }
                }
            }
            return outerCoordinates;
        }

        private bool IsOnRadius(int x, int y, int radius)
        {
            double distance = Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2));
            return distance >= radius - 0.5 && distance <= radius + 0.5;
        }

        private Dictionary<int, List<(Tile, Direction)>> GetExplosionLayersWithDirections(int maxRadius)
        {
            var layersWithDirections = new Dictionary<int, List<(Tile, Direction)>>();

            for (int radius = 1; radius <= maxRadius; radius++)
            {
                List<Tile> currentLayerCoordinates = GetExplosionOuterCoordinates(radius);
                List<(Tile, Direction)> currentLayerWithDirections = new List<(Tile, Direction)>();

                foreach (var coord in currentLayerCoordinates)
                {
                    Direction direction = DetermineDirectionFromCenter(coord.X, coord.Y, radius);
                    currentLayerWithDirections.Add((coord, direction));
                }

                layersWithDirections.Add(radius, currentLayerWithDirections);
            }

            return layersWithDirections;
        }

        private Direction DetermineDirectionFromCenter(int x, int y, int radius)
        {
            int range = (radius - 1) / 2;

            if (x >= X - range && x <= X + range && y < Y) return Direction.Up;
            if (x >= X - range && x <= X + range && y > Y) return Direction.Down;
            if (y >= Y - range && y <= Y + range && x < X) return Direction.Left;
            if (y >= Y - range && y <= Y + range && x > X) return Direction.Right;
            if (x > X && y < Y) return Direction.UpRight;
            if (x < X && y < Y) return Direction.UpLeft;
            if (x > X && y > Y) return Direction.DownRight;
            if (x < X && y > Y) return Direction.DownLeft;

            return Direction.Up; 
        }
    }
}