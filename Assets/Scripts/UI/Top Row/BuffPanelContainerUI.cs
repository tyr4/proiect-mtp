using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffPanelContainerUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text tier;

    private OwnedPowerup _powerup;
    
    private void OnEnable()
    {
        PowerupManager.OnPowerupUpdated += UpdateTier;
    }

    private void OnDisable()
    {
        PowerupManager.OnPowerupUpdated -= UpdateTier;
    }
    
    public void BuildPanel(OwnedPowerup powerup)
    {
        _powerup = powerup;
        
        icon.sprite = powerup.Base.Icon;
        tier.text = $"{powerup.CurrentTier}";
    }
    
    private void UpdateTier(OwnedPowerup powerup)
    {
        if (_powerup != powerup) return;
        
        tier.text = $"{powerup.CurrentTier}";
    }
}
