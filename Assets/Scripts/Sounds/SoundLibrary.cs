using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Sounds/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("UI")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;

    [Header("Player")]
    public AudioClip playerHurt;
    public AudioClip playerDeath;
    public AudioClip playerLevelUp;

    [Header("Music")]
    public AudioClip mainMenu;
    public AudioClip gameplay;

    [Header("Enemies")] 
    public AudioClip enemyDead;
    
    [Header("Others")] 
    public AudioClip xpPickup;
}