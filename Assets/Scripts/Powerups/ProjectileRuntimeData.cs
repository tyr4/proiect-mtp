using UnityEngine;

public class ProjectileRuntimeData : IProjectile
{
    public OwnedPowerup ownedPowerup;
    private float _cooldownTimer;
    private Projectile _projectile;
    private float _piercesLeft;

    public ProjectileRuntimeData(OwnedPowerup powerup)
    {
        ownedPowerup = powerup;
        _projectile = (Projectile)powerup.Base;
        _cooldownTimer = 0f;
        _piercesLeft = GetPiercesLeft();
    }
    
    public float GetDamage() => _projectile.Damage.GetValue(ownedPowerup.CurrentTier);
    public float GetCooldown() => _projectile.Cooldown.GetValue(ownedPowerup.CurrentTier);
    public float GetSpeed() => _projectile.Speed.GetValue(ownedPowerup.CurrentTier);
    public float GetCount() => _projectile.Count.GetValue(ownedPowerup.CurrentTier);
    public float GetPiercesLeft() => _projectile.PiercesLeft.GetValue(ownedPowerup.CurrentTier);

    public void Shoot(Vector2 playerPos, Vector2 nearestEnemyPos, ProjectileManager projManager, SpatialGrid grid)
    {
        Debug.Log("am intrat in shoot");
    }

    public void Tick(float dt, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity, ProjectileManager projManager, SpatialGrid grid)
    {
        _cooldownTimer += dt;

        if (_cooldownTimer >= GetCooldown())
        {
            _projectile.Shoot(this, playerPos, nearestEnemyPos, nearestEnemyVelocity, projManager, grid);
            _cooldownTimer = 0f;
        }
    }

    public void DecrementPiercesLeft()
    {
        if (_projectile.AlwaysPierce) return;
        
        _piercesLeft -= 1;
    }

    public bool CanPierce()
    {
        return _piercesLeft >= 0 ||  _projectile.AlwaysPierce; 
    }

    public void ResetPierces()
    {
        _piercesLeft = GetPiercesLeft();
    }
}
