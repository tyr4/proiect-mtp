using Unity.VisualScripting;
using UnityEngine;

public abstract class Projectile : Powerup
{
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public ProjectileType Type { get; private set; }
    [field: SerializeField] public TierData Cooldown { get; private set; }
    [field: SerializeField] public TierData Damage { get; private set; }
    [field: SerializeField] public TierData Speed { get; private set; }
    [field: SerializeField] public TierData Count { get; private set; }
    [field: SerializeField] public int CurrentTier { get; private set; }
}
