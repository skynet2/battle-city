namespace Code.PathFinding
{
    public class Point
    {
        // point X
        public int X;

        // point Y
        public int Y;

        /// <summary>
        /// Init the point with zeros.
        /// </summary>
        public Point()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Init the point with values.
        /// </summary>
        public Point(int iX, int iY)
        {
            this.X = iX;
            this.Y = iY;
        }

        /// <summary>
        /// Init the point with a single value.
        /// </summary>
        public Point(Point b)
        {
            X = b.X;
            Y = b.Y;
        }

        /// <summary>
        /// Get point hash code.
        /// </summary>
        public override int GetHashCode()
        {
            return X ^ Y;
        }

        /// <summary>
        /// Compare points.
        /// </summary>
        public override bool Equals(object obj)
        {
            // check type
            if (!(obj.GetType() == typeof(Point)))
                return false;

            // check if other is null
            var p = (Point) obj;

            // Return true if the fields match:
            return (X == p.X) && (Y == p.Y);
        }

        /// <summary>
        /// Compare points.
        /// </summary>
        public bool Equals(Point p)
        {
            // check if other is null
            if (ReferenceEquals(null, p))
            {
                return false;
            }

            // Return true if the fields match:
            return (X == p.X) && (Y == p.Y);
        }

        /// <summary>
        /// Check if points are equal in value.
        /// </summary>
        public static bool operator ==(Point a, Point b)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (ReferenceEquals(null, a))
            {
                return false;
            }
            if (ReferenceEquals(null, b))
            {
                return false;
            }
            // Return true if the fields match:
            return a.X == b.X && a.Y == b.Y;
        }

        /// <summary>
        /// Check if points are not equal in value.
        /// </summary>
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Set point value.
        /// </summary>
        public Point Set(int iX, int iY)
        {
            this.X = iX;
            this.Y = iY;
            return this;
        }
    }
}