using UnityEngine;

[CreateAssetMenu(fileName = "Bow", menuName = "Powerups/Projectiles/Bow")]
public class Bow : Projectile
{
    public override void Shoot(Vector2 playerPos, Vector2 nearestEnemyPos, ProjectileManager projManager, SpatialGrid grid)
    {
        var obj = projManager.RequestPoolObject(this);
        var tier = this.CurrentTier;
        var direction = (nearestEnemyPos - playerPos).normalized;
        var velocity = direction * Speed.GetValue(tier);
        
        obj.transform.position = playerPos;
        obj.Launch(this, velocity, this.Damage.GetValue(tier), 5f);
    }
}
