using TMPro;
using UnityEngine;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private CustomButton quitButton;
    
    private CanvasGroup _canvasGroup;

    private void Start()
    {
        quitButton.AddEventListeners(_ => GameManager.Instance.Quit());
        AudioEvents.RequestMusic(AudioManager.Sounds.mainMenu);
    }
}
