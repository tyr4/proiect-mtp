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
    
    [Header("Animation Fade variables")]
    [SerializeField] private float timescaleDuration;
    [SerializeField] private float panelDuration;
    [SerializeField] private float musicDuration;
    
    private List<Powerup> _allPowerups;
    private List<OwnedPowerup> _choices = new();
    private CanvasGroup _selfGroup;
    
    private AudioSource _source;
    private Tween _musicFadeTween;
    
    private void Start()
    {
        _selfGroup = GetComponent<CanvasGroup>();
        _allPowerups = PowerupManager.Instance.GetAllPowerups();
        _source = AudioManager.Instance.MusicSource;
        _source.Pause();

        _selfGroup.alpha = 0;
        _selfGroup.interactable = false;
        _selfGroup.blocksRaycasts = false;
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
        
        AudioEvents.RequestSFX(AudioManager.Sounds.playerLevelUp);
    }

    private IEnumerator PopupPanelCoroutine()
    {
        yield return UIState.WaitUntilUnlocked(); 
     
        _choices = PowerupManager.Instance.GeneratePowerupChoices();
        
        SetPanelData(_choices[0], panel1Container);
        SetPanelData(_choices[1], panel2Container);
        SetPanelData(_choices[2], panel3Container);

        FadeMusicOut();
        yield return Animations.LerpTimescale(1, 0, timescaleDuration).WaitForCompletion();
        
        _selfGroup.interactable = false;
        _selfGroup.blocksRaycasts = false;
        
        yield return Animations.LerpPanelAlpha(_selfGroup, 0, 1, panelDuration).WaitForCompletion();
        
        _selfGroup.interactable = true;
        _selfGroup.blocksRaycasts = true;
    }

    private void SetPanelData(OwnedPowerup powerup, PowerupPanelContainerUI panel)
    {
        panel.Populate(powerup);
    }

    public IEnumerator ClosePanel()
    {
        
        _selfGroup.interactable = false;
        _selfGroup.blocksRaycasts = false;
        
        FadeMusicIn();
        yield return Animations.LerpPanelAlpha(_selfGroup, 1, 0, panelDuration).WaitForCompletion();
        yield return Animations.LerpTimescale(0, 1, timescaleDuration);
        
        UIState.Unlock();
    }

    private void FadeMusicIn()
    {
        _musicFadeTween?.Kill();
        
        _source.UnPause();
        _musicFadeTween = Animations.LerpAudioSourceVolume(_source, _source.volume, 1, musicDuration);
    }

    private void FadeMusicOut()
    {
        _musicFadeTween?.Kill();

        _musicFadeTween = Animations.LerpAudioSourceVolume(_source, _source.volume, 0, musicDuration)
            .OnComplete(_source.Pause);
    }
}
