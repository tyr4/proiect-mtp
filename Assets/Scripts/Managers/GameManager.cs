using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Powerup _startPowerup;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public void LoadStartMenuScene()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void LoadMainLoopScene()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void SetStartPowerup(Powerup powerup)
    {
        _startPowerup = powerup;
    }

    public Powerup GetStartPowerup()
    {
        return _startPowerup;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
