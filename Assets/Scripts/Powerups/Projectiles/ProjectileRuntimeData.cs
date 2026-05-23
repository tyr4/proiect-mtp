using System.Collections.Generic;
using UnityEngine;

public class ProjectileRuntimeData
{
    public OwnedPowerup ownedPowerup;
    private Projectile _projectile;
    
    private float _piercesLeft;
    private HashSet<int> _hitEnemies = new();
    
    private List<GameObject> _objects = new();
    private IProjectileBehaviour _spawner;
    
    private float _cooldownTimer;
    
    public ProjectileRuntimeData(OwnedPowerup powerup, IProjectileBehaviour spawner)
    {
        ownedPowerup = powerup;
        _projectile = (Projectile)powerup.Base;
        _spawner = spawner;
        
        _cooldownTimer = 0f;
        _piercesLeft = GetPiercesLeft();
    }
    
    public ProjectileRuntimeData(ProjectileRuntimeData source)
    {
        ownedPowerup = source.ownedPowerup;
        _projectile = source._projectile;
        _spawner = source._spawner;
        _cooldownTimer = 0f;
        _piercesLeft = GetPiercesLeft();
        _hitEnemies = new HashSet<int>();
    }

    public void Initialize()
    {
        ResetPierces();
    }

    public ProjectileRuntimeData Clone() => new ProjectileRuntimeData(this);
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
