using System.Collections.Generic;
using UnityEngine;

public class PassiveEffectManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private List<PassiveEffectRuntimeData> _activeEffects = new();

    public static PassiveEffectManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Register(PassiveEffect effect)
    {
        var owned = PowerupManager.Instance.FindPlayerPowerup(effect);
        var effectRuntime = new PassiveEffectRuntimeData(owned);
        
        effect.Initialize(effectRuntime);
    }
}
