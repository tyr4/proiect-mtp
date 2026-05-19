using System.Collections.Generic;
using UnityEngine;

public class ProjectileRuntimeData : IProjectile
{
    public OwnedPowerup ownedPowerup;
    private float _cooldownTimer;
    private Projectile _projectile;
    private float _piercesLeft;
    private HashSet<int> _hitEnemies = new();

    public ProjectileRuntimeData(OwnedPowerup powerup)
    {
        ownedPowerup = powerup;
        _projectile = (Projectile)powerup.Base;
        _cooldownTimer = 0f;
        _piercesLeft = GetPiercesLeft();
    }

    public void Initialize()
    {
        ResetPierces();
    }
    
    public float GetDamage() => _projectile.Damage.GetValue(ownedPowerup.CurrentTier);
    public float GetCooldown() => _projectile.Cooldown.GetValue(ownedPowerup.CurrentTier);
    public float GetSpeed() => _projectile.Speed.GetValue(ownedPowerup.CurrentTier);
    public float GetCount() => _projectile.Count.GetValue(ownedPowerup.CurrentTier);
    public float GetPiercesLeft() => _projectile.PiercesLeft.GetValue(ownedPowerup.CurrentTier);

    public void Tick(float dt, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity, ProjectileManager projManager, SpatialGrid grid)
    {
        _cooldownTimer += dt;

        if (_cooldownTimer >= GetCooldown())
        {
            _projectile.Shoot(this, playerPos, nearestEnemyPos, nearestEnemyVelocity, projManager, grid);
            _cooldownTimer = 0f;
        }
    }

     private void DecrementPiercesLeft()
    {
        if (_projectile.AlwaysPierce) return;
        
        _piercesLeft -= 1;
    }

    public bool CanDealDamage()
    {
        return _piercesLeft >= 0 ||  _projectile.AlwaysPierce; 
    }

    private void ResetPierces()
    {
        _piercesLeft = GetPiercesLeft();
    }

    public void DealDamage(EnemyRuntime enemy)
    {
        int enemyID = enemy.GetInstanceID();
        if (_hitEnemies.Contains(enemyID)) return;

        var damage = GetDamage();
        
        enemy.TakeDamage(damage);
        _hitEnemies.Add(enemyID);

        DecrementPiercesLeft();
    }

    public virtual void ReturnPoolObject(GameObject obj)
    {
        _hitEnemies.Clear();
        obj.SetActive(false);
        ProjectileManager.Instance.ReturnPoolObject(_projectile, obj);
    }
}
