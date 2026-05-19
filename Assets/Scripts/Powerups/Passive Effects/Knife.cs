using UnityEngine;

[CreateAssetMenu(fileName = "Knife", menuName = "Powerups/Passive Effects/Knife")]
public class Knife : PassiveEffect
{
    public override void Initialize(PassiveEffectRuntimeData runtimeData)
    {
        var obj = Instantiate(EffectPrefab, Player.Instance.PassiveEffectsContainer);
        var runtime = obj.GetComponent<KnifeRuntime>();
        
        runtime.Initialize(runtimeData);
    }
}
