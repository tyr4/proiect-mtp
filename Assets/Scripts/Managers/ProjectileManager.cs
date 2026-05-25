using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager; // for the grid
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform parentContainer;
    
    private List<ProjectileRuntimeData> _activeProjectiles = new();
    private ObjectPool<Projectile> _objectPool = new();

    public static ProjectileManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        var playerPos =  playerTransform.position;
        var nearestEnemy = GetNearestEnemy(playerPos);
        
        if (nearestEnemy is null || !nearestEnemy.gameObject.activeInHierarchy) return;
        
        var nearestEnemyPos = nearestEnemy.cachedTransform.position;
        
        for (int i = 0; i < _activeProjectiles.Count; i++)
        {
            var proj =  _activeProjectiles[i];
        
            // TODO: add to enemymanager a BuildGrid() function
            proj.Tick(dt, playerPos, nearestEnemyPos, nearestEnemy.Velocity, this);
        }
    }

    public void Register(Projectile proj)
    {
        var owned = PowerupManager.Instance.FindPlayerPowerup(proj);

        var spawnerInstance = Instantiate(proj.Spawner);
        var spawner = spawnerInstance as IProjectileBehaviour;
        
        var projRuntime = new ProjectileRuntimeData(owned, spawner);
        
        // Debug.Log($"added {projRuntime} from {owned.Base}");
        _activeProjectiles.Add(projRuntime);
    }

    public void Unregister(ProjectileRuntimeData projectile)
    {
        _activeProjectiles.Remove(projectile);
    }
    
    private EnemyRuntime GetNearestEnemy(Vector3 playerPos)
    {
        var enemy = enemyManager.GetNearestEnemy(playerPos);

        return enemy;
    }

    public GameObject RequestPoolObject(Projectile projectile)
    {
        if (projectile is IAttachedToPlayer)
        {
            return _objectPool.Get(projectile, projectile.ProjectilePrefab, parentContainer);
        }
        
        return _objectPool.Get(projectile, projectile.ProjectilePrefab);
    }

    public void ReturnPoolObject(Projectile projectile, GameObject obj)
    {
        obj.gameObject.SetActive(false);
        _objectPool.Return(projectile, obj);
    }
}
