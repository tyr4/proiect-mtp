using UnityEngine;

[CreateAssetMenu(fileName = "Bow", menuName = "Powerups/Projectiles/Bow")]
public class Bow : Projectile
{
    [field: SerializeField] public Vector2 PositionOffset { get; private set; }
    [field: SerializeField] public float AngleOffset { get; private set; }
}
