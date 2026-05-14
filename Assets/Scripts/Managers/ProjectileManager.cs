using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager; // for the grid
    [SerializeField] private Transform playerTransform;

    private List<ProjectileRuntimeData> _activeProjectiles = new();
    private ObjectPool<Projectile, ProjectileBulletRuntime> _objectPool = new();

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
        
        if (nearestEnemy is null) return;
        
        var nearestEnemyPos = nearestEnemy.cachedTransform.position;
        
        for (int i = 0; i < _activeProjectiles.Count; i++)
        {
            var proj =  _activeProjectiles[i];
        
            // TODO: add to enemymanager a BuildGrid() function
            proj.Tick(dt, playerPos, nearestEnemyPos, nearestEnemy.Velocity, this, enemyManager.Grid);
        }
    }

    public void Register(ProjectileRuntimeData projectile)
    {
        _activeProjectiles.Add(projectile);
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

    public ProjectileBulletRuntime RequestPoolObject(Projectile projectile)
    {
        return _objectPool.Get(projectile, projectile.ProjectilePrefab);
    }

    public void ReturnPoolObject(Projectile projectile, ProjectileBulletRuntime obj)
    {
        obj.gameObject.SetActive(false);
        _objectPool.Return(projectile, obj);
    }
}
