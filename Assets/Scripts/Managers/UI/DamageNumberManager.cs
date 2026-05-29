using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class DamageNumberManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    
    private ObjectPool<GameObject> _objectPool = new();
    private Dictionary<GameObject, DamageNumber> _damageNumbers = new();

    private int _spawnEveryNFrames = 10;
    private int _frameCounter = 0;
    private int _spawned = 0;
    private int _spawnedLimit = 3;
    
    public static DamageNumberManager Instance;

    private void Awake()
    {
        Instance = this;

        EnemyRuntime.OnDamageTaken += RequestFromPool;
    }

    private void OnDestroy()
    {
        EnemyRuntime.OnDamageTaken -= RequestFromPool;
    }

    private void Update()
    {
        _frameCounter++;

        if (_frameCounter >= _spawnEveryNFrames)
        {
            _frameCounter = 0;
            _spawned = 0;
        }
    }

    private void RequestFromPool(Transform spawnPos, float value)
    {
        if (_spawned >= _spawnedLimit) return;
        
        _spawned++;
        var obj = _objectPool.Get(prefab, prefab);

        if (!_damageNumbers.TryGetValue(obj, out var damageNumber))
        {
            damageNumber = obj.GetComponent<DamageNumber>();
            _damageNumbers[obj] = damageNumber;
        }
        
        damageNumber.Initialize(spawnPos, value);
        // var damageNumber = obj.GetComponent<DamageNumber>();
        
        obj.SetActive(true);
    }

    public void ReturnToPool(DamageNumber damageNumber, GameObject go)
    {
        go.SetActive(false);
        _objectPool.Return(prefab, go);
    }
}
