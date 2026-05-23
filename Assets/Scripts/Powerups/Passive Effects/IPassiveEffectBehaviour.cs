public interface IPassiveEffectBehaviour
{
    void Initialize(PassiveEffectRuntimeData data, PassiveEffect effect);
    void OnTierUpgrade(PassiveEffect effect);
}
