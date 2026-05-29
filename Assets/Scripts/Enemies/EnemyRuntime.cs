using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyRuntime : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    
    public Enemy Data { get; private set; }
    public Transform cachedTransform;
    
    private IEnemyBehaviour _behaviour;
    private ShootingEnemyRuntime _shootingRuntime;
    
    private float _health;
    public float Damage;
    private float _movementSpeed;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Material _defaultMaterial;
    private Animator _animator;
    
    // dont update the direction every fixedupdate call
    private const float PathfindingRefreshRate = 0.2f;
    private float _timer;
    
    private Vector3 _direction;
    private Vector3 _finalDirection;

    private const float DespawnDistance = 8f;
    private float _despawnDistanceSquared;
    private float _distance;

    private bool _isDead;
    private bool _canMove = true;
    
    public Vector2 Velocity => _finalDirection * _movementSpeed;
    
    private static readonly int HasDied = Animator.StringToHash("hasDied");
    public static event Action<Transform, float> OnDamageTaken;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        
        _behaviour = GetComponent<IEnemyBehaviour>();
        _shootingRuntime = GetComponent<ShootingEnemyRuntime>();
        
        _defaultMaterial = _sr.material;
        cachedTransform = transform;

        _despawnDistanceSquared = DespawnDistance * DespawnDistance;
    }
    
    public void Initialize(Enemy enemy, float spawnTime, IEnemyProjectileBehaviour spawner = null)
    {
        Data = enemy;
        _health = Data.Health * GetHealthScalingFactor(spawnTime);
        Damage = Data.Damage * GetDamageScalingFactor(spawnTime);
        _movementSpeed = Data.MovementSpeed;
        
        _isDead = false;
        _rb.simulated = true;

        _distance = 0;
        EnableMovement();
        
        // apply a random speed multiplier
        _movementSpeed *= Random.Range(0.8f, 1.1f);

        _behaviour?.Initialize(this, enemy);

        if (enemy is ShootingEnemy shootingEnemy)
        {
            _shootingRuntime?.Initialize(this, shootingEnemy, spawner);
        }
    }

    private float GetHealthScalingFactor(float spawnTime)
    {
        return Mathf.Max(1, spawnTime / 300);
    }

    private float GetDamageScalingFactor(float spawnTime)
    {
        return Mathf.Max(1, spawnTime / 300);
    }
    
    public void Tick(float deltaTime, Transform playerTransform)
    {
        // basic pathfinding global for all enemies
        _timer += deltaTime;

        if (_timer >= PathfindingRefreshRate)
        {
            if (_canMove)
            {
                _direction = playerTransform.position - cachedTransform.position;
                _finalDirection = _direction.normalized;

                _distance = _direction.sqrMagnitude;
                DespawnIfOutOfRange();
            }
            else
            {
                _finalDirection = Vector3.zero;
            }
            
            _timer = 0;
        }
        
        _rb.linearVelocity = _finalDirection * _movementSpeed;

        if (_direction.x != 0)
        {
            cachedTransform.localScale = new Vector3(_direction.x < 0 ? -1f : 1f, 1f, 1f);
        }
        
        // execute custom logic
        _behaviour?.Tick(deltaTime);
        _shootingRuntime?.Tick(deltaTime, playerTransform);
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        
        _health -= damage;
        OnDamageTaken?.Invoke(cachedTransform, damage);

        if (_health <= 0)
        {
            Kill(true);
            return;
        }

        TakeDamageAnimation();
    }

    private void Kill(bool spawnXP)
    {
        if (_isDead) return;
        _isDead = true;
        DisableMovement();
        
        EnemyManager.Instance.Unregister(this);
        
        StartCoroutine(DieAnimation(spawnXP));
    }

    private void TakeDamageAnimation()
    {
        _sr.DOKill();
        
        _sr.material = flashMaterial;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (!_isDead)
            {
                _sr.material = _defaultMaterial;
            }
        }).SetLink(gameObject);
    }

    private IEnumerator DieAnimation(bool spawnXP)
    {
        _sr.material = _defaultMaterial;
        _rb.simulated = false;
        
        _animator.SetTrigger(HasDied);
        yield return null;
        
        // wait for the death animation to complete
        yield return new WaitUntil(() => 
        {
            var info = _animator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log($"state: {info.fullPathHash}, normalizedTime: {info.normalizedTime}, isTransition: {_animator.IsInTransition(0)}");
            return _animator is null ||
                   (info.normalizedTime >= 1f && !_animator.IsInTransition(0));
        });

        _sr.DOKill();
        _sr.DOFade(0f, 0.15f)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                ResetVisual();
                WaveManager.Instance.ReturnToPool(Data, this);
                if (spawnXP) XPManager.Instance.SpawnXP(cachedTransform.position);
            });
        
        // WaveManager.Instance.ReturnToPool(Data, this);
        // _sr.DOFade(0f, 0.2f).OnComplete(() => WaveManager.Instance.ReturnToPool(Data, this));
    }

    private void ResetVisual()
    {
        var c = _sr.color;
        c.a = 1f;
        _sr.color = c;

        _sr.material = _defaultMaterial;
    }

    public void DisableMovement()
    {
        _canMove = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void EnableMovement()
    {
        _canMove = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void DespawnIfOutOfRange()
    {
        if (_distance < _despawnDistanceSquared) return;

        Kill(false);
    }
    
    private void OnDrawGizmos()
    {
        float cellSize = 2f;

        Vector3 pos = transform.position;

        int x = Mathf.FloorToInt(pos.x / cellSize);
        int y = Mathf.FloorToInt(pos.y / cellSize);

        Vector3 cellCenter = new Vector3(
            x * cellSize + cellSize * 0.5f,
            y * cellSize + cellSize * 0.5f,
            0f
        );

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            cellCenter,
            Vector3.one * cellSize
        );
    }
}
