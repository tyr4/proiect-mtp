using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSkeletonRuntime : MonoBehaviour, IEnemyProjectileBehaviour
{
    private Transform _cachedTransform;
    
    private void Awake()
    {
        _cachedTransform = transform;
    }
    
    public void Shoot(ShootingEnemy data, EnemyRuntime runtime, List<GameObject> objects, Vector2 origin, Transform playerPos)
    {
        SpawnProjectiles(data, objects, origin, playerPos);
    }

    private void SpawnProjectiles(ShootingEnemy data, List<GameObject> objects, Vector2 origin, Transform playerPos)
    {
        int count = objects.Count;
        var speed = data.ProjSpeed;
        
        var playerDir = (playerPos.position - _cachedTransform.position).normalized;
        float baseAngle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
        float step = count > 1 ? data.ArcSpread / (count - 1) : 0f;
        float startAngle = count > 1 ? -data.ArcSpread / 2f : 0f;
        
        for (int i = 0; i < count; i++)
        {
            var obj = objects[i];
            if (obj is null) continue;

            float finalAngle = baseAngle + startAngle + step * i;
            var direction = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                Mathf.Sin(finalAngle * Mathf.Deg2Rad)
            );

            if (obj.TryGetComponent<BowSkeletonRuntimeProjectile>(out var proj))
            {
                proj.Launch(data, origin, direction * speed, finalAngle);
            }
        }
    }
}