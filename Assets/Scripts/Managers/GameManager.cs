using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StartingPlayerData dataIfNull;
    
    public static GameManager Instance;
    
    private StartingPlayerData _startingData;
    
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadStartMenuScene()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }

    public void LoadMainLoopScene()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIState.Unlock();
    }

    public void SetStartData(StartingPlayerData powerup)
    {
        _startingData = powerup;
    }

    public StartingPlayerData GetStartingData()
    {
        if (_startingData == null) return dataIfNull;
        
        return _startingData;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetPlayerData(Player player)
    {
        if (_startingData == null)
        {
            player.SetAnimationController(dataIfNull.animationController);
            PowerupManager.Instance.UpdatePlayerPowerups(dataIfNull.powerup);
            return;
        }
        
        player.SetAnimationController(_startingData.animationController);
        PowerupManager.Instance.UpdatePlayerPowerups(_startingData.powerup);
    }
}
