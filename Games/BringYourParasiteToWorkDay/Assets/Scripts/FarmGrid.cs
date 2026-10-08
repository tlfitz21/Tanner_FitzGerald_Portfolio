using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    private Vector2 origin = new Vector2(-8.6f, -4.83f);
    private float cellSize = 0.25f;
    private int height = 160;
    private int width = 160;
    [SerializeField] private float obstacleClearanceRadius;
    [SerializeField] private float blockedGoalSearchDistance;
    private bool[,] blockedCells;

    public Vector2 GetCellCenter(int x, int y)
    {
        float centerX = origin.x + (x + 0.5f) * cellSize;
        float centerY = origin.y + (y + 0.5f) * cellSize;
        return new Vector2(centerX, centerY);
    }

    void Awake()
    {
        blockedCells = new bool[width, height];

        // Optional: Visualize the grid in the editor
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 cellCenter = GetCellCenter(x, y);
                blockedCells[x, y] = Physics2D.OverlapCircle(
                    cellCenter, obstacleClearanceRadius, LayerMask.GetMask("Wall")) != null;
            }
        }
    }

    public Vector2[] aStar(Vector2 start, Vector2 goal)
    {
        if (blockedCells == null)
        {
            return new Vector2[0];
        }

        Vector2Int startCell = WorldToCell(start);
        Vector2Int goalCell = WorldToCell(goal);
        if (!IsInsideGrid(startCell) || !IsInsideGrid(goalCell) ||
            blockedCells[startCell.x, startCell.y])
        {
            return new Vector2[0];
        }

        // If the exact goal is too close to a wall for the farmer to occupy,
        // allow nearby walkable cells to serve as the destination.
        bool[,] goalCells = new bool[width, height];
        System.Collections.Generic.List<Vector2Int> goalOptions =
            new System.Collections.Generic.List<Vector2Int>();

        if (!blockedCells[goalCell.x, goalCell.y])
        {
            goalCells[goalCell.x, goalCell.y] = true;
            goalOptions.Add(goalCell);
        }
        else
        {
            int searchCells = Mathf.CeilToInt(blockedGoalSearchDistance / cellSize);
            int minX = Mathf.Max(0, goalCell.x - searchCells);
            int maxX = Mathf.Min(width - 1, goalCell.x + searchCells);
            int minY = Mathf.Max(0, goalCell.y - searchCells);
            int maxY = Mathf.Min(height - 1, goalCell.y + searchCells);
            float maxDistanceSquared = blockedGoalSearchDistance * blockedGoalSearchDistance;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (blockedCells[x, y])
                    {
                        continue;
                    }

                    Vector2Int candidate = new Vector2Int(x, y);
                    if ((GetCellCenter(x, y) - goal).sqrMagnitude <= maxDistanceSquared)
                    {
                        goalCells[x, y] = true;
                        goalOptions.Add(candidate);
                    }
                }
            }
        }

        if (goalOptions.Count == 0)
        {
            return new Vector2[0];
        }

        int[,] costFromStart = new int[width, height];
        bool[,] closed = new bool[width, height];
        Vector2Int[,] cameFrom = new Vector2Int[width, height];
        bool[,] hasParent = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                costFromStart[x, y] = int.MaxValue;
            }
        }

        MinHeap openSet = new MinHeap();
        costFromStart[startCell.x, startCell.y] = 0;
        openSet.Push(startCell, HeuristicToGoals(startCell, goalOptions));

        Vector2Int[] directions =
        {
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.down
        };

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Pop();
            if (closed[current.x, current.y])
            {
                continue;
            }

            if (goalCells[current.x, current.y])
            {
                return ReconstructPath(cameFrom, hasParent, startCell, current);
            }

            closed[current.x, current.y] = true;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;
                if (!IsInsideGrid(neighbor) || blockedCells[neighbor.x, neighbor.y] ||
                    closed[neighbor.x, neighbor.y])
                {
                    continue;
                }

                int tentativeCost = costFromStart[current.x, current.y] + 1;
                if (tentativeCost >= costFromStart[neighbor.x, neighbor.y])
                {
                    continue;
                }

                cameFrom[neighbor.x, neighbor.y] = current;
                hasParent[neighbor.x, neighbor.y] = true;
                costFromStart[neighbor.x, neighbor.y] = tentativeCost;
                int estimatedTotalCost = tentativeCost + HeuristicToGoals(neighbor, goalOptions);
                openSet.Push(neighbor, estimatedTotalCost);
            }
        }

        return new Vector2[0];
    }

    private Vector2Int WorldToCell(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - origin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.y - origin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    private bool IsInsideGrid(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < width && cell.y >= 0 && cell.y < height;
    }

    private int Heuristic(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    private int HeuristicToGoals(Vector2Int from, System.Collections.Generic.List<Vector2Int> goals)
    {
        int bestDistance = int.MaxValue;
        foreach (Vector2Int goal in goals)
        {
            bestDistance = Mathf.Min(bestDistance, Heuristic(from, goal));
        }
        return bestDistance;
    }

    private Vector2[] ReconstructPath(
        Vector2Int[,] cameFrom,
        bool[,] hasParent,
        Vector2Int startCell,
        Vector2Int goalCell)
    {
        System.Collections.Generic.List<Vector2> path =
            new System.Collections.Generic.List<Vector2>();
        Vector2Int current = goalCell;

        while (current != startCell)
        {
            path.Add(GetCellCenter(current.x, current.y));
            if (!hasParent[current.x, current.y])
            {
                return new Vector2[0];
            }
            current = cameFrom[current.x, current.y];
        }

        path.Reverse();
        return path.ToArray();
    }

    private class MinHeap
    {
        private struct Entry
        {
            public Vector2Int Cell;
            public int Priority;

            public Entry(Vector2Int cell, int priority)
            {
                Cell = cell;
                Priority = priority;
            }
        }

        private readonly System.Collections.Generic.List<Entry> entries =
            new System.Collections.Generic.List<Entry>();

        public int Count => entries.Count;

        public void Push(Vector2Int cell, int priority)
        {
            Entry entry = new Entry(cell, priority);
            entries.Add(entry);
            int index = entries.Count - 1;

            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (entries[parent].Priority <= entry.Priority)
                {
                    break;
                }

                entries[index] = entries[parent];
                index = parent;
            }

            entries[index] = entry;
        }

        public Vector2Int Pop()
        {
            Entry first = entries[0];
            Entry last = entries[entries.Count - 1];
            entries.RemoveAt(entries.Count - 1);

            if (entries.Count > 0)
            {
                int index = 0;
                while (true)
                {
                    int left = index * 2 + 1;
                    int right = left + 1;
                    if (left >= entries.Count)
                    {
                        break;
                    }

                    int child = right < entries.Count &&
                        entries[right].Priority < entries[left].Priority
                            ? right
                            : left;
                    if (entries[child].Priority >= last.Priority)
                    {
                        break;
                    }

                    entries[index] = entries[child];
                    index = child;
                }

                entries[index] = last;
            }

            return first.Cell;
        }
    }
}
