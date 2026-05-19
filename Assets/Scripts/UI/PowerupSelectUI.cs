using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PowerupSelectUI : MonoBehaviour
{
    [Header("Panel CanvasGroups")]
    [SerializeField] private CanvasGroup panel1Group;
    [SerializeField] private CanvasGroup panel2Group;
    [SerializeField] private CanvasGroup panel3Group;
    
    [Header("Panel Containers")]
    [SerializeField] private PowerupPanelContainerUI panel1Container;
    [SerializeField] private PowerupPanelContainerUI panel2Container;
    [SerializeField] private PowerupPanelContainerUI panel3Container;
    
    [Header("Animation variables")]
    [SerializeField] private float timescaleDuration;
    [SerializeField] private float panelDuration;
    
    private List<Powerup> _allPowerups;
    private List<OwnedPowerup> _choices = new();
    private CanvasGroup _selfGroup;
    
    private void Start()
    {
        _selfGroup = GetComponent<CanvasGroup>();
        _allPowerups = PowerupManager.Instance.GetAllPowerups();
        _selfGroup.alpha = 0;
    }
    
    private void OnEnable()
    {
        Player.OnLevelUp += PopupPanel;
    }

    private void OnDisable()
    {
        Player.OnLevelUp -= PopupPanel;
    }

    private void PopupPanel(int _)
    {
        StartCoroutine(PopupPanelCoroutine());
    }

    private IEnumerator PopupPanelCoroutine()
    {
        _choices = PowerupManager.Instance.GeneratePowerupChoices();
        
        SetPanelData(_choices[0], panel1Container);
        SetPanelData(_choices[1], panel2Container);
        SetPanelData(_choices[2], panel3Container);

        yield return LerpTimescale(1, 0).WaitForCompletion();
        
        LerpPanelAlpha(_selfGroup, 0, 1);
    }

    private void SetPanelData(OwnedPowerup powerup, PowerupPanelContainerUI panel)
    {
        panel.Populate(powerup);
    }

    public IEnumerator ClosePanel()
    {
        LerpPanelAlpha(_selfGroup, 1, 0);
        
        yield return LerpTimescale(0, 1).WaitForCompletion();
    }

    private Tween LerpValue(DOGetter<float> getter, DOSetter<float> setter, float startValue, float endValue, float endDuration)
    {
        setter(startValue);
        
        return DOTween.To(
            getter,
            setter,
            endValue,
            endDuration
        ).SetUpdate(true);
    }

    private Tween LerpTimescale(int start, int end)
    {
        return LerpValue(
            () => Time.timeScale, 
            x => Time.timeScale = x,
            start, end, timescaleDuration);
    }

    private Tween LerpPanelAlpha(CanvasGroup panel, int start, int end)
    {
        return LerpValue(
            () => panel.alpha, 
            x => panel.alpha = x,
            start, end, panelDuration);
    }
}
