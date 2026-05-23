using System;
using UnityEngine;

public abstract class Powerup : ScriptableObject
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    
    public virtual void OnAssign() {}
    public virtual void OnSelect(OwnedPowerup owned) {}

    public virtual string GetDescription()
    {
        return Description;
    }
}
