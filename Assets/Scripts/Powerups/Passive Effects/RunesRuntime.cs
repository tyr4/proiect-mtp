using System.Collections.Generic;
using UnityEngine;

public class RunesRuntime : MonoBehaviour, IPassiveEffectBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    public PassiveEffectRuntimeData RuntimeData;
    private Transform _cachedTransform;
    private Transform _cameraTransform;

    private float _rotationFactor;
    private float _projectileCount;

    private List<RunesProjectileRuntime> _activeProjectiles = new();
    
    private void Awake()
    {
        _cachedTransform = transform;
        _cameraTransform = Camera.main!.transform;
    }
    
    public void Initialize(PassiveEffectRuntimeData data, PassiveEffect effect)
    {
        RuntimeData = data;
        OnTierUpgrade(effect);
    }

    public void OnTierUpgrade(PassiveEffect effect)
    {
        var runes = (Runes)effect;
        var tier = RuntimeData.ownedPowerup.CurrentTier;
        
        _projectileCount = runes.GetProjectileCount(tier);
        
        UpdateProjectileCount();
        UpdateProjectileSpeed(runes, tier);
    }
    
    private void UpdateProjectileCount()
    {
        for (int i = _activeProjectiles.Count; i < _projectileCount; i++)
        {
            var obj = Instantiate(projectilePrefab);
            var runtime = obj.GetComponent<RunesProjectileRuntime>();
            
            runtime.Initialize(this);
            _activeProjectiles.Add(runtime);
        }
    }

    private void UpdateProjectileSpeed(Runes runes, int tier)
    {
        var speed = runes.GetSpeed(tier);

        foreach (var proj in _activeProjectiles)
        {
            proj.UpdateSpeed(speed);
        }
    }
}
