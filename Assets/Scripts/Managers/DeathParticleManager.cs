using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject particlePrefab;
    
    private ObjectPool<GameObject> _objectPool = new();
    private Dictionary<GameObject, OnDeathParticles> _particleSystems = new();
    
    public static DeathParticleManager Instance;
    
    private void Awake()
    {
        Instance = this;
        
        EnemyManager.OnEnemyDied += RequestFromPool;
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemyDied -= RequestFromPool;
    }

    private void RequestFromPool(Transform spawnPos)
    {
        var obj = _objectPool.Get(particlePrefab, particlePrefab);

        if (!_particleSystems.TryGetValue(obj, out var particles))
        {
            particles = obj.GetComponent<OnDeathParticles>();
            _particleSystems.Add(obj, particles);
        }
        
        particles.PlayAnimation(spawnPos);
    }
    
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        _objectPool.Return(particlePrefab, obj);
    }
}
