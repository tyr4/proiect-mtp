using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float gridCellSize;
    [SerializeField] private float separationRadius;

    private HashSet<EnemyRuntime> _activeEnemies = new();
    private Dictionary<Enemy, int> _enemyCounts = new();  // this is for the wavemanager
    private List<EnemyRuntime> _pendingDeletes = new();
    
    // private readonly Vector3 _positionOffset = new Vector3(0, -0.5f, 0);
    // private float _gridRebuildCooldown = 0.01f;
    // private float timer = 0;

    public SpatialGrid Grid { get; private set; }
    
    public static event Action<Transform> OnEnemySpawned;
    public static event Action<Transform> OnEnemyDied;
    public static EnemyManager Instance;

    private void Awake()
    {
        Instance = this;
        Grid = new SpatialGrid(gridCellSize);
    }
    
    public void Register(EnemyRuntime enemy)
    {
        _activeEnemies.Add(enemy);

        if (_enemyCounts.ContainsKey(enemy.Data))
            _enemyCounts[enemy.Data]++;
        
        else _enemyCounts[enemy.Data]= 1;
        
        OnEnemySpawned?.Invoke(enemy.cachedTransform);
    }

    public void Unregister(EnemyRuntime enemy)
    {
        _pendingDeletes.Add(enemy);
        _enemyCounts[enemy.Data]--;
        
        OnEnemyDied?.Invoke(enemy.cachedTransform);
    }

    private void FixedUpdate()
    {
        // clear dead enemies from the poll
        foreach (var e in _pendingDeletes)
        {
            _activeEnemies.Remove(e);
        }
        _pendingDeletes.Clear();
        
        var deltaTime = Time.fixedDeltaTime;

        foreach (var enemy in _activeEnemies)
        {
            enemy.Tick(deltaTime, playerTransform);
        }
    }

    private void GridRebuild()
    {
        Grid.Clear();
    
        foreach (var enemy in _activeEnemies)
        {
            Grid.Add(enemy);
        }
    }
    
    public EnemyRuntime GetNearestEnemy(Vector3 position)
    {
        GridRebuild();
    
        return Grid.GetNearest(position);
    }

    public int GetActiveEnemiesCount()
    {
        return _activeEnemies.Count;
    }

    public int GetActiveEnemiesCountByType(Enemy enemy)
    {
        return _enemyCounts.GetValueOrDefault(enemy, 0);
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
