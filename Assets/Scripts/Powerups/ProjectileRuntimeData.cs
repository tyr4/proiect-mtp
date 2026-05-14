using UnityEngine;

public class ProjectileRuntimeData : IProjectile
{
    public Projectile Projectile;
    private int _currentTier;
    private float _cooldownTimer;
    private float _piercesLeft;

    public ProjectileRuntimeData(Projectile projectile, int currentTier)
    {
        Projectile = projectile;
        _currentTier = currentTier;
        _cooldownTimer = 0f;
        _piercesLeft = GetPiercesLeft();
    }
    
    public float GetDamage() => Projectile.Damage.GetValue(_currentTier);
    public float GetCooldown() => Projectile.Cooldown.GetValue(_currentTier);
    public float GetSpeed() => Projectile.Speed.GetValue(_currentTier);
    public float GetCount() => Projectile.Count.GetValue(_currentTier);
    public float GetPiercesLeft() => Projectile.PiercesLeft.GetValue(_currentTier);

    public void UpgradeTier(int newTier)
    {
        if (newTier is < 1 or > 3) return;
        
        _currentTier = newTier;
    }

    public void Shoot(Vector2 playerPos, Vector2 nearestEnemyPos, ProjectileManager projManager, SpatialGrid grid)
    {
        Debug.Log("am intrat in shoot");
    }

    public void Tick(float dt, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity, ProjectileManager projManager, SpatialGrid grid)
    {
        _cooldownTimer += dt;

        if (_cooldownTimer >= GetCooldown())
        {
            Projectile.Shoot(this, playerPos, nearestEnemyPos, nearestEnemyVelocity, projManager, grid);
            _cooldownTimer = 0f;
        }
    }

    public void DecrementPiercesLeft()
    {
        if (Projectile.AlwaysPierce) return;
        
        _piercesLeft -= 1;
    }

    public bool CanPierce()
    {
        return _piercesLeft >= 0 ||  Projectile.AlwaysPierce; 
    }

    public void ResetPierces()
    {
        _piercesLeft = GetPiercesLeft();
    }
}
