using System.Collections.Generic;
using UnityEngine;

public interface IEnemyProjectileBehaviour
{
    void Shoot(ShootingEnemy data, EnemyRuntime runtime, List<GameObject> objects, Vector2 origin, Transform playerPos);
}
