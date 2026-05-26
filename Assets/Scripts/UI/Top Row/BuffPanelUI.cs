using System.Collections.Generic;
using UnityEngine;

public class BuffPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;

    private List<GameObject> _panels = new();

    private void OnEnable()
    {
        PowerupManager.OnPowerupAdded += UpdateCanvas;
    }

    private void OnDisable()
    {
        PowerupManager.OnPowerupAdded -= UpdateCanvas;
    }
    
    private void UpdateCanvas(OwnedPowerup powerup)
    {
        var panel = Instantiate(panelPrefab, transform);
        _panels.Add(panel);
        
        var container = panel.GetComponent<BuffPanelContainerUI>();
        container.BuildPanel(powerup);
    }
}
