
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private List<EnemyData> enemyList;
    [SerializeField] private float spawnRadiusFactor;

    private ObjectPool<EnemyData, EnemyRuntime> _objectPool;
    private Camera _camera;
    private float _cameraHeight;
    private float _cameraWidth;
    private float _cameraRadius;
    private float _spawnRadius;
    
    private float _cooldown = 0.75f;
    private float _timer = 0;

    private void Awake()
    {
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
        var enemyRuntime = _objectPool.Get(enemyData, enemyData.Prefab);
        var enemyObj = enemyRuntime.gameObject;

        enemyRuntime.cachedTransform.position = GenerateRandomPosition();
        enemyRuntime.Initialize(enemyData);
        
        enemyObj.SetActive(true);
        enemyManager.Register(enemyRuntime);
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
}
