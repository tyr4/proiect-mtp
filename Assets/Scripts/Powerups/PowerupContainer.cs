using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/Powerup Container")]
public class PowerupContainer : ScriptableObject
{
    public List<Powerup> Powerups;
}