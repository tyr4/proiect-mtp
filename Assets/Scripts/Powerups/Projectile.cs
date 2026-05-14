using UnityEngine;

[System.Serializable]
public class Projectile : Powerup
{
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public Sprite ProjectileSprite { get; private set; }
    [field: SerializeField] public ProjectileType Type { get; private set; }
    
    [field: SerializeField] public bool AlwaysPierce { get; private set; }
    [field: SerializeField] public TierData PiercesLeft { get; private set; }
    [field: SerializeField] public TierData Cooldown { get; private set; }
    [field: SerializeField] public TierData Damage { get; private set; }
    [field: SerializeField] public TierData Speed { get; private set; }
    [field: SerializeField] public TierData Count { get; private set; }
    [field: SerializeField] public int CurrentTier { get; private set; }
    
    public virtual void Shoot(
        ProjectileRuntimeData projRuntimeData,
        Vector2 playerPos, 
        Vector2 nearestEnemyPos, 
        Vector2 nearestEnemyVelocity,
        ProjectileManager projManager, 
        SpatialGrid grid) { }
}
