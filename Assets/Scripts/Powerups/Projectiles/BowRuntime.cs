using System;
using System.Collections.Generic;
using UnityEngine;

public class BowRuntime : MonoBehaviour
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
    
    public void Launch(ProjectileRuntimeData projRuntimeData, Vector2 velocity, float damage, float lifetime)
    {
        _projectile = (Projectile)projRuntimeData.ownedPowerup.Base;
        _projRuntimeData = projRuntimeData;
        _projRuntimeData.Initialize();
        
        _sr.sprite = _projectile.ProjectileSprite;
        
        _velocity = velocity;
        _lifetime = lifetime;
        
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
