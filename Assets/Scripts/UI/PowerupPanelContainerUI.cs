using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupPanelContainerUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private TMP_Text description;
    
    private Powerup _selectedPowerup;
    private PowerupSelectUI _parentPanel;

    public static event Action<Powerup> OnPowerupSelected;

    private void Awake()
    {
        _parentPanel = GetComponentInParent<PowerupSelectUI>();
    }

    private void Initialize(Powerup selectedPowerup)
    {
        _selectedPowerup = selectedPowerup;
    }
    
    public void Populate(OwnedPowerup powerup)
    {
        Initialize(powerup.Base);
        
        icon.sprite = powerup.Base.Icon;
        displayName.text = powerup.Base.DisplayName;
        description.text = powerup.Base.GetDescription();

        displayName.text += $" (Tier {powerup.CurrentTier + 1})";
    }

    public void OnClick()
    {
        StartCoroutine(_parentPanel.ClosePanel());
        
        Debug.Log($"am intrat cu {_selectedPowerup}");
        OnPowerupSelected?.Invoke(_selectedPowerup);
    }
}
