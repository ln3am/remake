using Remaster;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;

namespace remake
{
    public static class PlayingField
    {
        private static DispatcherTimer timer;
        public static Dispatcher UIDispatcher;
        public static double EventTick = 1;
        public static int GridSquare = 17; 
        public static TextBlock ScoreBock;
        public static Grid TileGrid;
        public static Random random = new Random();
        public static Button UpgradeButton;
        public static int UpgradeStage = 0;
        public static int UpgradeCost = 50;
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
                UpdateGameInfo();
                if (random.Next(0, 6) == 1) new ShapeShockExplosive(5, 2000);
            };
            timer.Start();
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
        public static void UpgradeClick()
        {
            switch (UpgradeStage)
            {
                case 0:
                    LocalUpgradeStageModify(1);
                    break;
                case 1:
                    LocalUpgradeStageModify(2);
                    break;
                case 2:
                    LocalUpgradeStageModify(3);
                    break;
                case 3:
                    LocalUpgradeStageModify(4);
                    break;
            }
            RecolorAllTiles(UpgradeStage);
            UpdateGameInfo();

            void LocalUpgradeStageModify(int factor)
            {
                if (!(Player.Points >= UpgradeCost)) return;
                Player.Points -= 10 * factor;
                UpgradeStage = 1 * factor;
                UpgradeCost = 50 * factor;
            }
        }
        public static void RecolorAllTiles(int variation)
        {
            LinearGradientBrush color = LocalColourVariation(variation);
            if (color == null) return;
            foreach (Tile tile in TileGrid.Children)
            {
                tile.SetShapeObjectColour(color, TileShapeObject.Tile);
            }

            LinearGradientBrush LocalColourVariation(int variation)
            {
                switch (variation)
                {
                    case 0:
                    case 1:
                        return Colours.BlueGradient();
                    case 3: 
                        return Colours.RedGradient();
                }
                return null;
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
        public static (int, int) UpdateCoordinatesFromDirection(int X, int Y, Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    X -= 1;
                    break;
                case Direction.Right:
                    X += 1;
                    break;
                case Direction.Up:
                    Y -= 1;
                    break;
                case Direction.Down:
                    Y += 1;
                    break;
                case Direction.UpLeft:
                    X -= 1;
                    Y -= 1;
                    break;
                case Direction.UpRight:
                    X += 1;
                    Y -= 1;
                    break;
                case Direction.DownLeft:
                    X -= 1;
                    Y += 1;
                    break;
                case Direction.DownRight:
                    X += 1;
                    Y += 1;
                    break;
            }
            return (X, Y);
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
