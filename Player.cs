using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace remake
{
    public static class Player
    {
        public static int X = 9;
        public static int Y = 9;
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
                    deltaY = -1;
                    break;
                case Direction.Right:
                    deltaY = 1;
                    break;
                case Direction.Up:
                    deltaX = -1;
                    break;
                case Direction.Down:
                    deltaX = 1;
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
            tile.MovePlayerTo(LocalOppositeDirection(direction));
            if (tile.IsPlayerPointTile)CollectPoint(tile);
            PlayingField.UpdateGameInfo();

            Direction LocalOppositeDirection(Direction direction)
            {
                switch(direction)
                {
                    case Direction.Left:
                        return Direction.Right;
                        break;
                    case Direction.Right:
                        return Direction.Left;
                        break;
                    case Direction.Up:
                        return Direction.Down;
                        break;
                    case Direction.Down:
                        return Direction.Up;
                        break;
                }
                return Direction.Down;
            }
        }
        public static bool IsPlayerOnTile(Tile tile)
        {
            return (tile.X == X && tile.Y == Y) ? true : false;
        }

        public static void CollectPoint(Tile tile)
        {
            Points++;
            tile.CollectPlayerPoint();
        }
    }
}
