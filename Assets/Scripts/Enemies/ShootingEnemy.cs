using UnityEngine;
using UnityEngine.Serialization;

public class ShootingEnemy : Enemy
{
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public Sprite ProjectileSprite { get; private set; }
    [field: SerializeField] public float ShootingRange { get; private set; }
    [field: SerializeField] public float ArcSpread { get; private set; }
    [field: SerializeField] public float Cooldown { get; private set; }
    [field: SerializeField] public int Count { get; private set; }
    [field: SerializeField] public float ProjDamage { get; private set; }
    [field: SerializeField] public float ProjSpeed { get; private set; }
    [field: SerializeField] public float Lifetime { get; private set; }
}
