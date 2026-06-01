using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerStats
{
    public float maxHealth;
    public float movementSpeed;

    public PlayerStats() { }

    public PlayerStats Clone()
    {
        return new PlayerStats
        {
            maxHealth = this.maxHealth,
            movementSpeed = this.movementSpeed
        };
    }
}
