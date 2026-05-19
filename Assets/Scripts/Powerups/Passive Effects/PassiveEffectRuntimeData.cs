using UnityEngine;

public class PassiveEffectRuntimeData
{
    public OwnedPowerup ownedPowerup;
    private PassiveEffect _passiveEffect;
    private float _cooldownTimer;

    public PassiveEffectRuntimeData(OwnedPowerup powerup)
    {
        ownedPowerup = powerup;
        _passiveEffect = (PassiveEffect)powerup.Base;
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
