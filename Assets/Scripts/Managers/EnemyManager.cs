using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float gridCellSize;
    [SerializeField] private float separationRadius;

    private List<EnemyRuntime> _activeEnemies = new();
    private readonly Vector3 _positionOffset = new Vector3(0, -0.5f, 0);

    public SpatialGrid Grid { get; private set; }
    
    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<GameObject> OnEnemyDied;

    private void Awake()
    {
        Grid = new SpatialGrid(gridCellSize);
    }
    
    public void Register(EnemyRuntime enemy)
    {
        _activeEnemies.Add(enemy);
        OnEnemySpawned?.Invoke(enemy.gameObject);
    }

    public void Unregister(EnemyRuntime enemy)
    {
        _activeEnemies.Remove(enemy);
        OnEnemyDied?.Invoke(enemy.gameObject);
    }

    private void FixedUpdate()
    {
        var deltaTime = Time.deltaTime;
        var playerPos =  playerTransform.position + _positionOffset;
        Grid.Clear();
        
        // step 1: build the grid
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            Grid.Add(_activeEnemies[i]);
            
        }
        
        // step 2: now the enemies will tick and simulate crowd distance properly
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            var enemy = _activeEnemies[i];
            var enemyPos = enemy.cachedTransform.position;
            var direction = playerPos - enemyPos;
            // var neighbors = _spatialGrid.GetNearby(enemyPos);
            
            enemy.Tick(deltaTime, direction, null, separationRadius);
        }
    }
}
