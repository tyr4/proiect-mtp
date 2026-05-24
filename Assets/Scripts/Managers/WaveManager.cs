
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private EnemyContainer enemyContainer;
    [SerializeField] private float spawnRadiusFactor;
    [SerializeField] private float waveCooldown = 3f;
    [SerializeField] private bool disableSpawns;
    
    private ObjectPool<Enemy> _objectPool = new();
    private Camera _camera;
    private float _cameraHeight;
    private float _cameraWidth;
    private float _cameraRadius;
    private float _spawnRadius;
    
    private float _timer = 0;

    public static WaveManager Instance;

    private void Awake()
    {
        Instance = this;
        
        _camera = Camera.main;
        
        if (_camera == null)
        {
            Debug.LogError("n ai camera bos");
            enabled = false;
            return;
        }
        
        _cameraHeight = _camera.orthographicSize;
        _cameraWidth = _cameraHeight * _camera.aspect;
        _cameraRadius = Mathf.Sqrt(_cameraWidth * _cameraWidth + _cameraHeight * _cameraHeight);

        _spawnRadius = _cameraRadius + spawnRadiusFactor;
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= waveCooldown)
        {
            if (disableSpawns) return;
            SpawnEnemy();
            _timer = 0;
        }
    }

    // TODO: more complex spawn logic
    public void SpawnEnemy()
    {
        int choice = Random.Range(0, enemyContainer.Enemies.Count);
        var enemy = enemyContainer.Enemies[choice];
        var enemyObj = _objectPool.Get(enemy, enemy.Prefab);
        var enemyData = enemyObj.GetComponent<EnemyRuntime>();

        IEnemyProjectileBehaviour spawner = null;
        if (enemy is ShootingEnemy shootingEnemy)
        {
            spawner = enemyObj.GetComponent<IEnemyProjectileBehaviour>();
        }
        
        enemyData.Initialize(enemy, spawner);
        enemyData.cachedTransform.position = GenerateRandomPosition();
        
        enemyObj.SetActive(true);
        enemyManager.Register(enemyData);
    }

    private Vector2 GenerateRandomPosition()
    {
        Vector2 dir = Random.insideUnitCircle;
        
        if (dir == Vector2.zero)
        {
            dir = Vector2.right;
        }
        dir.Normalize();

        Vector2 spawnPos = (Vector2)playerTransform.position + dir * _spawnRadius;

        return spawnPos;
    }

    public void ReturnToPool(Enemy data, EnemyRuntime enemy)
    {
        // enemyManager.Unregister(enemy);
        _objectPool.Return(data, enemy.gameObject);
        enemy.gameObject.SetActive(false);
    }
}
