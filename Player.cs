using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace remake
{
    public static class Player
    {
        public static int X = 9;
        public static int Y = 9;
        public static int HP = 3;
        public static int Points = 0;
        public static int Level = 0;
        public static void MovePlayer(Grid grid, Direction direction)
        {
            Tile tile;
            int deltaX = 0;
            int deltaY = 0;
            switch (direction)
            {
                case Direction.Left:
                    deltaX = -1;
                    break;
                case Direction.Right:
                    deltaX = 1;
                    break;
                case Direction.Up:
                    deltaY = -1;
                    break;
                case Direction.Down:
                    deltaY = 1;
                    break;
            }
            int newX = X + deltaX;
            int newY = Y + deltaY;
            if (newX < 0 || newX >= PlayingField.GridSquare || newY < 0 || newY >= PlayingField.GridSquare) return;
            
            tile = PlayingField.GetPlayerTile();
            tile.MovePlayerAway(direction);
            X = newX;
            Y = newY;

            tile = PlayingField.GetPlayerTile();
            tile.MovePlayerTo(PlayingField.OppositeDirection(direction));
            if (tile.IsPlayerPointTile)CollectPoint(tile);
            PlayingField.UpdateGameInfo();

            
        }
        public static void DecreaseHP(int amount)
        {
            HP -= amount;
            if (HP <= 0) MessageBox.Show("You died");
            PlayingField.UpdateGameInfo();
        }
        public static void CollectPoint(Tile tile)
        {
            Points += tile.CollectPlayerPoint();
        }
    }
}
