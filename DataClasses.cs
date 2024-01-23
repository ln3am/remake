using remake;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Remaster
{
    public class PreferedDirection 
    {
        int accessCount = 0;
        Dictionary<Direction, int> DirectionsIndex = new Dictionary<Direction, int>
        {
            { Direction.Up, 0 },
            { Direction.UpRight, 1 },
            { Direction.Right, 2 },
            { Direction.DownRight, 3 },
            { Direction.Down, 4 },
            { Direction.DownLeft, 5 },
            { Direction.Left, 6 },
            { Direction.UpLeft, 7 }
        };
        public Direction GetDirection(Direction currentDirection)
        {
            if (accessCount == 8) return Direction.Center;

            int i = DirectionsIndex[currentDirection];
            int nexti = (i + 1) % 8;
            accessCount++;
            return DirectionsIndex.FirstOrDefault(x => x.Value == nexti).Key;
        }  
    }
}
