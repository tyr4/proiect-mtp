using System;
using UnityEngine;

public class SkeletonMageProjectileRuntime : MonoBehaviour
{
    private EnemyRuntime _runtime;
    private ShootingEnemyRuntime _shootingRuntime;
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    private Vector2 _velocity;
    private float _lifetime;
    private ShootingEnemy _data;
    
    private readonly Vector2 _boxSize = new Vector2(0.14f, 0.04f);

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cachedTransform = transform;
    }

    public void Launch(ShootingEnemy data, Vector2 origin, Vector2 velocity, float angleDeg)
    {
        _data = data;
        _velocity = velocity;
        _lifetime = data.Lifetime;

        _cachedTransform.position = origin;
        _cachedTransform.rotation = Quaternion.Euler(0, 0, angleDeg);
        _sr.sprite = data.ProjectileSprite;

        gameObject.SetActive(true);
    }
    
    private void FixedUpdate()
    {
        _cachedTransform.position += (Vector3)(_velocity * Time.fixedDeltaTime);
        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime <= 0f)
        {
            ShootingEnemyManager.Instance.ReturnPoolObject(_data, gameObject);
            return;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Player>(out var player)) return;
        
        player.TakeDamage(_runtime.Damage);
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.matrix = Matrix4x4.TRS(_cachedTransform.position, _cachedTransform.rotation, Vector3.one);
    //     Gizmos.DrawWireCube(Vector3.zero, _boxSize);
    //     Gizmos.matrix = Matrix4x4.identity; // reset so other gizmos aren't affected
    // }
}
