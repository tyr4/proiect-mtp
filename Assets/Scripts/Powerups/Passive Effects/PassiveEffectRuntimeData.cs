using UnityEngine;

public class PassiveEffectRuntimeData
{
    public OwnedPowerup ownedPowerup;
    private PassiveEffect _passiveEffect;
    private IPassiveEffectBehaviour _behaviour;

    public PassiveEffectRuntimeData(OwnedPowerup powerup)
    {
        ownedPowerup = powerup;
        _passiveEffect = (PassiveEffect)powerup.Base;

        var obj = Object.Instantiate(_passiveEffect.EffectPrefab, Player.Instance.PassiveEffectsContainer);
        _behaviour = obj.GetComponent<IPassiveEffectBehaviour>();
        _behaviour?.Initialize(this, _passiveEffect);
    }
    
    public void OnTierUpgrade()
    {
        _behaviour?.OnTierUpgrade(_passiveEffect);
    }
    
    public float GetTickRate() => _passiveEffect.TickRate.GetValue(ownedPowerup.CurrentTier);
    public float GetDamage() => _passiveEffect.Damage.GetValue(ownedPowerup.CurrentTier);
    public float GetSpeed() => _passiveEffect.Speed.GetValue(ownedPowerup.CurrentTier);
    public float GetCount() => _passiveEffect.Count.GetValue(ownedPowerup.CurrentTier);
    
    public void DealDamage(EnemyRuntime enemy)
    {
        enemy.TakeDamage(GetDamage());
    }
}
