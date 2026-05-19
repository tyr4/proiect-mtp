using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBulletRuntime : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    
    private Vector2 _velocity;
    private float _lifetime;
    private Projectile _projectile;
    private ProjectileRuntimeData _projRuntimeData;
    private HashSet<int> _hitEnemies = new();

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cachedTransform = transform;
    }
    
    public void Launch(ProjectileRuntimeData projRuntimeData, Vector2 velocity, float damage, float lifetime)
    {
        _projectile = (Projectile)projRuntimeData.ownedPowerup.Base;
        _projRuntimeData = projRuntimeData;
        _projRuntimeData.ResetPierces();
        
        _sr.sprite = _projectile.ProjectileSprite;
        
        _velocity = velocity;
        _lifetime = lifetime;
        
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        _cachedTransform.position += (Vector3)(_velocity * Time.fixedDeltaTime);
        _lifetime -= Time.fixedDeltaTime;

        if (_lifetime <= 0) ReturnPoolObject();
    }

    private void ReturnPoolObject()
    {
        _hitEnemies.Clear();
        gameObject.SetActive(false);
        ProjectileManager.Instance.ReturnPoolObject(_projectile, this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // check if the enemy can be hit, mostly avoiding multiple trigger enters
        if (!other.gameObject.TryGetComponent<EnemyRuntime>(out var enemy)) return;
        int enemyID = enemy.GetInstanceID();
        if (_hitEnemies.Contains(enemyID)) return;

        var damage = _projRuntimeData.GetDamage();
        
        enemy.TakeDamage(damage);
        _hitEnemies.Add(enemyID);

        _projRuntimeData.DecrementPiercesLeft();
        if (_projRuntimeData.CanPierce()) return;

        ReturnPoolObject();
    }
}
