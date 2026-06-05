using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyRuntime : EnemyRuntime
{
    private ShootingEnemy _shootingEnemy;
    private IEnemyProjectileBehaviour _spawner;
    protected List<GameObject> Objects = new();

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

        if (_spawner == null  || _isShooting) return;
        
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

            Objects = RequestObjects(_shootingEnemy.Count);
                
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
        
        _spawner?.Shoot(_shootingEnemy, Objects, cachedTransform.position, playerPos);
        
        animator.SetBool(IsWalking, true);
        EnableMovement();
        _isShooting = false;
    }

    protected List<GameObject> RequestObjects(int count)
    {
        Objects.Clear();

        Debug.Log($"REQUESTING {count} OBJECTS PENTRU {_shootingEnemy}");
        for (int i = 0; i < count; i++)
        {
            var obj = ShootingEnemyManager.Instance.RequestPoolObject(_shootingEnemy);
            Debug.Log($"AM PRIMIT {obj} PENTRU {_shootingEnemy}");
            Objects.Add(obj);
        }

        return Objects;
    }

    // protected void ReturnObjects(ShootingEnemy enemy, List<GameObject> objects)
    // {
    //     foreach (var obj in objects)
    //     {
    //         ShootingEnemyManager.Instance.ReturnPoolObject(enemy, obj);
    //     }
    // }
}
