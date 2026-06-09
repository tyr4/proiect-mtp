using UnityEngine;

[CreateAssetMenu(fileName = "Vermi", menuName = "Enemies/Bosses/Vermi")]
public class Vermi : Boss
{
    [field: SerializeField] public float SpawnAttackCooldown { get; private set; }
    
    [field: SerializeField] public float ProjectileAngleOffset { get; private set;}
    [field: SerializeField] public float ProjectileAttackCooldown { get; private set; }
    
    [field: SerializeField] public float ProjectileFirstXValue { get; private set; }
    [field: SerializeField] public float ProjectileXStepValue { get; private set; }

    [field: SerializeField] public float ProjectileFirstYValue { get; private set; }
    [field: SerializeField] public float ProjectileYStepValue { get; private set; }
    
    [field: SerializeField] public Enemy SpawnEnemy { get; private set; }
    [field: SerializeField] public int SpawnAmount { get; private set; }
    
}
