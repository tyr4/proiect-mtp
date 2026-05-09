
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private List<EnemyData> enemyList;

    private float _cooldown = 1;
    private float _timer = 0;
    
    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _cooldown)
        {
            SpawnEnemy();
            _timer = 0;
        }
    }

    // TODO: more complex spawn logic
    private void SpawnEnemy()
    {
        int choice = Random.Range(0, enemyList.Count);
        var enemyData = enemyList[choice];

        var enemyRuntime = objectPool.Get(enemyData);
        
        enemyRuntime.gameObject.SetActive(true);
        enemyRuntime.Initialize(enemyData);
        
        enemyManager.Register(enemyRuntime);
    }
}
