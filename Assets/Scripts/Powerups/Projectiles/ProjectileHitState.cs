using System.Collections.Generic;

public class ProjectileHitState
{
    private Projectile _projectile;
    private ProjectileRuntimeData _runtimeData;
    
    private HashSet<int> _hitEnemies = new();
    private float _piercesLeft;

    public ProjectileHitState(Projectile proj, ProjectileRuntimeData runtimeData)
    {
        _projectile = proj;
        _runtimeData = runtimeData;
    }
    
    public void Reset()
    {
        _hitEnemies.Clear();
        ResetPierces();
    }
    
    public bool CanDealDamage()
    {
        return _piercesLeft >= 0 || _projectile.AlwaysPierce; 
    }

    public void DecrementPiercesLeft()
    {
        if (_projectile.AlwaysPierce) return;
        
        _piercesLeft -= 1;
    }
    
    private void ResetPierces()
    {
        _piercesLeft = _runtimeData.GetPiercesLeft();
    }
    
    public void DealDamage(EnemyRuntime enemy)
    {
        int enemyID = enemy.GetInstanceID();
        if (_hitEnemies.Contains(enemyID)) return;

        var damage = _runtimeData.GetDamage();
        
        enemy.TakeDamage(damage);
        _hitEnemies.Add(enemyID);

        DecrementPiercesLeft();
    }
}
