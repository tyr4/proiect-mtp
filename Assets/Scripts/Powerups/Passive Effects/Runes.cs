using UnityEngine;

[CreateAssetMenu(fileName = "Runes", menuName = "Powerups/Passive Effects/Runes")]
public class Runes : PassiveEffect
{  
    public float GetProjectileCount(int tier) =>
        Mathf.RoundToInt(Count.GetValue(tier));
    
    public float GetSpeed(int tier) => Speed.GetValue(tier);
}
