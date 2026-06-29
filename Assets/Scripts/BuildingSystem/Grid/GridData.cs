using UnityEngine;
using System.Collections.Generic;

public class GridData
{
    // 그리드
    private Dictionary<Vector2Int, CellData> grid = new Dictionary<Vector2Int, CellData>();

    public Dictionary<Vector2Int, CellData> GetGrid()
    {
        return grid;
    }

    // 셀 반환
    public CellData GetCell(Vector2Int index)
    {
        if (grid.TryGetValue(index, out CellData cell))
        {
            return cell;
        }

        return null;
    }

    public void AddCell(Vector2Int index)
    {
        if (grid != null && !grid.ContainsKey(index))
        {
            grid.Add(index, new CellData(index));
        }
    }

    public bool IsValidCell(Vector2Int index)
    {
        return grid.ContainsKey(index);
    }

    public bool IsCellOccupied(Vector2Int index)
    {
        if (grid.TryGetValue(index, out CellData cellData))
        {
            return cellData.isOccupied;
        }
        return true;
    }

    public void SetCellState(Vector2Int index, bool isOccupied)
    {
        if (grid.TryGetValue(index, out CellData cellData))
        {
            cellData.isOccupied = isOccupied;
        }
    }
}
