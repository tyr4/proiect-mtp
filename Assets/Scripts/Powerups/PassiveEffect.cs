using UnityEngine;

[System.Serializable]
public class PassiveEffect : Powerup, IHasTiers
{
    [field: SerializeField] public GameObject EffectPrefab { get; private set; }
    [field: SerializeField] public Sprite EffectSprite { get; private set; }
    [field: SerializeField] public TierData TickRate { get; private set; }
    [field: SerializeField] public TierData Damage { get; private set; }
    [field: SerializeField] public TierData Speed { get; private set; }
    [field: SerializeField] public TierData Count { get; private set; }
    
    public virtual void Initialize(PassiveEffectRuntimeData runtimeData) { }

    public override void OnAssign()
    {
        PassiveEffectManager.Instance.Register(this);
    }
}
