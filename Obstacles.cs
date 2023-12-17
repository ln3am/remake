using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using remake;

namespace Remaster
{
    public class ShapeEnemy
    {
        public int MoveTime = 1;
        private DispatcherTimer timer;
        public int X;
        public int Y;
        private Tile enemy;
        private TileShapeObject shape = TileShapeObject.Enemy;
        public void CreateEnemy()
        {
            enemy = PlayingField.GetRandomFreeCoordinate();
            enemy.SetShapeObject(shape, Colours.RedGradient());
            X = enemy.X;
            Y = enemy.Y;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(MoveTime);
            timer.Tick += (sender, e) =>
            {
                Direction direction = PlayingField.DetermineDirectionBetweenTiles(X, Y, Player.X, Player.Y);
                enemy.MoveShapeAway(direction, shape);
                (X, Y) = PlayingField.UpdateCoordinatesFromDirection(X, Y, direction);
                enemy = PlayingField.GetTile(X, Y);
                enemy.MoveShapeTo(PlayingField.OppositeDirection(direction), shape, Colours.RedGradient());
                if (enemy.IsPlayerPointTile) enemy.CollectPlayerPoint();
                if (enemy.IsPlayerTile) Player.DecreaseHP(1);
            };
            timer.Start();
        }
    }
}