
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private WaveDataContainer waveContainer;

    [SerializeField] private int maxEnemiesAlive;
    [SerializeField] private float spawnRadiusFactor;
    [SerializeField] private bool disableSpawns;
    
    private ObjectPool<Enemy> _objectPool = new();
    private List<WaveData> _waves = new();
    private List<GameObject> _enemiesThisWave = new();
    private WaveData _currentWave;
    
    private int _currentWaveIndex = 0;
    private bool IsLastWave => _currentWaveIndex >= _waves.Count - 1;
    
    private Camera _camera;
    private float _cameraHeight;
    private float _cameraWidth;
    private float _cameraRadius;
    private float _spawnRadius;

    private float _globalTimer = 0;
    private float _waveTimer = 0;
    private float _clockTimer = 0;
    
    public static WaveManager Instance;
    public static Action<float> OnSecondIncrease;

    private void Awake()
    {
        Instance = this;

        _waves = waveContainer.waves;
        _waves.Sort((a, b) => a.startTime.CompareTo(b.startTime));
        
        _currentWave = _waves[0];
        
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
        
        OnSecondIncrease?.Invoke(_globalTimer);
    }
    
    private void Update()
    {
        var dt = Time.deltaTime;
        
        _globalTimer += dt;
        _waveTimer += dt;
        _clockTimer += dt;

        // ui clock stuff
        if (_clockTimer >= 1f)
        {
            OnSecondIncrease?.Invoke(_globalTimer);
            _clockTimer -= 1f;
        }

        // update the wave data if needed
        if (_globalTimer >= _currentWave.startTime && !IsLastWave)
        {
            AdvanceWave();
        }
        
        // spawn wave every spawnInterval seconds
        if (_waveTimer >= _currentWave.spawnInterval)
        {
            if (disableSpawns) return;
            
            SpawnWave();
            _waveTimer = 0;
        }
    }

    private void SpawnWave()
    {
        if (EnemyManager.Instance.GetActiveEnemiesCount() >= maxEnemiesAlive) return;
        
        _enemiesThisWave.Clear();
        
        for (int i = 0; i < _currentWave.amount; i++)
        {
            var enemy = SpawnEnemy(_currentWave);
            _enemiesThisWave.Add(enemy);
        }

        if (!_currentWave.spawnAllAtOnce) return;

        foreach (var enemy in _enemiesThisWave)
        {
            enemy.SetActive(true);
        }
    }

    private GameObject SpawnEnemy(WaveData data)
    {
        // int choice = Random.Range(0, enemyContainer.Enemies.Count);
        var enemy = GetRandomEnemy(data);
        var enemyObj = _objectPool.Get(enemy, enemy.Prefab);
        var enemyData = enemyObj.GetComponent<EnemyRuntime>();

        IEnemyProjectileBehaviour spawner = null;
        if (enemy is ShootingEnemy shootingEnemy)
        {
            spawner = enemyObj.GetComponent<IEnemyProjectileBehaviour>();
        }
        
        enemyData.Initialize(enemy, spawner);
        enemyData.cachedTransform.position = GenerateRandomPosition();
        
        enemyObj.SetActive(!data.spawnAllAtOnce);
        enemyManager.Register(enemyData);

        return enemyObj;
    }

    private Enemy GetRandomEnemy(WaveData data)
    {
        var enemyList = data.specialEnemies;
        float random = Random.value;
        float cumulative = 0;

        foreach (var entry in enemyList)
        {
            cumulative += entry.spawnChance;
            
            if (random <= cumulative)
            {
                return entry.enemy;
            }
        }

        return data.defaultEnemy;
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

    private void AdvanceWave()
    {
        if (IsLastWave) return;

        _currentWaveIndex++;
        _currentWave = _waves[_currentWaveIndex];
    }
}
