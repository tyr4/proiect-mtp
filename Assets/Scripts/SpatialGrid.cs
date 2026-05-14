using System.Collections.Generic;
using UnityEngine;

public class SpatialGrid
{
    private readonly float _cellSize;
    private readonly Dictionary<Vector2Int, List<EnemyRuntime>> _grid = new();
    private List<EnemyRuntime> _results = new();

    public SpatialGrid(float cellSize)
    {
        _cellSize = cellSize;
    }

    public void Clear()
    {
        _grid.Clear();
    }

    public Vector2Int GetCell(Vector3 position)
    {
        var x = Mathf.FloorToInt(position.x / _cellSize);
        var y = Mathf.FloorToInt(position.y / _cellSize);
        
        return new Vector2Int(x, y);
    }

    public void Add(EnemyRuntime enemy)
    {
        Vector2Int cell = GetCell(enemy.cachedTransform.position);

        if (!_grid.TryGetValue(cell, out var list))
        {
            list = new();
            _grid[cell] = list;
        }
        
        list.Add(enemy);
    }

    // check neighboring cells in a 3x3 grid
    // something like this
    // []   []    []
    // [] [start] []
    // []   []    []
    public EnemyRuntime GetNearest(Vector3 position)
    {
        Vector2Int center = GetCell(position);
        EnemyRuntime nearest = null;
        float nearestSqrDist = float.MaxValue;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var cell = new Vector2Int(center.x + x, center.y + y);

                if (!_grid.TryGetValue(cell, out var list)) continue;

                for (int i = 0; i < list.Count; i++)
                {
                    float sqrDist = (list[i].cachedTransform.position - position).sqrMagnitude;
                
                    if (sqrDist < nearestSqrDist)
                    {
                        nearestSqrDist = sqrDist;
                        nearest = list[i];
                    }
                }
            }
        }

        return nearest;
    }
}
