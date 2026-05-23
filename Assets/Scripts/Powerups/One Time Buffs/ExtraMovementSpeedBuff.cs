using UnityEngine;

[CreateAssetMenu(fileName = "Extra HP", menuName = "Powerups/One Time Buff/Extra Movement Speed")]
public class ExtraMovementSpeedBuff : OneTimeBuff
{
    public override void ApplyBuff()
    {
        Debug.Log("Applying buff...");
        Player.Instance.ModifyMovementSpeed(Value, valueType);
    }
}
