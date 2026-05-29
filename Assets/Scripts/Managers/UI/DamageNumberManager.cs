using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class DamageNumberManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    
    private ObjectPool<GameObject> _objectPool = new();
    
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

    private void RequestFromPool(Transform spawnPos, float value)
    {
        var obj = _objectPool.Get(prefab, prefab);
        var damageNumber = obj.GetComponent<DamageNumber>();
        
        damageNumber.Initialize(spawnPos, value);
        obj.SetActive(true);
    }

    public void ReturnToPool(DamageNumber damageNumber, GameObject go)
    {
        go.SetActive(false);
        _objectPool.Return(prefab, go);
    }
}
