using UnityEngine;
using UnityEngine.UI;

public class XPBarUI : MonoBehaviour
{
    [SerializeField] private Image xpBar;

    private void OnEnable()
    {
        Player.OnXPChanged += UpdateValue;
    }

    private void OnDisable()
    {
        Player.OnXPChanged -= UpdateValue;
    }
    
    private void UpdateValue(float current, float max)
    {
        xpBar.fillAmount = current / max;
    }
}
