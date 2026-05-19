using UnityEngine;

[CreateAssetMenu(fileName = "Extra XP Range", menuName = "Powerups/One Time Buff/Extra XP Range")]
public class ExtraXPRadiusBuff : OneTimeBuff
{
    public override void ApplyBuff()
    {
        Player.Instance.ModifyXPRadius(Value, valueType);
    }
}