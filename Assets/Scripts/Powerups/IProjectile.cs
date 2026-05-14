using UnityEngine;

public interface IProjectile
{
    public void Shoot(Vector2 playerPos, Vector2 nearestEnemyPos, ProjectileManager projManager, SpatialGrid grid);
    public void Tick(float dt, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity, ProjectileManager projManager, SpatialGrid grid);
}
