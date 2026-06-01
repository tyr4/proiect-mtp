using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSelectUI : MonoBehaviour
{
    [SerializeField] private CustomButton startButton;
    
    [SerializeField] private float animationDuration = 0.3f;

    private CanvasGroup _canvasGroup;
    private bool _isOpen;

    public static event Action OnStartButtonDisabled;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void Start()
    {
        startButton.DisableButton();
        OnStartButtonDisabled?.Invoke();

        startButton.AddEventListeners(_ => GameManager.Instance.LoadMainLoopScene());
    }
    
    public void OnClick()
    {
        if (!_isOpen)
            StartCoroutine(ShowMenuCoroutine());
        else HideMenu();
    }

    private IEnumerator ShowMenuCoroutine()
    {
        yield return UIState.WaitUntilUnlocked();

        yield return Animations.LerpPanelAlpha(_canvasGroup, 0, 1, animationDuration).WaitForCompletion();
        
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        
        UIState.Unlock();
        _isOpen = true;
    }

    public void HideMenu()
    {
        Animations.LerpPanelAlpha(_canvasGroup, 1, 0, animationDuration);
        
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _isOpen = false;
    }

    public void OnPowerupSelected(CustomButton toggle)
    {
        var state = toggle.GetButtonState();

        Debug.Log($"TOGGLE BUTTON STATE {state}");
        if (state == CustomButton.ButtonState.Selected)
        {
            startButton.EnableButton();
            return;
        }
        
        startButton.DisableButton();
        OnStartButtonDisabled?.Invoke();
    }
}
