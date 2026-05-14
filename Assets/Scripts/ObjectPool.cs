using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<TKey, TValue>
{
    private Dictionary<TKey, Queue<TValue>> _pool = new();

    public TValue Get(TKey key, GameObject prefab)
    {
        TValue poolObj;

        if (!_pool.TryGetValue(key, out var queue))
        {
            queue = new Queue<TValue>();
            _pool[key] = queue;
        }

        if (queue.Count > 0)
        {
            poolObj = queue.Dequeue();
        }
        else
        {
            var newObj = Object.Instantiate(prefab);
            newObj.SetActive(false);
            poolObj = newObj.GetComponent<TValue>();
        }
        
        return poolObj;
    }

    public void Return(TKey key, TValue value)
    {
        if (!_pool.TryGetValue(key, out var queue))
        {
            queue = new Queue<TValue>();
            _pool[key] = queue;
        }

        _pool[key].Enqueue(value);
    }
}
