
using UnityEngine;

[System.Serializable]
public class TierData
{
    public float tier1;
    public float tier2;
    public float tier3;
    
    public float GetValue(int tier)
    {
        return tier switch
        {
            1 => tier1,
            2 => tier2,
            3 => tier3,
            _ => 0
        };
    }
}
