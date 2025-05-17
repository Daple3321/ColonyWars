using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridManager
{
    public Grid grid;
    public Vector3 cellSize;
    public Cell[,] cells;
    public int mapSize;

    private Renderer[,] planes;
    public GridManager(Grid grid, int mapSize) // mapSize = TerrainSize/CellSize 
    {
        this.grid = grid;
        this.mapSize = mapSize;
        cellSize = grid.cellSize;

        cells = new Cell[mapSize, mapSize];
        planes = new Renderer[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Cell newCell = new Cell
                {
                    worldPosition = new Vector3(
                        grid.CellToWorld(new Vector3Int(i, j)).x,
                        0,
                        grid.CellToWorld(new Vector3Int(i, j)).y),
                    captureProgress = 0,
                    whoIsCapturing = Affiliation.None,
                    yieldAmount = 1,
                    affiliation = Affiliation.None
                };
                cells[i, j] = newCell;

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = $"[{i}, {j}]";
                go.transform.position = grid.GetCellCenterWorld(new Vector3Int(i, 0, j));
                Bounds bd = grid.GetBoundsLocal(new Vector3Int(i, 0, j), new Vector3(1, 1, 1));
                go.transform.localScale = bd.size / 2;

                planes[i, j] = go.GetComponent<Renderer>();

            }
        }

        cells[0, 0].affiliation = Affiliation.Player;
        cells[1, 0].affiliation = Affiliation.Player;
        cells[0, 1].affiliation = Affiliation.Player;
        cells[1, 1].affiliation = Affiliation.Player;
        cells[2, 0].affiliation = Affiliation.Player;
        cells[3, 0].affiliation = Affiliation.Player;

        cells[4, 0].affiliation = Affiliation.Enemy;
        cells[0, 2].affiliation = Affiliation.Enemy;


        cells[4, 4].affiliation = Affiliation.Enemy;
        cells[3, 4].affiliation = Affiliation.Enemy;
        cells[4, 3].affiliation = Affiliation.Enemy;
        cells[5, 4].affiliation = Affiliation.Enemy;
        cells[4, 5].affiliation = Affiliation.Enemy;
        cells[5, 3].affiliation = Affiliation.Enemy;
        cells[3, 5].affiliation = Affiliation.Enemy;
        cells[5, 5].affiliation = Affiliation.Enemy;
        cells[3, 3].affiliation = Affiliation.Enemy;
        cells[6, 4].affiliation = Affiliation.Enemy;

        ColorGrid();

        Vector2Int closestCell = ClosestDifferentCell(4, 4);
        Debug.Log(closestCell.x + ", " + closestCell.y);

        Vector2Int closestCell2 = _ClosestDifferentCell(4, 4);
        Debug.Log(closestCell2.x + ", " + closestCell2.y);
    }

    private void ColorGrid()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (cells[i, j].affiliation == Affiliation.Player)
                {
                    planes[i, j].material.color = Color.green;
                }
                else if (cells[i, j].affiliation == Affiliation.Enemy)
                {
                    planes[i, j].material.color = Color.red;
                }
                else if (cells[i, j].affiliation == Affiliation.None)
                {
                    planes[i, j].material.color = Color.gray;
                }
            }
        }
    }

    public Cell GetCell(int x, int y) // можно и без проверки
    {
        if (!WithinBounds(x, y))
        {
            Debug.LogWarning("Trying to get cell out of bounds.");
            return Cell.GetEmptyCell();
        }

        return cells[x, y];
    }

    public bool WithinBounds(int x, int y)
    {
        if (x >= 0 && x < mapSize &&
            y >= 0 && y < mapSize)
        {
            //Debug.Log(cell + " within bounds");
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool WithinBounds(Vector2Int cell)
    {
        if (cell.x >= 0 && cell.x < mapSize &&
            cell.y >= 0 && cell.y < mapSize)
        {
            //Debug.Log(cell + " within bounds");
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector2Int[] GetNeighbors(int x, int y)
    {
        Vector2Int[] dirs = new Vector2Int[]{
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(-1,0),
            new Vector2Int(0,-1),
        };
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int neighbor = new Vector2Int(x + dir.x, y + dir.y);
            if (WithinBounds(neighbor))
            {
                result.Add(neighbor);
            }
        }

        return result.ToArray();
    }
    public Vector2Int[] GetNeighbors(int x, int y, Affiliation affiliation) // найти только определённых соседей
    {
        Vector2Int[] dirs = new Vector2Int[]{
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(-1,0),
            new Vector2Int(0,-1),
        };
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int neighbor = new Vector2Int(x + dir.x, y + dir.y);
            if (WithinBounds(neighbor) && GetCell(neighbor.x, neighbor.y).affiliation == affiliation)
            {
                result.Add(neighbor);
            }
        }

        return result.ToArray();
    }

    public Vector2Int[] GetDifferentNeighbors(int x, int y)
    {
        Vector2Int[] neighbors = GetNeighbors(x, y);
        Cell originCell = GetCell(x, y);

        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int neighbor in neighbors)
        {
            if (GetCell(neighbor.x, neighbor.y).affiliation != originCell.affiliation)
            {
                result.Add(neighbor);
            }
        }

        return result.ToArray();
    }

    public Vector2Int ClosestDifferentCell(int x, int y) // just straight up BULLSHIT
    {
        int maxIterations = 100;

        Cell origin = GetCell(x, y);

        bool isFound = false;
        Vector2Int lastCheckedCell = new Vector2Int(x, y);
        Vector2Int[] diffNeighbors;
        while (!isFound && maxIterations > 0)
        {
            diffNeighbors = GetDifferentNeighbors(lastCheckedCell.x, lastCheckedCell.y);
            if (diffNeighbors.Length > 0)
            {
                isFound = true;
                return diffNeighbors[0];
            }
            else
            {
                Vector2Int[] sameNeighbors = GetNeighbors(lastCheckedCell.x, lastCheckedCell.y, origin.affiliation);
                if (sameNeighbors.Length > 0)
                {
                    lastCheckedCell = sameNeighbors[Random.Range(0, sameNeighbors.Length)]; // всегда один и тот же будет?
                }
            }

            maxIterations--;
        }

        Debug.LogError("Different cell not found");
        return new Vector2Int(-1, -1);
    }
    
    public Vector2Int _ClosestDifferentCell(int x, int y)
    {
        Cell origin = GetCell(x, y);
        
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(new Vector2Int(x, y));

        bool isFound = false;
        while (!isFound || frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            foreach (Vector2Int next in GetNeighbors(current.x, current.y))
            {
                if (GetCell(next.x, next.y).affiliation != origin.affiliation)
                {
                    isFound = true;
                    return next;
                }
                else
                {
                    frontier.Enqueue(next);
                }
            }
        }

        Debug.LogError("Different cell not found");
        return new Vector2Int(-1, -1);
    }
}

[System.Serializable]
public struct Cell
{
    public Vector3 worldPosition;
    public float captureProgress;
    public Affiliation whoIsCapturing;
    public float yieldAmount;

    public Affiliation affiliation;
    
    
    
    public static Cell GetEmptyCell()
    {
        return new Cell
        {
            worldPosition = new Vector3(-1, -1, -1),
            captureProgress = 0,
            whoIsCapturing = Affiliation.None,
            yieldAmount = 1,
            affiliation = Affiliation.None,
        };
    }
    public static Cell GetEmptyCell(Vector3 worldPos) // why
    {
        return new Cell
        {
            worldPosition = new Vector3(
                worldPos.x,
                0,
                worldPos.y),
            captureProgress = 0,
            whoIsCapturing = Affiliation.None,
            yieldAmount = 1,
            affiliation = Affiliation.None,
        };
    }
}