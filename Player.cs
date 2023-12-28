using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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

        private static int InvincibillityInMS = 2000;
        private static DispatcherTimer Timer;
        private static bool PlayerIsInvincible;
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
            if (tile.IsPlayerPointTile) CollectPoint(tile);
            if (tile.IsExplosiveTile || tile.IsEnemyTile) DecreaseHP(1);
            PlayingField.UpdateGameInfo();
        }

        public static void DecreaseHP(int amount)
        {
            if (PlayerIsInvincible) return;
            PlayerIsInvincible = true;
            Timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(InvincibillityInMS) };
            HP -= amount;
            if (HP <= 0) MessageBox.Show("You died");
            PlayingField.UpdateGameInfo();

            Timer.Tick += (object sender, EventArgs e) => 
            { 
                PlayerIsInvincible = false; 
                Timer.Stop(); 
            };
            Timer.Start();
        }
        public static void CollectPoint(Tile tile)
        {
            Points += tile.CollectPlayerPoint();
        }
    }
}
