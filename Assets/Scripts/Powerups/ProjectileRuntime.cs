using UnityEngine;

public class ProjectileRuntime : IProjectile
{
    private Projectile _projectile;
    private int _currentTier;
    private float _cooldownTimer;

    public ProjectileRuntime(Projectile projectile, int currentTier)
    {
        _projectile = projectile;
        _currentTier = currentTier;
        _cooldownTimer = 0f;
    }
    
    public float GetDamage() => _projectile.Damage.GetValue(_currentTier);
    public float GetCooldown() => _projectile.Cooldown.GetValue(_currentTier);
    public float GetSpeed() => _projectile.Speed.GetValue(_currentTier);
    public float GetCount() => _projectile.Count.GetValue(_currentTier);

    public void UpgradeTier(int newTier)
    {
        if (newTier is < 1 or > 3) return;
        
        _currentTier = newTier;
    }

    public void Shoot(Vector2 playerPos, Vector2 nearestEnemyPos, ProjectileManager projManager, SpatialGrid grid)
    {

    }

    public void Tick(float dt, Vector2 playerPos, Vector2 nearestEnemyPos, ProjectileManager projManager, SpatialGrid grid)
    {
        _cooldownTimer += dt;

        if (_cooldownTimer >= GetCooldown())
        {
            Shoot(playerPos, nearestEnemyPos, projManager, grid);
            _cooldownTimer = 0f;
        }
    }
}
