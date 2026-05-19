using System;
using UnityEngine;

public class KnifeProjectileRuntime : MonoBehaviour
{
    private KnifeRuntime _parent;

    private void Awake()
    {
        _parent = GetComponentInParent<KnifeRuntime>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyRuntime>(out var enemy)) return;
        
        _parent.RuntimeData.DealDamage(enemy);
    }
}
