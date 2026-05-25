using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<TKey>
{
    private Dictionary<TKey, Queue<GameObject>> _pool = new();

    public GameObject Get(TKey key, GameObject prefab, Transform parent = null)
    {
        GameObject poolObj;

        if (!_pool.TryGetValue(key, out var queue))
        {
            queue = new Queue<GameObject>();
            _pool[key] = queue;
        }

        if (queue.Count > 0)
        {
            poolObj = queue.Dequeue();
        }
        else
        {
            var newObj = Object.Instantiate(prefab, parent);
            newObj.SetActive(false);
            poolObj = newObj;
        }
        
        return poolObj;
    }

    public void Return(TKey key, GameObject value)
    {
        if (!_pool.TryGetValue(key, out var queue))
        {
            queue = new Queue<GameObject>();
            _pool[key] = queue;
        }

        _pool[key].Enqueue(value);
    }
}
