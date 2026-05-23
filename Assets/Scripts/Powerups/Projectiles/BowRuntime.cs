using System;
using System.Collections.Generic;
using UnityEngine;

public class BowRuntime : MonoBehaviour, IProjectileBehaviour
{
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    
    private Vector2 _velocity;
    private float _lifetime;
    private Projectile _projectile;
    private ProjectileRuntimeData _projRuntimeData;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cachedTransform = transform;
    }
    
    public void Shoot(ProjectileRuntimeData data, Vector2 playerPos, Vector2 nearestEnemyPos, Vector2 nearestEnemyVelocity)
    {
        var bow = (Bow)data.ownedPowerup.Base;
        var projectileSpeed = data.GetSpeed();

        var travelTime = (nearestEnemyPos - playerPos).magnitude / projectileSpeed;
        var predictedPos = nearestEnemyPos + nearestEnemyVelocity * travelTime;

        var direction = (predictedPos - playerPos).normalized;
    
        _projRuntimeData = data;
        _projRuntimeData.Initialize();
        _velocity = direction * projectileSpeed;
        _lifetime = 5f;

        _cachedTransform.position = playerPos + bow.PositionOffset;
        _cachedTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + bow.AngleOffset);
    
        _sr.sprite = bow.ProjectileSprite;
    
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        _cachedTransform.position += (Vector3)(_velocity * Time.fixedDeltaTime);
        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime <= 0) _projRuntimeData.ReturnPoolObject(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // check if the enemy can be hit, mostly avoiding multiple trigger enters
        if (!other.gameObject.TryGetComponent<EnemyRuntime>(out var enemy)) return;

        if (_projRuntimeData.CanDealDamage())
        {
            _projRuntimeData.DealDamage(enemy);
            return;
        }
        
        _projRuntimeData.ReturnPoolObject(gameObject);
    }
}
