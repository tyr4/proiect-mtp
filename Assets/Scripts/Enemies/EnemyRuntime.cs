using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    // public Vector3 cachedPosition;
    public Transform cachedTransform;
    
    private float _health;
    private float _damage;
    private float _movementSpeed;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Material _defaultMaterial;
    private Material _flashMaterial;
    
    private const float DirectionForce = 10f;
    private const float SeparationForce = 1f;
    private const float FlashMaterialDuration = 0.1f;
    
    // dont update the direction every fixedupdate call
    private const float PathfindingRefreshRate = 0.2f;
    private float _timer = 0;
    
    private Vector3 _direction;
    private Vector3 _finalDirection;

    public Vector2 Velocity => _finalDirection * _movementSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _defaultMaterial = _sr.material;
        cachedTransform = transform;
        // cachedPosition = cachedTransform.position;
    }
    
    public void Initialize(EnemyData enemyData, Material flashMaterial)
    {
        Data = enemyData;
        _health = Data.Health;
        _damage = Data.Damage;
        _movementSpeed = Data.MovementSpeed;
        
        _flashMaterial = flashMaterial;

        // apply a random speed multiplier
        _movementSpeed *= Random.Range(0.8f, 1.1f);
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
        Debug.Log("Before Damage: " + $"{_health:F}, taking {damage} damage");
        _health -= damage;

        Debug.Log($"now {_health:F} health");
        if (_health <= 0)
        {
            WaveManager.Instance.ReturnToPool(Data, this);
            return;
        }
        
        StartCoroutine(TakeDamageAnimation());
    }

    // TODO: hurt logic here
    private IEnumerator TakeDamageAnimation()
    {
        _sr.material = _flashMaterial;

        yield return new WaitForSeconds(FlashMaterialDuration);
        
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
