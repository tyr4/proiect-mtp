using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private float appearDuration;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
    
    private void OnEnable()
    {
        WaveManager.OnBossSpawned += ShowHPBar;
        EnemyRuntime.OnBossDied += HideHPBar;
        EnemyRuntime.OnBossHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        WaveManager.OnBossSpawned -= ShowHPBar;
        EnemyRuntime.OnBossDied -= HideHPBar;
        EnemyRuntime.OnBossHealthChanged -= UpdateHealthBar;
    }

    private void ShowHPBar(Boss _)
    {
        Animations.LerpPanelAlpha(_canvasGroup, 0, 1, appearDuration);
        _canvasGroup.blocksRaycasts = true;
    }

    private void HideHPBar()
    {
        Animations.LerpPanelAlpha(_canvasGroup, 1, 0, appearDuration);
        _canvasGroup.blocksRaycasts = false;
    }

    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        hpBar.fillAmount = currentHealth / maxHealth;
    }
}
