using UnityEngine;

public class BowRuntimeProjectile : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    
    private Vector2 _velocity;
    private float _lifetime;
    
    private ProjectileRuntimeData _projRuntimeData;
    private ProjectileHitState _hitState;
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cachedTransform = transform;
    }

    public void Launch(ProjectileRuntimeData runtimeData, Vector2 velocity, Vector2 spawnPos, float angleDeg, Bow bow)
    {
        _projRuntimeData = runtimeData;
        _hitState = _projRuntimeData.GenerateHitState(_hitState);
        
        _velocity = velocity;
        _lifetime = 5f;

        _cachedTransform.position = spawnPos;
        _cachedTransform.rotation = Quaternion.Euler(0, 0, angleDeg);
        _sr.sprite = bow.ProjectileSprite;
        
        gameObject.SetActive(true);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        // check if the enemy can be hit, mostly avoiding multiple trigger enters
        if (!other.gameObject.TryGetComponent<EnemyRuntime>(out var enemy)) return;

        if (_hitState.CanDealDamage())
        {
            _hitState.DealDamage(enemy);
            return;
        }
        
        _projRuntimeData.ReturnPoolObject(gameObject);
    }
    
    private void FixedUpdate()
    {
        
        _cachedTransform.position += (Vector3)(_velocity * Time.fixedDeltaTime);
        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime <= 0) _projRuntimeData.ReturnPoolObject(gameObject);
    }
}