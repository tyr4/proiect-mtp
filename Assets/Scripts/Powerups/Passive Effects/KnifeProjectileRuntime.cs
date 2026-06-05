using System;
using UnityEngine;

public class KnifeProjectileRuntime : MonoBehaviour
{
    private KnifeRuntime _parent;
    private ParticleSystem _ps;
    
    private Transform _cachedTransform;
    private Transform _psTransform;
    
    private void Awake()
    {
        _cachedTransform = transform;
        _parent = GetComponentInParent<KnifeRuntime>();
        _ps = GetComponentInChildren<ParticleSystem>();
        _psTransform = _ps.GetComponent<Transform>();
        
        _ps.Stop();
        _ps.transform.SetParent(null);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyRuntime>(out var enemy)) return;
        
        Vector3 contactPoint = other.ClosestPoint(_cachedTransform.position);

        _parent.RuntimeData.DealDamage(enemy);
        
        var scale = _psTransform.localScale;
        var enemyScale = enemy.cachedTransform.localScale;
        
        _psTransform.SetParent(enemy.cachedTransform);
        _psTransform.position = contactPoint;
        _psTransform.localScale = new Vector3
        (
            enemyScale.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x), 
            scale.y, 
            scale.z
        );
        
        _ps.Play();
    }
}
