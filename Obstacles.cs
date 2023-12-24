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

namespace Remaster
{
    public abstract class BaseShapeObstacle
    {
        protected DispatcherTimer timer = new DispatcherTimer();
        protected int timerIntervalMS = 900;
        public int X;
        public int Y;
        protected Tile obstacle;
        protected TileShapeObject Shape;
        public BaseShapeObstacle(TileShapeObject shape)
        {
            Shape = shape;
            obstacle = PlayingField.GetRandomFreeCoordinate();
            obstacle.SetShapeObject(Shape, Colours.RedGradient());
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
        public ShapeEnemy() : base(TileShapeObject.Enemy)
        {
        }
        public override void MoveEnemy(object sender, EventArgs e)
        {
            var directionDistance = PlayingField.DetermineDirectionBetweenTiles(X, Y, Player.X, Player.Y);
            Direction direction = directionDistance.Item1;
            obstacle.MoveShapeAway(direction, Shape);
            (X, Y) = PlayingField.UpdateCoordinatesFromDirection(X, Y, direction);

            bool CloseDistance = directionDistance.Item2 + directionDistance.Item3 <= 10;
            if (CloseDistance) SetTimerInterval(450); 
            else SetTimerInterval(timerIntervalMS);

            obstacle = PlayingField.GetTile(X, Y);
            obstacle.MoveShapeTo(PlayingField.OppositeDirection(direction), Shape, Colours.RedGradient());
            if (obstacle.IsPlayerTile) Player.DecreaseHP(1);
        }
    }
    public class ShapeShockExplosive
    {
        public Tile Obstacle;
        public int Radius;
        private TileShapeObject Shape;
        private int TimeTickMS;
        public int X;
        public int Y;
        public ShapeShockExplosive(int radius, int timeTickMS)
        {
            Tile center = PlayingField.GetRandomFreeCoordinate();
            TimeTickMS = timeTickMS;
            X = center.X; 
            Y = center.Y;
            Obstacle = center;
            center.SetShapeObject(TileShapeObject.ShockExplosive, Colours.VioletGradient());
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
                        if (MoveTo) tile.MoveShapeTo(PlayingField.OppositeDirection(tileDirection.Item2), TileShapeObject.ShockExplosive, Colours.VioletGradient());
                        else tile.MoveShapeAway(tileDirection.Item2, TileShapeObject.ShockExplosive);
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
                    if (IsWithinGrid(x, y) && IsOnRadius(x, y, radius))
                    {
                        outerCoordinates.Add(PlayingField.GetTile(x, y));
                    }
                }
            }
            return outerCoordinates;
        }

        private bool IsWithinGrid(int x, int y)
        {
            return x >= 0 && y >= 0 && x < PlayingField.GridSquare && y < PlayingField.GridSquare;
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