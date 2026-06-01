using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text movementSpeedText;
    [SerializeField] private TMP_Text startingPowerupText;
    [SerializeField] private TMP_Text startingPowerupDescriptionText;
    
    [SerializeField] private Image powerupImage;
    
    private void OnEnable()
    {
        PlayerSelectButtonUI.OnPlayerSelected += BuildText;
        PlayerSelectUI.OnStartButtonDisabled += EmptyText;
    }

    private void OnDisable()
    {
        PlayerSelectButtonUI.OnPlayerSelected -= BuildText;
        PlayerSelectUI.OnStartButtonDisabled -= EmptyText;
    }

    private void BuildText(StartingPlayerData data)
    {
        var stats = data.playerStats;

        playerNameText.text = $"Name: {data.displayName}";
        healthText.text = $"Heath: {stats.maxHealth}";
        movementSpeedText.text = $"Movement Speed: {stats.movementSpeed}";
        startingPowerupText.text = $"Starting Powerup: {data.powerup.name}";
        startingPowerupDescriptionText.text = $"Description: {data.powerup.Description}";

        powerupImage.sprite = data.powerup.Icon;
        powerupImage.enabled = true;
    }

    private void EmptyText()
    {
        playerNameText.text = string.Empty;
        healthText.text = string.Empty;
        movementSpeedText.text = string.Empty;
        startingPowerupText.text = string.Empty;
        startingPowerupDescriptionText.text = string.Empty;
        
        powerupImage.enabled = false;
    }
}
