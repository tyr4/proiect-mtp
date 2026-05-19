using UnityEngine;

[CreateAssetMenu(fileName = "Extra HP", menuName = "Powerups/One Time Buff/Extra HP")]
public class ExtraHPBuff : OneTimeBuff
{
    public override void ApplyBuff()
    {
        Debug.Log("Applying buff...");
        Player.Instance.ModifyMaxHealth(Value, valueType);
    }
}
