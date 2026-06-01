using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectButtonUI : MonoBehaviour
{
    [SerializeField] private StartingPlayerData startingData;

    public static event Action<StartingPlayerData> OnPlayerSelected;
    
    public void OnClick()
    {
        GameManager.Instance.SetStartData(startingData);
        OnPlayerSelected?.Invoke(startingData);
    }
}
