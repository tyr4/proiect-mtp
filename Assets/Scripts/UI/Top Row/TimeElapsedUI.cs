using System;
using System.Text;
using TMPro;
using UnityEngine;

public class TimeElapsedUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private VertexGradient bossTextGradient;
    [SerializeField] private Color bossGradientBaseColor;
    
    private bool _isBossAlive;
    
    private void Awake()
    {
        BuildText(0);
    }
    
    private void OnEnable()
    {
        WaveManager.OnTimeChanged += BuildText;
        WaveManager.OnBossSpawned += HandleBossSpawn;
        
        EnemyRuntime.OnBossDied += HandleBossDeath;
    }

    private void OnDisable()
    {
        WaveManager.OnTimeChanged -= BuildText;
        WaveManager.OnBossSpawned -= HandleBossSpawn;
        
        EnemyRuntime.OnBossDied -= HandleBossDeath;
    }
    
    private void BuildText(float seconds)
    {
        Debug.Log($"hello? {seconds}");
        var rounded = Mathf.RoundToInt(seconds);
        
        int minutes = rounded / 60;
        int second = rounded % 60;
        
        timeText.text = $"{minutes:D2}:{second:D2}";
    }

    private void HandleBossDeath()
    {
        _isBossAlive = false;
        timeText.colorGradient = new VertexGradient(Color.white);
        timeText.color = Color.white;
    }

    private void HandleBossSpawn(Boss boss)
    {
        _isBossAlive = true;
        SetGradient();
    }

    private void SetGradient()
    {
        // timeText.ForceMeshUpdate();
        //
        // var mesh = timeText.textInfo.meshInfo;
        // var colors = new VertexGradient(
        //     bossTextGradient.Evaluate(0f),   // top left
        //     bossTextGradient.Evaluate(1f),   // top right
        //     bossTextGradient.Evaluate(0f),   // bottom left
        //     bossTextGradient.Evaluate(1f)    // bottom right
        // );

        timeText.color = bossGradientBaseColor;
        timeText.colorGradient = bossTextGradient;
    }
}
