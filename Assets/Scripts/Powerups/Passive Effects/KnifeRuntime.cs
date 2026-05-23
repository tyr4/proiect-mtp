using System.Collections.Generic;
using UnityEngine;

public class KnifeRuntime : MonoBehaviour, IPassiveEffectBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    
    public PassiveEffectRuntimeData RuntimeData;
    private Transform _cachedTransform;

    private float _rotationFactor;
    private float _projectileCount;

    private List<Transform> _activeProjectiles = new();
    
    private void Awake()
    {
        _cachedTransform = transform;
    }
    
    public void Initialize(PassiveEffectRuntimeData data, PassiveEffect effect)
    {
        RuntimeData = data;
        OnTierUpgrade(effect);
    }

    public void OnTierUpgrade(PassiveEffect effect)
    {
        var knife = (Knife)effect;
        var tier = RuntimeData.ownedPowerup.CurrentTier;
        
        _rotationFactor = knife.GetRotationFactor(tier);
        _projectileCount = knife.GetProjectileCount(tier);
        
        UpdateProjectileRadius(knife.Radius);
    }

    private void UpdateProjectileRadius(float radius)
    {
        for (int i = _activeProjectiles.Count; i < _projectileCount; i++)
        {
            var obj = Instantiate(projectilePrefab, _cachedTransform);
            _activeProjectiles.Add(obj.transform);
        }
        
        var count = _activeProjectiles.Count;
        for (int i = 0; i < _activeProjectiles.Count; i++)
        {
            float angle = (Mathf.PI * 2f / count) * i;
            
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            
            var rotation = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            
            _activeProjectiles[i].localPosition = offset;
            _activeProjectiles[i].localRotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        _cachedTransform.Rotate(0, 0, _rotationFactor * dt);
    }
}
