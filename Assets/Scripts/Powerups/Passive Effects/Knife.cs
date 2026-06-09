using UnityEngine;

[CreateAssetMenu(fileName = "Knife", menuName = "Powerups/Passive Effects/Knife")]
public class Knife : PassiveEffect
{
    [field: SerializeField] public float Radius { get; set; }
    [field: SerializeField] public TierData RotationFactor { get; private set; }
    
    public float GetRotationFactor(int tier) =>
        RotationFactor.GetValue(tier);
    
    public float GetProjectileCount(int tier) =>
        Mathf.RoundToInt(Count.GetValue(tier));
}
