using TMPro;
using UnityEngine;

public class KillsCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textCounter;
    
    private int _counter;

    private void Awake()
    {
        textCounter.text = "0";
    }
    
    private void OnEnable()
    {
        EnemyManager.OnEnemyDied += BuildText;
    }

    private void OnDisable()
    {
        EnemyManager.OnEnemyDied -= BuildText;
    }
    
    private void BuildText(Transform _)
    {
        _counter++;
        
        textCounter.text = _counter.ToString();
    }
}