using GraftGuard.Grafting.Towers;
using GraftGuard.Graphics;
using GraftGuard.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraftGuard.Map.Pathing;
internal class PathManager
{
    public static Random random = new();
    public readonly Point[] Directions = [
        new Point(1, 0), // Right
        new Point(1, 1), // Down-Right
        new Point(0, 1), // Down
        new Point(-1, 1), // Down-Left
        new Point(-1, 0), // Left
        new Point(-1, -1), // Up-Left
        new Point(0, -1), // Up
        new Point(1, -1), // Up-Right
        ];
    public readonly bool[] DirectionDiagonals = [
        false, true, false, true, false, true, false, true
        ];
    public static readonly float SquareRootOfTwo = MathF.Sqrt(2);

    public Vector2 Start { get; set; } = new Vector2(0, 0);
    public Vector2 End { get; set; } = new Vector2(1000, 1000);
    public PathNode[,] Nodes { get; set; }
    
    public bool CheckGetNode(Point gridPosition, out PathNode node)
    {
        if (gridPosition.X < 0 || gridPosition.X >= Nodes.GetLength(0) || gridPosition.Y < 0 || gridPosition.Y >= Nodes.GetLength(1))
        {
            node = null;
            return false;
        }

        PathNode possibleNode = Nodes[gridPosition.X, gridPosition.Y];

        if (possibleNode is null)
        {
            node = null;
            return false;
        }

        node = possibleNode;
        return true;
    }

    public PathNode GetNode(Point gridPosition)
    {
        return Nodes[gridPosition.X, gridPosition.Y];
    }

    public void BuildGrid(World world)
    {
        Nodes = new PathNode?[
            (int)((End.X - Start.X) / PathNode.GridDistance) + 1,
            (int)((End.Y - Start.Y) / PathNode.GridDistance) + 1
            ];

        Point gridPosition = new Point(-1, -1);
        for (float x = Start.X; x < End.X; x += PathNode.GridDistance)
        {
            gridPosition.Y = -1;
            gridPosition.X += 1;
            for (float y = Start.Y; y < End.Y; y += PathNode.GridDistance)
            {
                gridPosition.Y += 1;
                PathNode node = new PathNode(new Vector2(x, y), gridPosition);

                if (world.Terrain.Overlaps(node.CheckCircle))
                {
                    Nodes[gridPosition.X, gridPosition.Y] = null;
                    continue;
                }

                // Add Tower Costs to Node
                foreach (Tower tower in world.TowerManager.Towers)
                {
                    bool overlapsTower = tower.PathAreas.Any((area) => area.Intersects(node.CheckCircle));

                    if (overlapsTower)
                    {
                        node.Cost += tower.PathCost;
                    }
                }

                Nodes[gridPosition.X, gridPosition.Y] = node;
            }
        }
    }

    public void UpdateCosts(World world)
    {
        foreach (PathNode node in Nodes)
        {
            if (node is null)
            {
                continue;
            }
            // Add Tower Costs to Node
            foreach (Tower tower in world.TowerManager.Towers)
            {
                bool overlapsTower = tower.PathAreas.Any((area) => area.Intersects(node.CheckCircle));

                if (overlapsTower)
                {
                    node.Cost += tower.PathCost;
                }
            }
        }
    }

    public void Draw(DrawManager drawing, GameTime time)
    {
        foreach (PathNode? possibleNode in Nodes)
        {
            if (possibleNode is PathNode node)
            {
                node.Draw(drawing);
            }
        }
    }

    public PathNode FindNearestNode(Vector2 position)
    {
        PathNode closest = null;
        float minDistance = float.PositiveInfinity;
        foreach (PathNode node in Nodes)
        {
            if (node is null)
            {
                continue;
            }

            float distance = Vector2.DistanceSquared(position, node.WorldPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = node;
            }
        }

        if (closest is null)
        {
            throw new NullReferenceException("FindNearestNode was called without any nodes generated!");
        }

        return closest;
    }

    public List<PathNode> FindPath(World world, Vector2 start, PathSettings settings, Func<Vector2, float> getExtraPathingCost = null)
    {
        switch (settings.Goal)
        {
            case PathGoal.Garage:
                return FindPointPath(FindNearestNode(start).GridPosition, FindNearestNode(world.Garage.Center).GridPosition, true, getExtraPathingCost);
            case PathGoal.Player:
                return FindPointPath(FindNearestNode(start).GridPosition, FindNearestNode(world.Player.Position).GridPosition, true, getExtraPathingCost);
            case PathGoal.NearestTower:
                break;
        }
        return null;
    }

    private Vector2 NodeGridToWorld(Point grid)
    {
        return Nodes[grid.X, grid.Y].WorldPosition;
    }

