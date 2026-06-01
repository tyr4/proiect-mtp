using UnityEngine;

[CreateAssetMenu(fileName = "Starting Player", menuName = "Player/Starting Player")]
public class StartingPlayerData : ScriptableObject
{
    public string displayName;
    public PlayerStats playerStats;

    public Sprite sprite;
    public RuntimeAnimatorController animationController;
    public Powerup powerup;
}
