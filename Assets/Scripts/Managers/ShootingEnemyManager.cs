using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyManager : MonoBehaviour
{
    private ObjectPool<ShootingEnemy> _objectPool = new();

    public static ShootingEnemyManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // public IEnemyProjectileBehaviour GetOrCreateSpawner(ShootingEnemy enemy)
    // {
    //     IEnemyProjectileBehaviour spawner = null;
    //     
    //         spawner = Instantiate(enemy.ProjectileSpawner).GetComponent<IEnemyProjectileBehaviour>();
    //         _spawnerCache[enemy] = spawner;
    //
    //     return spawner;
    // }
    
    public GameObject RequestPoolObject(ShootingEnemy enemy)
    {
        return _objectPool.Get(enemy, enemy.ProjectilePrefab);
    }

    public void ReturnPoolObject(ShootingEnemy enemy, GameObject obj)
    {
        obj.SetActive(false);
        _objectPool.Return(enemy, obj);
    }
}
