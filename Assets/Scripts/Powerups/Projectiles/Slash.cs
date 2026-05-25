using UnityEngine;

[CreateAssetMenu(fileName = "Slash", menuName = "Powerups/Projectiles/Slash")]
public class Slash : Projectile, IAttachedToPlayer
{
    [field: SerializeField] public TierData ScaleX { get; private set; }

    public float GetScaleX(int tier) => ScaleX.GetValue(tier);
}