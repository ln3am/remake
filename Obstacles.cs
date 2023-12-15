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
        public void CreateEnemy()
        {
            PlayingField.GetRandomFreeCoordinate();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(MoveTime);
            timer.Tick += (sender, e) =>
            {

            };
            timer.Start();
        }
    }
}
