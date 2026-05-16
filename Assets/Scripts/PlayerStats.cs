using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Player Stats", menuName = "Player/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public float MaxHealth;
    public float MovementSpeed;
}
