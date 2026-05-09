using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<EnemyData, Queue<EnemyRuntime>> _pool = new();

    public EnemyRuntime Get(EnemyData data)
    {
        EnemyRuntime enemy;
        GameObject obj;
        
        if (!_pool.TryGetValue(data, out var queue))
        {
            queue = new Queue<EnemyRuntime>();
            _pool[data] = queue;
        }

        if (queue.Count > 0)
        {
            enemy = queue.Dequeue();
        }
        else
        {
            obj = Instantiate(data.Prefab);
            enemy = obj.GetComponent<EnemyRuntime>();
        }
        
        return enemy;
    }

    public void Return(EnemyRuntime enemy)
    {
        enemy.gameObject.SetActive(false);
        
        if (!_pool.TryGetValue(enemy.Data, out var queue))
        {
            queue = new Queue<EnemyRuntime>();
            _pool[enemy.Data] = queue;
        }
        
        _pool[enemy.Data].Enqueue(enemy);
    }
}
