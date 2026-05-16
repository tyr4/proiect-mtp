using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image hpBarImage;

    private void OnEnable()
    {
        Player.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        Player.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float current, float max)
    {
        hpBarImage.fillAmount = current / max;
    } 
}