using System.Collections.Generic;
using UnityEngine;

public interface IGridProvider
{
    int CellSize { get; }

    bool CheckGridInPoint(Vector3 point);

    Vector2Int GetCellIndex(Vector3 point);

    Vector3 GetCellCenterFromIndex(Vector2Int index);

    Vector3 GetCellCenterFromPoint(Vector3 point);

    bool CheckCellValid(Vector2Int index, LayerMask obstacleLayer);

    void RegisterOccupancy(Vector2Int index, List<Vector2Int> offsetList, bool isOccupied);
}
