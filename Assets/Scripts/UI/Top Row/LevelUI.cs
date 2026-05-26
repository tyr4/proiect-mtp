using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;

    private void OnEnable()
    {
        Player.OnLevelUp += UpdateValue;
    }

    private void OnDisable()
    {
        Player.OnLevelUp -= UpdateValue;
    }

    private void UpdateValue(int level)
    {
        levelText.text = $"Lv. {level}";
    }
}
