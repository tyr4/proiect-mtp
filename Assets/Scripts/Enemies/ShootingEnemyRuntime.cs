using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ShootingEnemyRuntime : EnemyRuntime
{
    private ShootingEnemy _shootingEnemy;
    private IEnemyProjectileBehaviour _spawner;
    private List<GameObject> _objects = new();

    private float _timer;
    private bool _isShooting;
    private bool _storedAttack;
    
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public override void Initialize(Enemy enemy, float spawnTime, IEnemyProjectileBehaviour spawner = null)
    {
        base.Initialize(enemy, spawnTime, spawner);
        _shootingEnemy = (ShootingEnemy)enemy;
        _spawner = spawner;

        _timer = 0;
        _isShooting = false;
        EnableMovement();
    }
    
    public override void Tick(float dt, Transform playerPos)
    {
        base.Tick(dt, playerPos);
        
        if (_isShooting) return;
        
        _timer += dt;

        if (_timer >= _shootingEnemy.Cooldown || _storedAttack)
        {
            var distance = (playerPos.position - transform.position).sqrMagnitude;
            if (distance > _shootingEnemy.ShootingRange * _shootingEnemy.ShootingRange)
            {
                _storedAttack = true;
                _timer = 0;
                
                return;
            }
            
            _objects.Clear();

            for (int i = 0; i < _shootingEnemy.Count; i++)
            {
                var obj = ShootingEnemyManager.Instance.RequestPoolObject(_shootingEnemy);
                _objects.Add(obj);
            }

            StartCoroutine(ShootWithAnimation(playerPos));
            _storedAttack = false;
            _timer = 0;
        }
    }

    private IEnumerator ShootWithAnimation(Transform playerPos)
    {
        _isShooting = true;
        DisableMovement();
        
        animator.SetBool(IsWalking, false);
        yield return null;
        
        // wait for the shoot animation to complete
        yield return new WaitUntil(() => 
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log($"state: {info.fullPathHash}, normalizedTime: {info.normalizedTime}, isTransition: {_animator.IsInTransition(0)}");
            return animator is null ||
                   (info.normalizedTime >= 1f && !animator.IsInTransition(0));
        });

        if (Health <= 0)
        {
            Debug.Log("AM MURIT HELLO");
            yield break;
        }
        
        _spawner?.Shoot(_shootingEnemy, _objects, cachedTransform.position, playerPos);
        
        animator.SetBool(IsWalking, true);
        EnableMovement();
        _isShooting = false;
    }
}
