using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    public EnemyData Data { get; private set; }
    public Vector3 cachedPosition;
    public Transform cachedTransform;

    private float _health;
    private float _damage;
    private float _movementSpeed;
    private Rigidbody2D _rb;

    // MAGIC NUMBERS!!!
    // apply "weights" to the separation force
    private const float DirectionForce = 10f;
    private const float SeparationForce = 1f;

    // dont update the direction every fixedupdate call
    private const float PathfindingRefreshRate = 0.2f;
    private float _timer = 0;
    private Vector3 _finalDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        cachedTransform = transform;
        cachedPosition = cachedTransform.position;
    }
    
    public void Initialize(EnemyData enemyData)
    {
        Data = enemyData;
        _health = Data.Health;
        _damage = Data.Damage;
        _movementSpeed = Data.MovementSpeed;
        
        // apply a random speed multiplier
        _movementSpeed *= Random.Range(0.8f, 1.1f);
    }
    
    public void Tick(float deltaTime, Vector3 direction, List<EnemyRuntime> neighbors, float separationRadius)
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
            _finalDirection = (
                    direction * DirectionForce + 
                    separation * SeparationForce
            ).normalized;

            _timer = 0;
        }

        // move the enemy
        // _cachedTransform.position += finalDirection * (_movementSpeed * deltaTime);
        _rb.linearVelocity = _finalDirection * _movementSpeed;

        if (direction.x != 0)
        {
            cachedTransform.rotation = Quaternion.Euler(0f, direction.x < 0 ? 180f : 0f, 0f);
        }
    }

    // TODO: hurt logic here
    public void UpdateAnimation()
    {
        
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
