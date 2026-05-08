using Unity.VisualScripting;
using UnityEngine;

public abstract class Projectile : Powerup, IProjectile
{
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public TierData Cooldown { get; private set; }
    [field: SerializeField] public TierData Damage { get; private set; }
    [field: SerializeField] public TierData Speed { get; private set; }
    [field: SerializeField] public TierData Count { get; private set; }

    public abstract void Tick(float deltaTime);
    public abstract void Shoot(Player player);
    
    
}
