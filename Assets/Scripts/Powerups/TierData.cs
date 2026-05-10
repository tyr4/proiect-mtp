
[System.Serializable]
public abstract class TierData
{
    public int tier1;
    public int tier2;
    public int tier3;
    
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
