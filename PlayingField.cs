using Remaster;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;
using System.Collections.Generic;

namespace remake
{
    public static class PlayingField
    {
        private static DispatcherTimer timer;
        public static Dispatcher UIDispatcher;
        public static double EventTick = 0.5;
        public static int GridSquare = 17; 
        public static TextBlock ScoreBock;
        public static Grid TileGrid;
        public static Random random = new Random();
        public static Button UpgradeButton;
        public static int UpgradeStage = 1;
        public static int UpgradeBaseCost = 10;
        public static int UpgradeCost = 10;
        public static List<ShapeEnemy> Enemies = new List<ShapeEnemy>();

        private static int ExplosiveRange = 6;
        private static int ExplosiveSpeedInMS = 900;
        private static int ExplosionSpawnChanceToOne = 12;
        public static void UpdateGameInfo()
        {
            UIDispatcher.Invoke(new Action(() => {
                ScoreBock.Text = $"Points: {Player.Points}\nLevel: {UpgradeStage}\nHP: {Player.HP}";
                UpgradeButton.Content = $"Upgrade ({UpgradeCost})";
            }));
        }
        public static void StartTimeTick()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(EventTick);
            timer.Tick += (sender, e) =>
            {
                AddPointOnMap(1);
                if (RandomDecide(9)) AddPointOnMap(5, Colours.BlueDarkGradient());
                UpdateGameInfo();
                if (RandomDecide(ExplosionSpawnChanceToOne)) new ShapeShockExplosive(ExplosiveRange, ExplosiveSpeedInMS);
            };
            timer.Start();
        }
        public static void AddEnemy(int moveIntervalMS)
        {
            ShapeEnemy shapeEnemy = new ShapeEnemy(){ timerIntervalMS = moveIntervalMS };
            Enemies.Add(shapeEnemy);
        }
        public static bool RandomDecide(int ChanceToOne) 
        {
           return random.Next(0, ChanceToOne) == 1;
        }
        public static Tile GetRandomFreeCoordinate()
        {
            int countloop = 0;
            Random random = new Random();
            while (true)
            {
                int x = random.Next(0, GridSquare);
                int y = random.Next(0, GridSquare);
                Tile tile = GetTile(x, y);

                countloop++;
                if (countloop == 100) return null;
                var directionDistance = DetermineDirectionBetweenTiles(Player.X, Player.Y, x, y);
                if (!tile.IsEmpty() && 3 < (directionDistance.Item2 + directionDistance.Item3)) continue;
                return tile;
            }
        }
        public static void AddPointOnMap(int Value)
        {
            Tile tile = GetRandomFreeCoordinate();
            if (tile == null) return;
            tile.SetPoint(Value);
        }
        public static void AddPointOnMap(int Value, LinearGradientBrush colour)
        {
            Tile tile = GetRandomFreeCoordinate();
            if (tile == null) return;
            tile.SetPoint(Value);
            tile.SetShapeObjectColour(colour, TileShapeObject.Point);
        }
        public static void UpgradeClick()
        {
            if (Player.Points < UpgradeCost) return;
            Player.Points -= UpgradeCost;
            UpgradeStage++;
            UpgradeCost = UpgradeBaseCost * UpgradeStage;

            if (UpgradeStage == 4)
            {
                ExplosiveSpeedInMS = 700;
                AddEnemy(800);
            }
           
            UpdateGameInfo();   
        }
        public static void RecolorAllTiles(LinearGradientBrush color)
        {
            if (color == null) return;
            foreach (Tile tile in TileGrid.Children)
            {
                tile.SetShapeObjectColour(color, TileShapeObject.Tile);
            }
        }
        public static Direction OppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.UpLeft:
                    return Direction.DownRight;
                case Direction.UpRight:
                    return Direction.DownLeft;
                case Direction.DownLeft:
                    return Direction.UpRight;
                case Direction.DownRight:
                    return Direction.UpLeft;
                default:
                    return Direction.Down;
            }
        }
        public static (int, int) UpdateCoordinatesFromDirection(int x, int y, Direction direction)
        {
                switch (direction)
                {
                    case Direction.Left:
                        x -= 1;
                        break;
                    case Direction.Right:
                        x += 1;
                        break;
                    case Direction.Up:
                        y -= 1;
                        break;
                    case Direction.Down:
                        y += 1;
                        break;
                    case Direction.UpLeft:
                        x -= 1;
                        y -= 1;
                        break;
                    case Direction.UpRight:
                        x += 1;
                        y -= 1;
                        break;
                    case Direction.DownLeft:
                        x -= 1;
                        y += 1;
                        break;
                    case Direction.DownRight:
                        x += 1;
                        y += 1;
                        break;
                }
                return (x, y);
        }
        public static (int, int, bool, Direction) UpdateCoordinatesFromDirectionWithObstacles(int X, int Y, Direction direction, Direction previousDirection, List<TileShapeObject> tso)
        {
            int XP; 
            int YP;
            bool noMove = false;
            PreferedDirection preferedDirection = new PreferedDirection();
            while (true)
            {
                (XP, YP) = UpdateCoordinatesFromDirection(X, Y, direction);
                if (IsWithinGrid(XP, YP) && direction != OppositeDirection(previousDirection) && !LocalIsOneOfTSO(GetTile(XP, YP), tso)) break;
                direction = preferedDirection.GetDirection(direction);
            }
            return (XP, YP, noMove, direction);

            bool LocalIsOneOfTSO(Tile tile, List<TileShapeObject> tsoList)
            {
                bool returnTrue = false;
                foreach (var tso in tsoList)
                {
                    if (tile.IsTSO(tso)) returnTrue = true;
                }
                return returnTrue;
            }
        }
        public static (Direction, int, int) DetermineDirectionBetweenTiles(int tile1X, int tile1Y, int tile2X, int tile2Y)
        {
            int distanceX = Math.Abs(tile1X - tile2X);
            int distanceY = Math.Abs(tile1Y - tile2Y);
            bool moveOnX = distanceX >= distanceY;
            bool moveOnY = distanceY >= distanceX;

            return (LocalDecideDirection(moveOnX, moveOnY), distanceX, distanceY);

            Direction LocalDecideDirection(bool onX, bool onY)
            {
                if (moveOnX && moveOnY)
                {
                    if (tile1X < tile2X && tile1Y < tile2Y) return Direction.DownRight;
                    if (tile1X > tile2X && tile1Y < tile2Y) return Direction.DownLeft;
                    if (tile1X < tile2X && tile1Y > tile2Y) return Direction.UpRight;
                    if (tile1X > tile2X && tile1Y > tile2Y) return Direction.UpLeft;
                }
                if (moveOnX) return tile1X < tile2X ? Direction.Right : Direction.Left;
                if (moveOnY) return tile1Y < tile2Y ? Direction.Down : Direction.Up;
                return Direction.Up;
            }
        }
        public static bool IsWithinGrid(int x, int y)
        {
            return x >= 0 && y >= 0 && x < GridSquare && y < GridSquare;
        }
        public static Tile GetTile(int X, int Y)
        {
            int index = Y * GridSquare + X;
            if (index < TileGrid.Children.Count) return TileGrid.Children[index] as Tile;
            return null;
        }
        public static Tile GetPlayerTile()
        {
            int index = Player.Y * GridSquare + Player.X;
            if (index < TileGrid.Children.Count)return TileGrid.Children[index] as Tile;
            return null;
        }
    }
}
