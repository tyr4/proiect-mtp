using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private EnemyManager enemyManager; // for the grid
    [SerializeField] private Transform playerTransform;
    
    private List<ProjectileRuntime> _activeProjectiles = new();

    public static ProjectileManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    private void Update()
    {
        float dt = Time.deltaTime;
        var playerPos =  playerTransform.position;
        var nearestEnemy = GetNearestEnemyPos(playerPos);
        
        for (int i = 0; i < _activeProjectiles.Count; i++)
        {
            var proj =  _activeProjectiles[i];
        
            proj.Tick(dt, playerPos, nearestEnemy, this, enemyManager.Grid);
        }
    }

    public void Register(ProjectileRuntime projectile)
    {
        _activeProjectiles.Add(projectile);
    }

    public void Unregister(ProjectileRuntime projectile)
    {
        _activeProjectiles.Remove(projectile);
    }

    private Vector2 GetNearestEnemyPos(Vector3 playerPos)
    {
        throw new Exception("not implemented fraiere");
    }
}
