using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class GridManager
{
    public List<Cell> playerCells;
    public List<Cell> enemyCells;
    
    public Grid grid;
    public Vector3 cellSize;
    public float cellHalf;
    public Cell[,] cells;
    public int mapSize;
    public int gridDimension;
    public int outerCellOffset = 2;
    // если есть outerCellOffset то это размер отступа
    public float mapEdgeOffset;

    [Space(10), Header("Borders")]
    public float borderOffset = 0.45f;
    private BorderPool borderPool;
    
    public readonly Vector2Int[] dirs = new Vector2Int[]{
        new(1,0), // RIGHT >
        new(-1,0), // LEFT <
        new(0,1), //    UP /\
        new(0,-1), // DOWN \/
    };
    
    public enum Direction : byte
    {
        Left,
        Right,
        Top,
        Bottom,
        
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }

    private Renderer[,] planes;
    public GridManager(Grid grid, BorderPool borderPool, int dimension) // mapSize = TerrainSize/CellSize 
    {
        this.grid = grid;
        gridDimension = dimension;
        this.mapSize = this.gridDimension - outerCellOffset;
        this.borderPool = borderPool;
        cellSize = grid.cellSize;
        cellHalf = cellSize.x / 2;
        sharedPropertyBlock = new MaterialPropertyBlock();
        
        playerCells = new List<Cell>();
        enemyCells = new List<Cell>();
        
        borders = new List<GameObject>();
        cells = new Cell[this.gridDimension, this.gridDimension];
        planes = new Renderer[this.gridDimension, this.gridDimension];
        for (int i = outerCellOffset; i < this.gridDimension; i++)
        {
            for (int j = outerCellOffset; j < this.gridDimension; j++)
            {
                Cell newCell = new Cell
                {
                    worldPosition = new Vector3(
                        grid.CellToWorld(new Vector3Int(i, j)).x,
                        0,
                        grid.CellToWorld(new Vector3Int(i, j)).y),
                    captureProgress = 0,
                    whoIsCapturing = Affiliation.None,
                    yieldAmount = 0.1f,
                    affiliation = Affiliation.None
                };
                cells[i, j] = newCell;

                newCell.OnCaptured += OnCellCaptured;
                newCell.OnCaptureStarted += OnCaptureStarted;
                
                // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // go.name = $"[{i}, {j}]";
                // go.transform.position = grid.GetCellCenterWorld(new Vector3Int(i, 0, j));
                // Bounds bd = grid.GetBoundsLocal(new Vector3Int(i, 0, j), new Vector3(1, 1, 1));
                // go.transform.localScale = bd.size / 2;
                // GameObject.Destroy(go.GetComponent<Collider>());
                // planes[i, j] = go.GetComponent<Renderer>();
            }
        }
        
        //GameObject test = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //test.transform.position = RandomPointOnSide(cells[2, 2], Direction.Left, 4f);

        //cells[0, 0].affiliation = Affiliation.Player;
        //cells[1, 0].affiliation = Affiliation.Player;

        // cells[4, 0].affiliation = Affiliation.Enemy;
        // cells[0, 2].affiliation = Affiliation.Enemy;
        // cells[0,2].captureProgress = 1;
        // cells[4,0].captureProgress = 1;

        //ColorGrid();
        UpdateBorders();
        CalculateEdgeOffset();

        // Vector2Int closestCell = ClosestDifferentCell(4, 4);
        // Debug.Log(closestCell.x + ", " + closestCell.y);

        // Vector2Int closestCell2 = _ClosestDifferentCell(4, 4);
        // Debug.Log(closestCell2.x + ", " + closestCell2.y);
        
        //Debug.Log(RandomPointInCell(7,4));
        
        GameController.i.UpdatePointStats(playerCells, enemyCells);
    }
    
    private void CalculateEdgeOffset()
    {
        mapEdgeOffset = grid.cellSize.x * outerCellOffset;
    }
    
    private void OnCellCaptured(Cell cell, Affiliation capturer)
    {
        HandleCellCapture(cell, capturer);
        GameController.i.UpdatePointStats(playerCells, enemyCells);
        
        Debug.Log($"Cell {cell.worldPosition} captured by {capturer}.");
        ClearBorders();
        UpdateBorders();
    }
    private void OnCaptureStarted(Cell cell, Affiliation capturer)
    {
        switch(capturer)
        {
            case Affiliation.Player:
                if(enemyCells.Contains(cell)){
                    enemyCells.Remove(cell);
                }
            break;
            case Affiliation.Enemy:
                if(playerCells.Contains(cell)){
                    playerCells.Remove(cell);
                }
            break;
        }
        GameController.i.UpdatePointStats(playerCells, enemyCells);
        
        Debug.Log($"Capture started of {cell.worldPosition} by {capturer}.");
    }
    private void HandleCellCapture(Cell cell, Affiliation capturer)
    {
        switch(capturer)
        {
            case Affiliation.Player:
                if(enemyCells.Contains(cell)){
                    enemyCells.Remove(cell);
                }
                playerCells.Add(cell);
            break;
            case Affiliation.Enemy:
                if(playerCells.Contains(cell)){
                    playerCells.Remove(cell);
                }
                enemyCells.Add(cell);
            break;
        }
    }
    
    private List<GameObject> borders;
    public void UpdateBorders()
    {
        for (int i = outerCellOffset; i < gridDimension; i++)
        {
            for (int j = outerCellOffset; j < gridDimension; j++)
            {
                Cell origin = GetCell(i, j);
                Vector2Int[] neighbors = GetDifferentNeighbors(i, j);
                if (neighbors.Length > 0 && origin.affiliation != Affiliation.None)
                {
                    foreach (Vector2Int neighbor in neighbors)
                    {
                        GameObject go = borderPool.Pool.Get();
                        go.transform.localScale = new Vector3(cellSize.x - (borderOffset), 50, 1);
                        if (!borders.Contains(go))
                        {
                            borders.Add(go);
                        }

                        PositionBorder(go, new Vector2Int(i, j), neighbor);
                        ColorBorder(go, origin.affiliation);
                    }
                }
            }
        }
    }
    public void ClearBorders()
    {
        for(int i = 0; i < borders.Count; i++)
        {
            if(borders[i] == null) { borders.RemoveAt(i); }
            else
            {
                if(borders[i].activeInHierarchy){
                    borderPool.Pool.Release(borders[i]);
                }
            }
        }
    }

    public void PositionBorder(GameObject border, Vector2Int origin, Vector2Int neighbor)
    {
        Vector3 center = grid.GetCellCenterWorld(new Vector3Int(origin.x, 0, origin.y));

        if (neighbor.x > origin.x) // right
        {
            Vector3 pos = center + new Vector3(cellHalf - borderOffset, 0, 0);
            border.transform.position = pos;
            border.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        }
        else if (neighbor.x < origin.x) // left
        {
            Vector3 pos = center + new Vector3(-cellHalf + borderOffset, 0, 0);
            border.transform.position = pos;
            border.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        }
        else if (neighbor.y > origin.y) // up
        {
            Vector3 pos = center + new Vector3(0, 0, cellHalf - borderOffset);
            border.transform.position = pos;
            border.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
        else if (neighbor.y < origin.y) // down
        {
            Vector3 pos = center + new Vector3(0, 0, -cellHalf + borderOffset);
            border.transform.position = pos;
            border.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
    }

    MaterialPropertyBlock sharedPropertyBlock;
    public void ColorBorder(GameObject border, Affiliation originAffiliation)
    {
        switch (originAffiliation)
        {
            case Affiliation.Player:
                sharedPropertyBlock.SetColor("_BaseColor", GameAssets.colors.playerBorder);
                border.GetComponent<Renderer>().SetPropertyBlock(sharedPropertyBlock);
                break;
            case Affiliation.Enemy:
                sharedPropertyBlock.SetColor("_BaseColor", GameAssets.colors.enemyBorder);
                border.GetComponent<Renderer>().SetPropertyBlock(sharedPropertyBlock);
                break;
            case Affiliation.None:

                break;
        }
    }

    /*private void ColorGrid()
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
    }*/

    public Cell GetCell(int x, int y) // можно и без проверки
    {
        if (!WithinBounds(x, y))
        {
            //Debug.LogWarning("Trying to get cell out of bounds.");
            return null;
        }

        return cells[x, y];
    }

    public bool WithinBounds(int x, int y)
    {
        if (x >= outerCellOffset && x < gridDimension &&
            y >= outerCellOffset && y < gridDimension)
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
        if (cell.x >= outerCellOffset && cell.x < gridDimension &&
            cell.y >= outerCellOffset && cell.y < gridDimension)
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
        // Vector2Int[] dirs = new Vector2Int[]{
        //     new Vector2Int(1,0),
        //     new Vector2Int(0,1),
        //     new Vector2Int(-1,0),
        //     new Vector2Int(0,-1),
        // };
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
        // Vector2Int[] dirs = new Vector2Int[]{
        //     new Vector2Int(1,0),
        //     new Vector2Int(0,1),
        //     new Vector2Int(-1,0),
        //     new Vector2Int(0,-1),
        // };
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
    
    /// <summary>
    /// Оставляет только те ячейки, которые имеют соседа/соседей с affiliation == neighbourAff
    /// </summary>
    /// <param name="cells"></param>
    /// <param name="neighbourAff"></param>
    /// <returns></returns>
    public List<Vector2Int> FilterCellsWithNeighbours(List<Vector2Int> cells, Affiliation neighbourAff)
    {
        List<Vector2Int> finalCells = new();
        
        foreach (Vector2Int cell in cells)
        {
            if(HasNeighbourOfAffiliation(cell.x, cell.y, neighbourAff))
            {
                finalCells.Add(cell);
            }
        }
        
        return finalCells;
    }
    /// <summary>
    /// Оставляет только ячейки, у которых есть соседи с отличающейся affiliation
    /// </summary>
    /// <param name="cells"></param>
    /// <returns></returns>
    public List<Vector2Int> FilterCellsWithDiffNeighbours(List<Vector2Int> cells)
    {
        List<Vector2Int> finalCells = new();
        
        foreach (Vector2Int cell in cells)
        {
            if(HasNeighbourOfDiffAffiliation(cell.x, cell.y))
            {
                finalCells.Add(cell);
            }
        }
        
        return finalCells;
    }
    /// <summary>
    /// Оставляет только те ячейки, у которых есть соседи с отличающейся affiliation
    /// </summary>
    /// <param name="cells"></param>
    /// <returns></returns>
    public List<Vector2Int> FilterCellsWithDiffNeighbours(List<Cell> cells)
    {
        List<Vector2Int> finalCells = new();
        
        foreach (Cell c in cells)
        {
            Vector3Int cellIdx = grid.WorldToCell(c.worldPosition);
            
            if(HasNeighbourOfDiffAffiliation(cellIdx.x, cellIdx.z))
            {
                finalCells.Add(new Vector2Int(cellIdx.x, cellIdx.z));
            }
        }
        
        return finalCells;
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
    
    public bool HasNeighbourOfAffiliation(int x, int y, Affiliation aff)
    {
        foreach (Vector2Int dir in dirs)
        {
            Vector2Int neighbor = new Vector2Int(x + dir.x, y + dir.y);
            if (WithinBounds(neighbor) && GetCell(neighbor.x, neighbor.y).affiliation == aff)
            {
                return true;
            }
        }
        
        return false;
    }
    public bool HasNeighbourOfDiffAffiliation(int x, int y)
    {
        Cell origin = GetCell(x, y);
        foreach (Vector2Int dir in dirs)
        {
            Vector2Int neighbor = new Vector2Int(x + dir.x, y + dir.y);
            if (WithinBounds(neighbor) && GetCell(neighbor.x, neighbor.y).affiliation != origin.affiliation)
            {
                return true;
            }
        }
        
        return false;
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

    public Vector2Int _ClosestDifferentCell(int x, int y) // БЕСКОНЕЧНЫЙ ЛУП КОГДА ВСЕ ЯЧЕЙКИ НА КАРТЕ ОДНОЙ AFFILIATION!
    {
        Cell origin = GetCell(x, y);

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(new Vector2Int(x, y));

        // когда вся карта одинаковая, ячейки будут бесконечно добавлятся во frontier
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
                    frontier.Enqueue(next); // Вот это скорее всего вызывает баг
                }
            }
        }

        Debug.LogError("Different cell not found");
        return new Vector2Int(-1, -1);
    }
    public async UniTask<Vector2Int> FindClosestCell(int x, int y, Affiliation ofAffiliation)
    {
        return await UniTask.RunOnThreadPool(() => 
        {
            return ClosestCell(x, y, ofAffiliation);
        });
    }
    /// <summary>
    /// Находит ближайшую от координат x,y клетку с заданной ofAffiliation. Использует поиск в ширину (Breadth-first search)
    /// </summary>
    /// <param name="x">Origin of search</param>
    /// <param name="y">Origin of search</param>
    /// <param name="ofAffiliation">Какой affiliation искать</param>
    /// <returns></returns>
    public Vector2Int ClosestCell(int x, int y, Affiliation ofAffiliation)
    {
        if(!HasCellOfAffiliation(ofAffiliation)) {
            //Debug.LogWarning($"No cells found of affiliation: {ofAffiliation}");
            return new Vector2Int(-1, -1);
        }
        
        //Cell origin = GetCell(x, y);
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(new Vector2Int(x, y));
        
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(new Vector2Int(x, y));
        
        // когда вся карта одинаковая, ячейки будут бесконечно добавлятся во frontier
        //bool isFound = false;
        while (frontier.Count > 0) 
        {
            Vector2Int current = frontier.Dequeue();
            
            if (GetCell(current.x, current.y).affiliation == ofAffiliation)
            {
                return current;
            }
            
            foreach (Vector2Int next in GetNeighbors(current.x, current.y))
            {
                // Проверяем, не посещали ли мы эту ячейку ранее
                if(!visited.Contains(next))
                {
                    if (GetCell(next.x, next.y).affiliation == ofAffiliation)
                    {
                        //isFound = true;
                        return next; // Нашли ближайшую ячейку
                    }
                    
                    // Если не нашли, добавляем в очередь и помечаем как посещенную
                    frontier.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        //Debug.LogError("Different cell not found");
        return new Vector2Int(-1, -1);
    }
    
    public Vector2Int FindCellIterations(int x, int y, int iterationsAmount)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(new Vector2Int(x, y));
        
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        visited.Add(new Vector2Int(x, y));
        
        int iterationsDone = 0;
        while (frontier.Count > 0 || iterationsDone <= iterationsAmount) 
        {
            Vector2Int current = frontier.Dequeue();
            
            // if (iterationsDone == iterationsAmount)
            // {
            //     return current;
            // }
            Vector2Int[] neighbors = GetNeighbors(current.x, current.y);
            ArrayExtensionMethods.Shuffle(neighbors);
            foreach (Vector2Int next in neighbors)
            {
                // Проверяем, не посещали ли мы эту ячейку ранее
                if(!visited.Contains(next))
                {
                    if(iterationsDone == iterationsAmount){
                        return neighbors[Random.Range(0, neighbors.Length)];
                    }
                    
                    // Если ещё не закончились итерации, добавляем в очередь и помечаем как посещенную
                    frontier.Enqueue(next);
                    visited.Add(next);
                }
            }
            iterationsDone++;
        }

        return new Vector2Int(-1, -1);
    }
    
    /// <summary>
    /// Остались ли в мире ячейки с заданной affiliation
    /// </summary>
    /// <param name="aff"></param>
    /// <returns></returns>
    public bool HasCellOfAffiliation(Affiliation aff)
    {
        for (int i = outerCellOffset; i < gridDimension; i++)
        {
            for (int j = outerCellOffset; j < gridDimension; j++)
            {
                if(GetCell(i, j).affiliation == aff){
                    return true;
                }
            }
        }
        
        return false;
    }

    public Collider[] GetCellBuildings(int x, int y)
    {
        Vector3Int cell = new Vector3Int(x, 0, y);
        
        return Physics.OverlapBox(
            grid.GetCellCenterWorld(new Vector3Int(x, 0, y)), 
            grid.GetBoundsLocal(cell).extents, 
            Quaternion.identity, 
            LayerMask.GetMask("PlayerBuilding","EnemyBuilding")
        );
    }
    public Collider[] GetCellBuildings(int x, int y, Affiliation ofAffiliation)
    {
        Vector3Int cell = new Vector3Int(x, 0, y);
        LayerMask mask;
        if(ofAffiliation == Affiliation.Player){
            mask = LayerMask.GetMask("PlayerBuilding");
        }
        else{
            mask = LayerMask.GetMask("EnemyBuilding");
        }
        
        return Physics.OverlapBox(
            grid.GetCellCenterWorld(new Vector3Int(x, 0, y)), 
            grid.GetBoundsLocal(cell).extents, 
            Quaternion.identity, 
            mask
        );
    }
    public bool CheckCellForBuildings(int x, int y, Affiliation ofAffiliation)
    {
        Collider[] b = GetCellBuildings(x, y);
        foreach (Collider c in b){
            if(c.GetComponent<Building>().affiliation == ofAffiliation){
                return true;
            }
        }
        
        return false;
    }
    
    public Vector3 RandomPointInCell(int x, int y)
    {
        Vector3 finalPoint;
        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(x, 0, y));
        
        float randX = Random.Range(-cellHalf, cellHalf);
        float randY = Random.Range(-cellHalf, cellHalf);
        finalPoint = new Vector3(cellCenter.x+randX, 0, cellCenter.z+randY);
        finalPoint = GameController.TerrainPoint(finalPoint); // ground height correction
        
        return finalPoint;
    }
    public Vector3 RandomPointInCell(Cell cell)
    {
        Vector3 finalPoint;
        Vector3Int cellIndex = grid.WorldToCell(cell.worldPosition);
        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(cellIndex.x, 0, cellIndex.z));
        
        float randX = Random.Range(-cellHalf, cellHalf);
        float randY = Random.Range(-cellHalf, cellHalf);
        finalPoint = new Vector3(cellCenter.x+randX, 0, cellCenter.z+randY);
        finalPoint = GameController.TerrainPoint(finalPoint); // ground height correction
        
        return finalPoint;
    }
    
    public Vector3Int RandomCellIndex(){
        return new Vector3Int(Random.Range(outerCellOffset, mapSize), 0, Random.Range(outerCellOffset, mapSize));
    }
    public Cell RandomCell(){
        Vector3Int randIndex = new Vector3Int(Random.Range(outerCellOffset, mapSize), 0, Random.Range(outerCellOffset, mapSize));
        return cells[randIndex.x,randIndex.z];
    }
    
    public Vector3 RandomPointOnSide(Cell cell, Direction side, float padding = 2f)
    {
        Vector3 finalPoint;
        Vector3Int cellIndex = grid.WorldToCell(cell.worldPosition);
        Vector3 cellCenter = grid.GetCellCenterWorld(new Vector3Int(cellIndex.x, 0, cellIndex.z));
        
        float randX;
        float randZ;
        switch(side){
        case Direction.Left:
            randZ = Random.Range(-cellHalf, cellHalf);
            finalPoint = new Vector3(cellCenter.x+cellHalf-padding, 0, cellCenter.z+randZ);
        break;
        
        case Direction.Right:
            randZ = Random.Range(-cellHalf, cellHalf);
            finalPoint = new Vector3(cellCenter.x-cellHalf+padding, 0, cellCenter.z+randZ);
        break;
        
        case Direction.Top:
            randX = Random.Range(-cellHalf, cellHalf);
            finalPoint = new Vector3(cellCenter.x+randX, 0, cellCenter.z+cellHalf-padding);
        break;
        
        case Direction.Bottom:
            randX = Random.Range(-cellHalf, cellHalf);
            finalPoint = new Vector3(cellCenter.x+randX, 0, cellCenter.z-cellHalf+padding);
        break;
        
        default:
            finalPoint = new Vector3(cellCenter.x, 0, cellCenter.z);
        break;
        }
        
        return GameController.TerrainPoint(finalPoint);
    }
    
    // public Cell[] FindCells(Predicate<Cell> match)
    // {
    //     List<List<Cell>> cellsList = new List<List<Cell>>(); // wat da fuuuu
    //     for(int i = 0; i < cells.Length; i++)
    //     {
    //         for(int j = 0; j < cells.Length; i++)
    //         {
    //             cellsList[i][j] = cells[i, j];
    //         }
    //     }
    //     Dictionary<Vector2Int, Cell> testDict = new Dictionary<Vector2Int, Cell>();
    //     testDict.First(x=>x.Value.affiliation == Affiliation.Player);
    //     //return cellsList.Find(x => x.Find(match).).ToArray();
    // }
    
    public void CaptureCell(int x, int y, Affiliation affiliation) // delete that shi?
    {
        cells[x, y].Capture(affiliation);
    }
}

[System.Serializable]
public class Cell
{
    public Vector3 worldPosition;
    public float captureProgress;
    public Affiliation whoIsCapturing;
    public float yieldAmount;

    public Affiliation affiliation;
    
    //public Action OnCellBuildingsChanged;
    public Action<Building> OnCellBuildingAdded;
    public Action<Building> OnCellBuildingRemoved;
    public Action<Cell, Affiliation> OnCaptured;
    public Action<Cell, Affiliation> OnCaptureStarted;
    
    public void StartCapture(Affiliation capturer)
    {
        whoIsCapturing = capturer;
        
        OnCaptureStarted?.Invoke(this, capturer);
    }
    
    public void Capture(Affiliation capturer)
    {
        affiliation = capturer;
        whoIsCapturing = Affiliation.None;
        captureProgress = 1;
        OnCaptured?.Invoke(this, capturer);
    }
    
    public void ResetCapture()
    {
        captureProgress = 0f;
        whoIsCapturing = Affiliation.None;
    }
    
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