using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    
    private Vector2 _velocity;
    private float _damage;
    private float _lifetime;
    private Projectile _projectile;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cachedTransform = transform;
    }
    
    public void Launch(Projectile projectile, Vector2 velocity, float damage, float lifetime)
    {
        _sr.sprite = projectile.ProjectileSprite;
        _projectile = projectile;
        _velocity = velocity;
        _damage = damage;
        _lifetime = lifetime;
        
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        _cachedTransform.position += (Vector3)(_velocity * Time.fixedDeltaTime);
        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime <= 0) Return();
    }

    private void Return()
    {
        gameObject.SetActive(false);
        ProjectileManager.Instance.ReturnPoolObject(_projectile, this);
    }
}
