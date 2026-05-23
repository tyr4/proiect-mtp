using System.Collections.Generic;
using UnityEngine;

public interface IProjectileBehaviour
{
    void Shoot(ProjectileRuntimeData data, List<GameObject> objects, Vector2 playerPos, Vector2 nearestEnemyPos,
        Vector2 nearestEnemyVelocity);
}
