
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
    private WaveData _nextWave;
    private AudioClip _bossMusic;
    private AudioSource _musicAudioSource;
    
    private int _currentWaveIndex = 0;
    private bool IsLastWave => _currentWaveIndex >= _waves.Count - 1;

    private bool _bossSpawnedThisWave;
    private bool _bossIsAlive;
    public EnemyRuntime _bossRuntime;
    public Boss _currentBoss;
    
    private Camera _camera;
    private float _cameraHeight;
    private float _cameraWidth;
    private float _cameraRadius;
    private float _spawnRadius;

    private float _globalTimer = 0;
    private float _waveTimer = 0;
    private float _clockTimer = 0;
    private float _clockTimerUpdateFrequency = 0.2f;
    private float _bossTimer = 0;
    
    public static WaveManager Instance;
    public static Action<float> OnTimeChanged;
    public static Action<Boss> OnBossSpawned;

    private void Awake()
    {
        Instance = this;

        _waves = waveContainer.waves;
        _waves.Sort((a, b) => a.startTime.CompareTo(b.startTime));
        
        _currentWave = _waves[0];
        _nextWave = _waves[1];
        
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

    private void Start()
    {
        _musicAudioSource = AudioManager.Instance.MusicSource;
    }

    private void OnEnable()
    {
        EnemyRuntime.OnBossDied += OnBossDied;
    }
    
    private void OnDisable()
    {
        EnemyRuntime.OnBossDied -= OnBossDied;
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        
        _waveTimer += dt;
        _clockTimer += dt;
        if (!_bossIsAlive || _bossTimer <= 0) _globalTimer += dt;
        else HandleBossTimer(dt);
        
        // ui clock stuff
        if (_clockTimer >= _clockTimerUpdateFrequency)
        {
            var timer = _bossIsAlive ? _bossTimer : _globalTimer;
            OnTimeChanged?.Invoke(timer);
            _clockTimer = 0;
        }
        
        // update the wave data if needed
        if (_globalTimer >= _nextWave.startTime && !IsLastWave)
        {
            AdvanceWave();
        }
        
        // spawn wave every spawnInterval seconds
        if (_waveTimer >= _currentWave.spawnInterval || enemyManager.GetActiveEnemiesCount() == 0)
        {
            if (disableSpawns) return;
            
            SpawnWave();
            _waveTimer = 0;
        }
    }

    private void SpawnWave()
    {
        if (EnemyManager.Instance.GetActiveEnemiesCount() >= maxEnemiesAlive) return;
        
        // spawn boss if it exists
        if (_currentWave.boss && !_bossSpawnedThisWave)
        {
            SpawnBoss(_currentWave.boss);
            _bossTimer = _currentWave.boss.BossMusic.length;
            Debug.Log($"boss music length {_bossTimer}");
            
            OnBossSpawned?.Invoke(_currentWave.boss);
        }
        
        // spawn the rest of the enemies
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
        
        enemyData.Initialize(enemy, _globalTimer, spawner);
        enemyData.cachedTransform.position = GenerateRandomPosition();
        
        enemyObj.SetActive(!data.spawnAllAtOnce);
        enemyManager.Register(enemyData);

        return enemyObj;
    }

    public EnemyRuntime SpawnEnemy(Enemy enemy)
    {
        var enemyObj = _objectPool.Get(enemy, enemy.Prefab);
        var enemyData = enemyObj.GetComponent<EnemyRuntime>();

        IEnemyProjectileBehaviour spawner = null;
        if (enemy is ShootingEnemy shootingEnemy)
        {
            spawner = enemyObj.GetComponent<IEnemyProjectileBehaviour>();
        }
        
        enemyData.Initialize(enemy, _globalTimer, spawner);
        enemyData.cachedTransform.position = GenerateRandomPosition();
        
        enemyObj.SetActive(true);
        enemyManager.Register(enemyData);

        return enemyData;
    }

    private GameObject SpawnBoss(Boss boss)
    {
        var bossObj = _objectPool.Get(boss, boss.Prefab);
        _currentBoss = boss;
        _bossRuntime = bossObj.GetComponent<EnemyRuntime>();
        
        _bossRuntime.Initialize(boss, _globalTimer);
        _bossRuntime.cachedTransform.position = GenerateRandomPosition();
        
        bossObj.SetActive(true);
        enemyManager.Register(_bossRuntime);

        _bossSpawnedThisWave = true;
        _bossIsAlive = true;
        
        AudioEvents.RequestMusic(boss.BossMusic);
        
        return bossObj;
    }

    private Enemy GetRandomEnemy(WaveData data)
    {
        var enemyList = data.specialEnemies;
        float random = Random.value;
        float cumulative = 0;

        foreach (var entry in enemyList)
        {
            if (enemyManager.GetActiveEnemiesCountByType(entry.enemy) >= entry.maxCount)
                continue;
            
            cumulative += entry.spawnChance;
            
            if (random <= cumulative)
            {
                return entry.enemy;
            }
        }

        return data.defaultEnemy;
    }

    public Vector2 GenerateRandomPosition()
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
        
        _nextWave = _waves[Mathf.Min(_currentWaveIndex + 1, _waves.Count - 1)];
        
        _bossSpawnedThisWave = false;
        
        Debug.Log($"advanced wave to {_currentWave}");
    }

    public void SetTime(float value)
    {
        _globalTimer = value;
        Debug.Log($"set time to {_globalTimer}");
    }

    private void HandleBossTimer(float dt)
    {
        _bossTimer = _currentBoss.BossMusic.length - _musicAudioSource.time;
        
        if (_bossTimer <= 0 && _bossIsAlive)
        {
            _bossRuntime.Kill(true);
        }
    }

    private void OnBossDied()
    {
        _bossIsAlive = false;
        _currentBoss = null;
        _bossRuntime = null;
        
        AudioEvents.RequestMusic(AudioManager.Sounds.gameplay);
    }
}
