using System;

[System.Serializable]
public class OwnedPowerup
{
    public Powerup Base;
    public int CurrentTier;

    public OwnedPowerup(Powerup Base, int tier)
    {
        this.Base = Base;
        CurrentTier = tier;
    }
}
