using System;
using TMPro;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    [SerializeField] private Projectile projectileSO;
    [SerializeField] private TMP_Text fpsCounter;
    [SerializeField] private TMP_Text enemiesAlive;
    
    private float _refreshRate = 0.5f;
    private float _deltaTime;
    private float _timer;
    private int _enemyCount = 0;
    
    private void Awake()
    {
        EnemyManager.OnEnemySpawned += OnEnemySpawned;
        EnemyManager.OnEnemyDied += OnEnemyDied;
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemySpawned -= OnEnemySpawned;
        EnemyManager.OnEnemyDied -= OnEnemyDied;
    }

    private void Update()
    {
        _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= _refreshRate)
        {
            fpsCounter.text = $"FPS: {Mathf.CeilToInt(1f / _deltaTime)}";
            _timer = 0f;
        }
    }

    private void UpdateEnemyText()
    {
        enemiesAlive.text = $"Enemies alive: {_enemyCount}";
    }
    
    private void OnEnemyDied(Transform obj)
    {
        _enemyCount--;
        UpdateEnemyText();
    }
    
    private void OnEnemySpawned(Transform obj)
    {
        _enemyCount++;
        UpdateEnemyText();
    }

    public void AddWeapon()
    {
        var proj = Instantiate(projectileSO);
        
        PowerupManager.Instance.UpdatePlayerPowerups(proj);
    }
    
    public void JumpTo30Minutes()
    {
        Debug.Log("am intrat 30 minutes");
        WaveManager.Instance.SetTime(1800);
        Debug.Log("am terminat 30 minutes");
    }
}
