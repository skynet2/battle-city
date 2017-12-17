using System.Collections.Generic;

namespace Code.PathFinding
{
    public class Grid
    {
        // nodes in grid
        public Node[,] Nodes;

        // grid size
        readonly int _gridSizeX;

        readonly int _gridSizeY;

        /// <summary>
        /// Create a new grid with tile prices.
        /// </summary>
        /// <param name="width">Grid width.</param>
        /// <param name="height">Grid height.</param>
        /// <param name="tilesCosts">A 2d array, matching width and height, of tile prices.
        ///     0.0f = Unwalkable tile.
        ///     1.0f = Normal tile.
        ///     > 1.0f = costy tile.
        ///     < 1.0f = cheap tile.
        /// </param>
        public Grid(int width, int height, float[,] tilesCosts)
        {
            _gridSizeX = width;
            _gridSizeY = height;
            Nodes = new Node[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Nodes[x, y] = new Node(tilesCosts[x, y], x, y);
                }
            }
        }

        /// <summary>
        /// Create a new grid without tile prices, eg with just walkable / unwalkable tiles.
        /// </summary>
        /// <param name="width">Grid width.</param>
        /// <param name="height">Grid height.</param>
        /// <param name="walkableTiles">A 2d array, matching width and height, which tiles are walkable and which are not.</param>
        public Grid(int width, int height, bool[,] walkableTiles)
        {
            _gridSizeX = width;
            _gridSizeY = height;
            Nodes = new Node[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Nodes[x, y] = new Node(walkableTiles[x, y] ? 1.0f : 0.0f, x, y);
                }
            }
        }

        /// <summary>
        /// Get all the neighbors of a given tile in the grid.
        /// </summary>
        /// <param name="node">Node to get neighbots for.</param>
        /// <returns>List of node neighbors.</returns>
        public List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();

            for (var x = -1; x <= 1; x++)
            {
                for (var y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var checkX = node.GridX + x;
                    var checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                    {
                        neighbours.Add(Nodes[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }
    }
}