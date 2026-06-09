using UnityEngine;

[CreateAssetMenu(fileName = "Boomerang", menuName = "Powerups/Projectiles/Boomerang")]
public class Boomerang : Projectile
{
    [field: SerializeField] public TierData Range { get; private set; } // e.g. 30
    [field: SerializeField] public TierData ArcWidth { get; private set; } // e.g. 30
    [field: SerializeField] public TierData ArcSpreadAngle { get; private set; } // e.g. 30
    [field: SerializeField] public TierData FlightDuration { get; private set; } // e.g. 30
    [field: SerializeField] public float SpinSpeed { get; private set; } // e.g. 30
    
}