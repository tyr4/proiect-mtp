using System.Collections.Generic;
using UnityEngine;

public class BowRuntime : MonoBehaviour, IProjectileBehaviour
{
    private Projectile _projectile;
    private ProjectileRuntimeData _projRuntimeData;
    
    public void Shoot(ProjectileRuntimeData data, List<GameObject> objects, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity)
    {
        var bow = (Bow)data.ownedPowerup.Base;
        var projectileSpeed = data.GetSpeed();
        int count = objects.Count;
        
        var travelTime = (nearestEnemyPos - playerPos).magnitude / projectileSpeed;
        var predictedPos = nearestEnemyPos + nearestEnemyVelocity * travelTime;
        var direction = (predictedPos - playerPos).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        float spread = bow.ArcSpreadDegrees;
        float step = count > 1 ? spread / (count - 1) : 0f;
        float startAngle = count > 1 ? -spread / 2f : 0f;

        for (int i = 0; i < count; i++)
        {
            var obj = objects[i];
            if (obj is null) continue;

            float angleOffset = startAngle + step * i;
            float finalAngle = baseAngle + angleOffset;
            var newDirection = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad), 
                Mathf.Sin(finalAngle * Mathf.Deg2Rad)
            );

            if (obj.TryGetComponent<BowRuntimeProjectile>(out var proj))
            {
                proj.Launch(data, newDirection * projectileSpeed, playerPos, finalAngle, bow);
            }
        }
    }
}
