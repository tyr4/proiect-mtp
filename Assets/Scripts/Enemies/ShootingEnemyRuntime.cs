using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyRuntime : MonoBehaviour
{
    private EnemyRuntime _runtime;
    private ShootingEnemy _shootingEnemy;
    private IEnemyProjectileBehaviour _spawner;
    private List<GameObject> _objects = new();

    private Transform _cachedTransform;
    private float _timer;
    private bool _isShooting;

    private Animator _animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    
    private void Awake()
    {
        _cachedTransform = transform;
        _animator = GetComponentInChildren<Animator>();
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Initialize(EnemyRuntime runtime, ShootingEnemy enemy, IEnemyProjectileBehaviour spawner)
    {
        _runtime = runtime;
        _shootingEnemy = enemy;
        _spawner = spawner;

        _timer = 0;
        _isShooting = false;
        _runtime.EnableMovement();
    }
    
    public void Tick(float dt, Transform playerPos)
    {
        if (_isShooting) return;
        
        _timer += dt;

        if (_timer >= _shootingEnemy.Cooldown)
        {
            _objects.Clear();

            for (int i = 0; i < _shootingEnemy.Count; i++)
            {
                var obj = ShootingEnemyManager.Instance.RequestPoolObject(_shootingEnemy);
                _objects.Add(obj);
            }

            StartCoroutine(ShootWithAnimation(playerPos));
            _timer = 0;
        }
    }

    private IEnumerator ShootWithAnimation(Transform playerPos)
    {
        _isShooting = true;
        _runtime.DisableMovement();
        
        _animator.SetBool(IsWalking, false);
        yield return null;
        
        // wait for the shoot animation to complete
        yield return new WaitUntil(() => 
        {
            var info = _animator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log($"state: {info.fullPathHash}, normalizedTime: {info.normalizedTime}, isTransition: {_animator.IsInTransition(0)}");
            return _animator is null ||
                   (info.normalizedTime >= 1f && !_animator.IsInTransition(0));
        });

        _spawner?.Shoot(_shootingEnemy, _runtime, _objects, _cachedTransform.position, playerPos);
        
        _animator.SetBool(IsWalking, true);
        _runtime.EnableMovement();
        _isShooting = false;
    }
}
