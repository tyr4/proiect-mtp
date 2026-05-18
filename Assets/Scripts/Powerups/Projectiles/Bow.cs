using UnityEngine;

[CreateAssetMenu(fileName = "Bow", menuName = "Powerups/Projectiles/Bow")]
public class Bow : Projectile
{
    [field: SerializeField] public Vector2 PositionOffset { get; private set; }
    [field: SerializeField] public float AngleOffset { get; private set; }
    
    public override void Shoot(ProjectileRuntimeData projRuntimeData, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity, ProjectileManager projManager, SpatialGrid grid)
    {
        var tier = projRuntimeData.ownedPowerup.CurrentTier;
        var projectileSpeed = Speed.GetValue(tier);

        var travelTime = (nearestEnemyPos - playerPos).magnitude / projectileSpeed;
        var predictedPos = nearestEnemyPos + nearestEnemyVelocity * travelTime;
        
        var obj = projManager.RequestPoolObject(this);
        var objTransform = obj.transform;
        
        var direction = (predictedPos - playerPos).normalized;
        var velocity = direction * projectileSpeed;
        
        objTransform.position = playerPos + PositionOffset;
        objTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + AngleOffset);
        obj.Launch(projRuntimeData, velocity, this.Damage.GetValue(tier), 5f);
    }
}
