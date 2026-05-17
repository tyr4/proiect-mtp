using System;
using System.Text;
using TMPro;
using UnityEngine;

public class TimeElapsedUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    private int _totalSeconds = 0;
    private float _timer;
    // private StringBuilder _newText = new StringBuilder();

    private void Awake()
    {
        BuildText(_totalSeconds);
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        _timer += dt;

        if (_timer >= 1f)
        {
            _totalSeconds++;
            _timer -= 1f;
            
            BuildText(_totalSeconds);
        }
    }
    
    private void BuildText(int seconds)
    {
        int minutes = seconds / 60;
        int second = seconds % 60;
        
        timeText.text = $"{minutes:D2}:{second:D2}";
    }
}
