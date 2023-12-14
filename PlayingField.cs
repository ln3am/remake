using Remaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace remake
{
    public static class PlayingField
    {
        public static int GridSquare = 17;
        public static TextBlock ScoreBock;
        public static Grid TileGrid;
        public static Button UpgradeButton;
        public static int UpgradeStage = 0;
        private static int UpgradeCost = 50;
        public static void UpdateGameInfo()
        {
            ScoreBock.Text = $"Points: {Player.Points}\nLevel: {Player.Level}";
            UpgradeButton.Content = $"Upgrade ({UpgradeCost})";
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
        public static void AddPointOnMap()
        {
            Tile tile = GetRandomFreeCoordinate();
            if (tile == null) return;
            tile.SetPlayerPoint();
        }
        public static void UpgradeClick()
        {
            switch (UpgradeStage)
            {
                case 0:
                    UpgradeStageModify(1);
                    break;
                case 1:
                    UpgradeStageModify(2);
                    break;
                case 2:
                    UpgradeStageModify(3);
                    break;
                case 3:
                    UpgradeStageModify(4);
                    break;
            }
            RecolorAllTiles();
            UpdateGameInfo();
            void UpgradeStageModify(int factor)
            {
                if (!(Player.Points >= 10 * factor)) return;
                Player.Points -= 10 * factor;
                UpgradeStage = 1 * factor;
                UpgradeCost = 50 * factor;
            }
        }
        public static void RecolorAllTiles()
        {
            foreach (Tile tile in TileGrid.Children)
            {
                tile.SetTileColour(Colours.RedGradient());
            }
        }
        public static Tile GetTile(int rowX, int columnY)
        {
            int index = rowX * GridSquare + columnY;
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
            int index = Player.X * GridSquare + Player.Y;
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
