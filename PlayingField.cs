using Remaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace remake
{
    public static class PlayingField
    {
        private static DispatcherTimer timer;

        public static double EventTick = 1;
        public static int GridSquare = 17; 
        public static TextBlock ScoreBock;
        public static Grid TileGrid;
        public static Button UpgradeButton;
        public static int UpgradeStage = 0;
        public static int UpgradeCost = 50;
        public static void UpdateGameInfo()
        {
            ScoreBock.Text = $"Points: {Player.Points}\nLevel: {UpgradeStage}";
            UpgradeButton.Content = $"Upgrade ({UpgradeCost})";
        }
        public static void StartTimeTick()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(EventTick);
            timer.Tick += (sender, e) =>
            {
                AddPointOnMap(1);
                UpdateGameInfo();
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
                if (!tile.IsEmpty()) continue;
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
                if (!(Player.Points >= 10 * factor)) return;
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
                        break;
                    case 3: return Colours.RedGradient();
                        break;
                       
                }
                return null;
            }
        }
        public static Tile GetTile(int rowY, int columnX)
        {
            int index = rowY * GridSquare + columnX;
            if (index < TileGrid.Children.Count)
            {
                return TileGrid.Children[index] as Tile;
            }
            else
            {
                return null;
            }
        }
        public static Tile GetPlayerTile()
        {
            int index = Player.Y * GridSquare + Player.X;
            if (index < TileGrid.Children.Count)
            {
                return TileGrid.Children[index] as Tile;
            }
            else
            {
                return null;
            }
        }

    }
}
