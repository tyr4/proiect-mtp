using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyRuntime : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    
    public EnemyData Data { get; private set; }
    // public Vector3 cachedPosition;
    public Transform cachedTransform;
    
    private float _health;
    public float Damage;
    private float _movementSpeed;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Material _defaultMaterial;
    // private Material _flashMaterial;
    
    private const float DirectionForce = 10f;
    private const float SeparationForce = 1f;
    private const float FlashMaterialDuration = 0.1f;
    
    // dont update the direction every fixedupdate call
    private const float PathfindingRefreshRate = 0.2f;
    private float _timer = 0;
    
    private Vector3 _direction;
    private Vector3 _finalDirection;
    private bool _isDead;

    public Vector2 Velocity => _finalDirection * _movementSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _defaultMaterial = new Material(_sr.material);
        cachedTransform = transform;
        // cachedPosition = cachedTransform.position;
    }
    
    public void Initialize(EnemyData enemyData)
    {
        Data = enemyData;
        _health = Data.Health;
        Damage = Data.Damage;
        _movementSpeed = Data.MovementSpeed;
        _isDead = false;

        // apply a random speed multiplier
        _movementSpeed *= Random.Range(0.8f, 1.1f);
        
        // kill any ongoing tweens
        // DOTween.Kill(_sr);
        // DOTween.Kill(cachedTransform);
        
        // reset the alpha back to 1
        // var currentColor = _sr.color;
        // currentColor.a = 1f;
        // _sr.color = currentColor;
    }
    
    public void Tick(float deltaTime, Transform playerTransform, List<EnemyRuntime> neighbors, float separationRadius)
    {
        _timer += deltaTime;
        
        Vector3 separation = Vector3.zero;
        
        // step 1: cache positions
        // apply the swarm effect
        // basically, distance close-by enemies from themselves a bit
        float separationRadiusSqr =
            separationRadius * separationRadius;

        // foreach (var other in neighbors)
        // {
        //     if (ReferenceEquals(other, this))
        //         continue;
        //
        //     Vector3 diff =
        //         cachedPosition - other.cachedPosition;
        //
        //     float sqrDist = diff.sqrMagnitude;
        //
        //     if (sqrDist < separationRadiusSqr)
        //     {
        //         separation += diff / (sqrDist + 0.001f);
        //     }
        // }

        if (_timer >= PathfindingRefreshRate)
        {
             _direction = playerTransform.position - cachedTransform.position;
            _finalDirection = _direction.normalized;

            _timer = 0;
        }

        // move the enemy
        // _cachedTransform.position += finalDirection * (_movementSpeed * deltaTime);
        _rb.linearVelocity = _finalDirection * _movementSpeed;

        if (_direction.x != 0)
        {
            cachedTransform.rotation = Quaternion.Euler(0f, _direction.x < 0 ? 180f : 0f, 0f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        
        _health -= damage;

        if (_health <= 0)
        {
            Kill();
            return;
        }

        TakeDamageAnimation();
        // if (_flashCoroutine != null)
        // {
        //     StopCoroutine(_flashCoroutine);
        // }
        //
        // _flashCoroutine = StartCoroutine(TakeDamageAnimation());
    }

    private void Kill()
    {
        if (_isDead) return;
        _isDead = true;
        
        EnemyManager.Instance.Unregister(this);
        
        DieAnimation();
    }

    // TODO: hurt logic here
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

    private void DieAnimation()
    {
        _sr.material = _defaultMaterial;
        _sr.DOKill();

        _sr.DOFade(0f, 0.15f)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                ResetVisual();
                WaveManager.Instance.ReturnToPool(Data, this);
                XPManager.Instance.SpawnXP(cachedTransform.position);
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
