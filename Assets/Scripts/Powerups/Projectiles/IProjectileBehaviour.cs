using UnityEngine;

public interface IProjectileBehaviour
{
    void Shoot(ProjectileRuntimeData data, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity);
}
