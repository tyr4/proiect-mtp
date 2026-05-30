using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private CustomButton pauseMenuButton;
    
    private float _timescaleDuration = 0.8f;
    private float _panelDuration = 0.3f;
    private CanvasGroup _canvasGroup;
    private Coroutine _activeCoroutine;

    private void Awake()
    {
        UIState.Unlock();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    public void OnClick()
    {
        if (_activeCoroutine != null) return;
        
        _activeCoroutine = StartCoroutine(ShowMenu());
    }
    
    private IEnumerator ShowMenu()
    {
        yield return UIState.WaitUntilUnlocked();
        
        pauseMenuButton.DisableButton();
        
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        
        Animations.LerpTimescale(1, 0, _timescaleDuration);
        Animations.LerpPanelAlpha(_canvasGroup, 0, 1, _panelDuration);
        
        _activeCoroutine = null;
    }

    private IEnumerator HideMenu()
    {
        pauseMenuButton.EnableButton();
        
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        yield return Animations.LerpTimescale(0, 1, _timescaleDuration);
        yield return Animations.LerpPanelAlpha(_canvasGroup, 1, 0, _panelDuration);
        
        UIState.Unlock();
    }

    public void Resume()
    {
        StartCoroutine(HideMenu());
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);    
    }

    public void Leave()
    {
        GameManager.Instance.LoadStartMenuScene();
    }
}
