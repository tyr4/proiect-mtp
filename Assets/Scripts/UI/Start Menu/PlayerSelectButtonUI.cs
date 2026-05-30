using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectButtonUI : MonoBehaviour
{
    [SerializeField] private Powerup startPowerup;

    public void OnClick()
    {
        GameManager.Instance.SetStartPowerup(startPowerup);
    }
}
