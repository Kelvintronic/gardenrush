using System;
using System.Collections.Generic;
using System.Text;

namespace gardenrush.lib.Data
{
    public class CoOrdinate
    {
        public CoOrdinate(int index, int nColumns=5)
        {
            y = index / nColumns;
            x = index % nColumns;
        }
        public CoOrdinate()
        {

        }
        public int x { get; set; }
        public int y { get; set; }

        public void Rotate90()
        {
            int _x = 0 - y;
            y = x;
            x = _x;
        }
        public static bool operator ==(CoOrdinate left, CoOrdinate right)
        {
            if (object.ReferenceEquals(left, right)) return true;

            if ((object)right == null)
                return false;

            return left.x == right.x && left.y == right.y;
        }
        public static bool operator !=(CoOrdinate left, CoOrdinate right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return this == (CoOrdinate)obj;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode()^ y.GetHashCode();
        }
    }

}
