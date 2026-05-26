using System;
using System.Text;
using TMPro;
using UnityEngine;

public class TimeElapsedUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    private void Awake()
    {
        BuildText(0);
    }
    
    private void OnEnable()
    {
        WaveManager.OnSecondIncrease += BuildText;
    }

    private void OnDisable()
    {
        WaveManager.OnSecondIncrease -= BuildText;
    }
    
    private void BuildText(float seconds)
    {
        var rounded = Mathf.RoundToInt(seconds);
        
        int minutes = rounded / 60;
        int second = rounded % 60;
        
        timeText.text = $"{minutes:D2}:{second:D2}";
    }
}
