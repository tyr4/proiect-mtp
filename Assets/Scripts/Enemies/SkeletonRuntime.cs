using UnityEngine;

public class SkeletonRuntime : MonoBehaviour, IEnemyBehaviour
{
    private EnemyRuntime _runtime;
    private Enemy _enemy;

    private float _timer;
    // private float _cooldown = 3f;
    
    public void Initialize(EnemyRuntime data, Enemy enemy)
    {
        _runtime = data;
        _enemy = enemy;
    }

    public void Tick(float dt)
    {
        // _timer += dt;
        //
        // if (_timer >= _cooldown)
        // {
        //     Debug.Log("am intrat din enemy");
        //     _timer = 0;
        // }
    }
}
