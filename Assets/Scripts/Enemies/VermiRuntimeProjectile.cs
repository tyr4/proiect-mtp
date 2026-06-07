using System.Collections;
using UnityEngine;

public class VermiRuntimeProjectile : MonoBehaviour
{
    private ShootingEnemy _data;
    private ShootingEnemyRuntime _shootingRuntime;
    
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    private Rigidbody2D _rb;
    private Animator _animator;

    private float _gravity;
    private float _lifetime;
    
    private static readonly int HasHit = Animator.StringToHash("hasHit");
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _gravity = _rb.gravityScale;
        _cachedTransform = transform;
    }

    public void Launch(ShootingEnemyRuntime runtime, Vector2 origin, Vector2 velocity)
    {
        _shootingRuntime = runtime;
        _data = (ShootingEnemy)runtime.Data;
        _lifetime = _data.Lifetime;

        _cachedTransform.position = origin;
        // _cachedTransform.rotation = Quaternion.Euler(0, 0, angleDeg);
        _sr.sprite = _data.ProjectileSprite;
        
        gameObject.SetActive(true);
        
        _rb.linearVelocity = velocity;
        _rb.gravityScale = _gravity;
    }
     
    private void FixedUpdate()
    {
        if (_rb.linearVelocity != Vector2.zero)
        {
            var angle = Mathf.Atan2(_rb.linearVelocity.y, -_rb.linearVelocity.x) *  Mathf.Rad2Deg;
            _cachedTransform.rotation = Quaternion.Euler(0, 0, angle);
        }

        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime <= 0f)
        {
            ShootingEnemyManager.Instance.ReturnPoolObject(_data, gameObject);
            return;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<CollisionHitbox>(out var hitbox)) return;
        
        hitbox.PlayerTakeDamage(_shootingRuntime.Damage);

        StartCoroutine(ImpactCoroutine());
    }

    private IEnumerator ImpactCoroutine()
    {
        _animator.SetTrigger(HasHit);
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;
        
        yield return null;
        
        yield return Animations.WaitForAnimationEnd(_animator);

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
