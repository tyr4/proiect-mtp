using System.Collections.Generic;
using UnityEngine;

public class ProjectileRuntimeData
{
    public OwnedPowerup ownedPowerup;
    private Projectile _projectile;
    
    private List<GameObject> _objects = new();
    private IProjectileBehaviour _spawner;
    
    private float _cooldownTimer;
    
    public ProjectileRuntimeData(OwnedPowerup powerup, IProjectileBehaviour spawner)
    {
        ownedPowerup = powerup;
        _projectile = (Projectile)powerup.Base;
        _spawner = spawner;
        
        _cooldownTimer = 0f;
    }

    public Projectile GetProjectile() => _projectile;
    public float GetDamage() => _projectile.Damage.GetValue(ownedPowerup.CurrentTier);
    public float GetCooldown() => _projectile.Cooldown.GetValue(ownedPowerup.CurrentTier);
    public float GetSpeed() => _projectile.Speed.GetValue(ownedPowerup.CurrentTier);
    public float GetCount() => _projectile.Count.GetValue(ownedPowerup.CurrentTier);
    public float GetPiercesLeft() => _projectile.PiercesLeft.GetValue(ownedPowerup.CurrentTier);

    public void Tick(float dt, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity, ProjectileManager projManager)
    {
        _cooldownTimer += dt;

        if (_cooldownTimer >= GetCooldown())
        {
            _objects.Clear();
            int count = Mathf.RoundToInt(GetCount());
            
            for (int i = 0; i < count; i++)
            {
                _objects.Add(projManager.RequestPoolObject(_projectile));
            }
            
            _spawner.Shoot(this, _objects, playerPos, nearestEnemyPos, nearestEnemyVelocity);
            
            _cooldownTimer = 0f;
        }
    }
    
    public virtual void ReturnPoolObject(GameObject obj)
    {
        obj.SetActive(false);
        ProjectileManager.Instance.ReturnPoolObject(_projectile, obj);
    }

    public ProjectileHitState GenerateHitState(ProjectileHitState state)
    {
        if (state == null) return new ProjectileHitState(_projectile, this);
        
        state.Reset();
        return state;
    }
}
