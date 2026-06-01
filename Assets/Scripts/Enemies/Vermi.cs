using UnityEngine;

[CreateAssetMenu(fileName = "Vermi", menuName = "Enemies/Bosses/Vermi")]
public class Vermi : Boss
{
    [field: SerializeField] public float SpawnAttackCooldown { get; private set; }
    [field: SerializeField] public float ProjectileAttackCooldown { get; private set; }
    
    [field: SerializeField] public Enemy SpawnEnemy { get; private set; }
    [field: SerializeField] public int SpawnAmount { get; private set; }
    
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public int ProjectileAmount { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
}
