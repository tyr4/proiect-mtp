using System;
using UnityEngine;

public class BowSkeletonRuntimeProjectile : MonoBehaviour
{
    private EnemyRuntime _runtime;
    private ShootingEnemyRuntime _shootingRuntime;
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    private Vector2 _velocity;
    private float _lifetime;
    private ShootingEnemy _data;
    
    private static readonly Collider2D[] Hits = new Collider2D[1];
    private static ContactFilter2D _filter = new ContactFilter2D
    {
        useLayerMask = true,
        layerMask = ~0
    };
    
    private readonly Vector2 _boxSize = new Vector2(0.14f, 0.04f);

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cachedTransform = transform;
        
        _filter.layerMask = 1 << LayerMask.NameToLayer("Player");
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
        
        // check collision
        if (Physics2D.OverlapBox(_cachedTransform.position, _boxSize, _cachedTransform.eulerAngles.z, _filter, Hits) > 0)
        {
            if (!Hits[0].TryGetComponent<CollisionHitbox>(out var hitbox)) return;
            
            hitbox.PlayerTakeDamage(_data.ProjDamage);
            ShootingEnemyManager.Instance.ReturnPoolObject(_data, gameObject);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(_cachedTransform.position, _cachedTransform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, _boxSize);
        Gizmos.matrix = Matrix4x4.identity; // reset so other gizmos aren't affected
    }
}
