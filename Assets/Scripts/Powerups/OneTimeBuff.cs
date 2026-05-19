using System.Text;
using UnityEngine;

public class OneTimeBuff : Powerup
{
    public enum ValueType
    {
        Additive,
        Multiplicative,
        Percentage
    }

    [field: SerializeField] public ValueType valueType { get; private set; }
    [field: SerializeField] public float Value { get; private set; }

    public virtual void ApplyBuff() { }

    public override void OnSelect()
    {
        ApplyBuff();
    }

    public override string GetDescription()
    {
        switch (valueType)
        {
            case ValueType.Additive:
                return Description.Replace("{value}", $"+{Value:F1}");

            case ValueType.Multiplicative:
                return Description.Replace("{value}", $"x{Value:F1}");
            
            case ValueType.Percentage:
                return Description.Replace("{value}", $"+{Value:F1}%");
        }
        
        return base.GetDescription();
    }
}
