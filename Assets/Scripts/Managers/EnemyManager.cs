using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    
    private List<EnemyRuntime> _activeEnemies = new();
    private readonly Vector3 _positionOffset = new Vector3(0, -0.5f, 0);

    public void Register(EnemyRuntime enemy)
    {
        _activeEnemies.Add(enemy);
    }

    public void Unregister(EnemyRuntime enemy)
    {
        _activeEnemies.Remove(enemy);
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        var playerPos =  playerTransform.position + _positionOffset;
        
        foreach (var enemy in _activeEnemies)
        {
            var direction = playerPos - enemy.gameObject.transform.position;
            enemy.Tick(deltaTime, direction);
        }
    }
}
