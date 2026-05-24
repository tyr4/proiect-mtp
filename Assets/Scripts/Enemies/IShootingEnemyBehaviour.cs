using UnityEngine;

public interface IShootingEnemyBehaviour
{
    void Initialize(EnemyRuntime runtime, ShootingEnemy enemy, IEnemyProjectileBehaviour spawner);
    void Tick(float dt, Vector2 direction);
}
