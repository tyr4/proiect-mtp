using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float gridCellSize;
    [SerializeField] private float separationRadius;

    private List<EnemyRuntime> _activeEnemies = new();
    // private readonly Vector3 _positionOffset = new Vector3(0, -0.5f, 0);
    // private float _gridRebuildCooldown = 0.01f;
    // private float timer = 0;

    public SpatialGrid Grid { get; private set; }
    
    public static event Action<GameObject> OnEnemySpawned;
    public static event Action<GameObject> OnEnemyDied;
    public static EnemyManager Instance;

    private void Awake()
    {
        Instance = this;
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
        var deltaTime = Time.fixedDeltaTime;
        
        // step 1: build the grid, runs every _gridRebuildCooldown seconds
        // GridRebuild(deltaTime);

        // step 2: now the enemies will tick and simulate crowd distance properly
        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            var enemy = _activeEnemies[i];
            
            enemy.Tick(deltaTime, playerTransform, null, separationRadius);
        }
    }

    private void GridRebuild()
    {
        Grid.Clear();

        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            Grid.Add(_activeEnemies[i]);
        }
    }

    public EnemyRuntime GetNearestEnemy(Vector3 position)
    {
        GridRebuild();

        return Grid.GetNearest(position);
    }
    
    private void OnDrawGizmos()
    {
        if (_activeEnemies == null) return;

        foreach (var enemy in _activeEnemies)
        {
            
            // red sphere at actual transform position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(enemy.cachedTransform.position, 0.15f);
        }
    }
}
