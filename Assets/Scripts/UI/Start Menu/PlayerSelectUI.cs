using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectUI : MonoBehaviour
{
    [SerializeField] private CustomButton startButton;
    [SerializeField] private float animationDuration = 0.3f;

    private CanvasGroup _canvasGroup;

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
    }

    public void ShowMenu()
    {
        StartCoroutine(ShowMenuCoroutine());
    }

    private IEnumerator ShowMenuCoroutine()
    {
        yield return UIState.WaitUntilUnlocked();

        yield return Animations.LerpPanelAlpha(_canvasGroup, 0, 1, animationDuration).WaitForCompletion();
        
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        
        UIState.Unlock();
    }

    public void HideMenu()
    {
        Animations.LerpPanelAlpha(_canvasGroup, 1, 0, animationDuration);
        
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
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
    }
}
