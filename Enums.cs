using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remake
{
    public enum Direction
    {
        Center,
        Left, 
        Right,
        Up,
        Down,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }
    public enum TileShapeObject
    {
        Player,
        Point, 
        Tile,
        Enemy,
        ShockExplosive
    }
    public enum GradientColour
    {
        Red,
        Blue,
        Violet,
        Green,
        Orange,
        GreenBrush
    }
}
