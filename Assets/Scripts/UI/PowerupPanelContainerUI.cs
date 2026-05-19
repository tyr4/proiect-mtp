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
    [SerializeField] private Button confirmButton;
    
    private Powerup _selectedPowerup;
    private PowerupSelectUI _parentPanel;

    public static event Action<Powerup> OnPowerupSelected;

    private void Awake()
    {
        _parentPanel = GetComponentInParent<PowerupSelectUI>();
    }

    private void Initialzie(Powerup selectedPowerup)
    {
        _selectedPowerup = selectedPowerup;
    }
    
    public void Populate(Powerup powerup, int tier)
    {
        Initialzie(powerup);
        
        icon.sprite = powerup.Icon;
        displayName.text = powerup.DisplayName;
        description.text = powerup.GetDescription();

        displayName.text += $" (Tier {tier + 1})";
    }

    public void OnClick()
    {
        StartCoroutine(_parentPanel.ClosePanel());
        
        Debug.Log($"am intrat cu {_selectedPowerup}");
        OnPowerupSelected?.Invoke(_selectedPowerup);
    }
}
