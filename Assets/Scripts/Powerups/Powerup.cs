using System;
using UnityEngine;

public abstract class Powerup : ScriptableObject
{
    public enum PowerupTypeEnum
    {
        Projectile,
        Passive,
        OneTimeBuff
    }
    
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public PowerupTypeEnum PowerupType { get; private set; }
    
    public virtual void OnAssign() {}
    public virtual void OnSelect() {}

    public virtual string GetDescription()
    {
        return Description;
    }
}
