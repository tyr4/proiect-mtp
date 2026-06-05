using System;
using UnityEngine;

public class VermiRuntimeProjectile : MonoBehaviour
{
    private ShootingEnemy _data;
    private ShootingEnemyRuntime _shootingRuntime;
    
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    private Rigidbody2D _rb;
    
    private Vector2 _velocity;
    private float _lifetime;
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _cachedTransform = transform;
    }

    public void Launch(ShootingEnemyRuntime runtime, Vector2 origin, Vector2 velocity)
    {
        _shootingRuntime = runtime;
        _data = (ShootingEnemy)runtime.Data;
        _velocity = velocity;
        _lifetime = _data.Lifetime;

        _cachedTransform.position = origin;
        // _cachedTransform.rotation = Quaternion.Euler(0, 0, angleDeg);
        _sr.sprite = _data.ProjectileSprite;
        

        gameObject.SetActive(true);
        
        Debug.Log(Vector2.right * velocity + " " + velocity);
        _rb.linearVelocity = velocity;
    }
     
    private void FixedUpdate()
    {
        // _cachedTransform.position += (Vector3)(_velocity * Time.fixedDeltaTime);
        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime <= 0f)
        {
            ShootingEnemyManager.Instance.ReturnPoolObject(_data, gameObject);
            return;
        }
        
        // check collision
        // if (Physics2D.OverlapBox(_cachedTransform.position, _boxSize, _cachedTransform.eulerAngles.z, _filter, Hits) > 0)
        // {
        //     if (!Hits[0].TryGetComponent<CollisionHitbox>(out var hitbox)) return;
        //     
        //     hitbox.PlayerTakeDamage(_shootingRuntime.Damage);
        //     ShootingEnemyManager.Instance.ReturnPoolObject(_data, gameObject);
        // }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<CollisionHitbox>(out var hitbox)) return;
        
        hitbox.PlayerTakeDamage(_shootingRuntime.Damage);
        ShootingEnemyManager.Instance.ReturnPoolObject(_data, gameObject);
    }
    
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.matrix = Matrix4x4.TRS(_cachedTransform.position, _cachedTransform.rotation, Vector3.one);
    //     Gizmos.DrawWireCube(Vector3.zero, _boxSize);
    //     Gizmos.matrix = Matrix4x4.identity; // reset so other gizmos aren't affected
    // }
}
