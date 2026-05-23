using UnityEngine;

[CreateAssetMenu(fileName = "Bow", menuName = "Powerups/Projectiles/Bow")]
public class Bow : Projectile
{
    [field: SerializeField] public float ArcSpreadDegrees { get; private set; } // e.g. 30
}
