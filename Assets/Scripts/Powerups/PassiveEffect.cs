using UnityEngine;

[System.Serializable]
public class PassiveEffect : Powerup, IHasTiers
{
    [field: SerializeField] public GameObject EffectPrefab { get; private set; }
    [field: SerializeField] public TierData TickRate { get; private set; }
    [field: SerializeField] public TierData Damage { get; private set; }
    [field: SerializeField] public TierData Speed { get; private set; }
    [field: SerializeField] public TierData Count { get; private set; }
    
    public override void OnAssign()
    {
        PassiveEffectManager.Instance.Register(this);
    }

    public override void OnSelect(OwnedPowerup owned)
    {
        var runtime = PassiveEffectManager.Instance.FindRuntimeData(owned);

        if (runtime == null)
        {
            Debug.LogError("nu ai runtime data bos");
            return;
        }
        
        runtime.OnTierUpgrade();
    }
}
