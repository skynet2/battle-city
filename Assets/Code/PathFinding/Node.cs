namespace Code.PathFinding
{
    public class Node
    {
        // is this node walkable?
        public bool Walkable;
        public int GridX;
        public int GridY;
        public float Price;

        // calculated values while finding path
        public int GCost;
        public int HCost;
        public Node Parent;

        /// <summary>
        /// Create the grid node.
        /// </summary>
        /// <param name="price">Price to walk on this node (set to 1.0f to ignore).</param>
        /// <param name="gridX">Node x index.</param>
        /// <param name="gridY">Node y index.</param>
        public Node(float price, int gridX, int gridY)
        {
            Walkable = price != 0.0f;
            Price = price;
            GridX = gridX;
            GridY = gridY;
        }

        /// <summary>
        /// Create the grid node.
        /// </summary>
        /// <param name="walkable">Is this tile walkable?</param>
        /// <param name="gridX">Node x index.</param>
        /// <param name="gridY">Node y index.</param>
        public Node(bool walkable, int gridX, int gridY)
        {
            Walkable = walkable;
            Price = walkable ? 1f : 0f;
            GridX = gridX;
            GridY = gridY;
        }

        /// <summary>
        /// Get fCost of this node.
        /// </summary>
        public int FCost
        {
            get
            {
                return GCost + HCost;
            }
        }
    }
}