    public struct PathSettings
    {
        public required PathGoal Goal;
    }

    public enum PathGoal
    {
        Garage,
        Player,
        NearestTower,
    }


    private PriorityQueue<Point, float> _pathingFrontier = new PriorityQueue<Point, float>();
    private Dictionary<Point, Point> _pathingCameFrom = [];
    private Dictionary<Point, float> _pathingCostSoFar = [];

    public List<PathNode> FindPointPath(Point start, Point goal, bool crashIfNotFound = true, Func<Vector2, float> getExtraCost = null)
    {
        Point[] directions = Directions.ToArray();
        random.Shuffle(directions);
        // Priority Queue of Points with a float priority
        _pathingFrontier.Clear();
        // Maps paths of Grid Locations
        _pathingCameFrom.Clear();
        // Maps minimum cost for each Grid Location
        _pathingCostSoFar.Clear();

        // Add start with a priority and cost of zero
        // (Lower priorities are chosen first!)
        _pathingFrontier.Enqueue(start, 0.0f);
        _pathingCostSoFar.Add(start, 0.0f);

        // Will be set to true when a path is found
        bool found = false;

        // While there are locations on the frontier...
        while (_pathingFrontier.Count > 0)
        {
            // Get and Dequeue the next location on the frontier
            Point current = _pathingFrontier.Dequeue();

            // End early if a path to the goal has been found
            if (current == goal)
            {
                found = true;
                break;
            }

            // Check all eight possible neighbors
            for (int index = 0; index < Directions.Length; index++)
            {
                // Direction to neighbor
                Point neighborDirection = Directions[index];
                // True if this neighbor is diagonally from the current node
                bool isDiagonal = DirectionDiagonals[index];

                // Get the grid position of this next neighbor
                Point next = current + neighborDirection;

                // Check if "next" is in the grid bounds and that is exists
                // CheckGetNode also returns the node (if found) through its "out" parameter
                bool inBounds = next.X >= 0 && next.Y >= 0 && next.X < Nodes.GetLength(0) && next.Y < Nodes.GetLength(1);
                bool exists = CheckGetNode(next, out PathNode node);

                // Skip out-of-bounds and non-existent nodes
                if (!inBounds || !exists)
                {
                    continue;
                }

                // The cost to move to this next node
                float costToNext = node.Cost;

                // Calculate any extra cost
                if (getExtraCost is not null)
                {
                    costToNext += getExtraCost(NodeGridToWorld(next));
                }

                // Multiply move costs for diagonals by Sqrt 2
                if (isDiagonal)
                {
                    costToNext *= SquareRootOfTwo;
                }

                // Full next cost is the cost for the ENTIRE path so far, plus the cost to move to the neighbor
                float fullNextCost = _pathingCostSoFar[current] + costToNext;

                // If there is a cost CURRENTLY of this next node, and it's LESS than the cost we just found,
                // skip this node, as we DON'T want to calculate HIGHER cost paths
                if (_pathingCostSoFar.TryGetValue(next, out float cost) && fullNextCost >= cost)
                {
                    continue;
                }

                // Save the cost to the current next node
                _pathingCostSoFar[next] = fullNextCost;
                // Calculate the priority from the cost and distance
                float priority = fullNextCost + OctileDistance(goal, next);
                // Enqueue this next node with its priority
                _pathingFrontier.Enqueue(next, priority);
                // Save the node to the path
                _pathingCameFrom[next] = current;
            }
        }

        // Throw an error if a path wasn't found and this is set
        if (!found)
        {
            if (crashIfNotFound)
            {
                throw new InvalidOperationException($"Could not find a path from {start} to {goal}");
            }
            return [];
        }

        // The path has been found! Now we turn it into a list for easier use
        // We Start with a list containing just the goal...
        List<PathNode> path = [ GetNode(goal) ];

        // Keep track of our current position, starting with the goal
        Point currentPosition = goal;
        // We follow the path through the "cameFrom" dictionary
        while (_pathingCameFrom.TryGetValue(currentPosition, out Point cameFromPoint))
        {
            // Save each node
            path.Add(GetNode(cameFromPoint));
            // And set the new position
            currentPosition = cameFromPoint;
        }
        path.RemoveAt(path.Count - 1);
        // We reverse the list since it's currently backwards
        path.Reverse();

        // Finally, we return the path
        return path;
    }

    public static float OctileDistance(Point first, Point second)
    {
        float xDifference = Math.Abs(first.X - second.X);
        float yDifference = Math.Abs(first.Y - second.Y);

        float minDifference = Math.Min(xDifference, yDifference);
        float maxDifference = Math.Max(xDifference, yDifference);

        return (SquareRootOfTwo - 1) * minDifference + maxDifference;
    }
}
