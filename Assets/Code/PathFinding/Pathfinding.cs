using System.Collections.Generic;
using UnityEngine;

namespace Code.PathFinding
{
    public class Pathfinding
    {
        /// <summary>
        /// Find a path between two points.
        /// </summary>
        /// <param name="grid">Grid to search.</param>
        /// <param name="startPos">Starting position.</param>
        /// <param name="targetPos">Ending position.</param>
        /// <param name="ignorePrices">If true, will ignore tile price (how much it "cost" to walk on).</param>
        /// <returns>List of points that represent the path to walk.</returns>
        public static List<Point> FindPath(Grid grid, Point startPos, Point targetPos, bool ignorePrices = false)
        {
            // find path
            var nodesPath = _ImpFindPath(grid, startPos, targetPos, ignorePrices);

            // convert to a list of points and return
            var ret = new List<Point>();
            if (nodesPath != null)
            {
                foreach (var node in nodesPath)
                {
                    ret.Add(new Point(node.GridX, node.GridY));
                }
            }
            return ret;
        }

        /// <summary>
        /// Internal function that implements the path-finding algorithm.
        /// </summary>
        /// <param name="grid">Grid to search.</param>
        /// <param name="startPos">Starting position.</param>
        /// <param name="targetPos">Ending position.</param>
        /// <param name="ignorePrices">If true, will ignore tile price (how much it "cost" to walk on).</param>
        /// <returns>List of grid nodes that represent the path to walk.</returns>
        private static List<Node> _ImpFindPath(Grid grid, Point startPos, Point targetPos, bool ignorePrices = false)
        {
            var startNode = grid.Nodes[startPos.X, startPos.Y];
            var targetNode = grid.Nodes[targetPos.X, targetPos.Y];

            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet[0];
                for (var i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost ||
                        openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(grid, startNode, targetNode);
                }

                foreach (var neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    var newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour) *
                                                     (ignorePrices ? 1 : (int) (10.0f * neighbour.Price));
                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrace path between two points.
        /// </summary>
        /// <param name="grid">Grid to work on.</param>
        /// <param name="startNode">Starting node.</param>
        /// <param name="endNode">Ending (target) node.</param>
        /// <returns>Retraced path between nodes.</returns>
        private static List<Node> RetracePath(Grid grid, Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();
            return path;
        }

        /// <summary>
        /// Get distance between two nodes.
        /// </summary>
        /// <param name="nodeA">First node.</param>
        /// <param name="nodeB">Second node.</param>
        /// <returns>Distance between nodes.</returns>
        private static int GetDistance(Node nodeA, Node nodeB)
        {
            var dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            var dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}