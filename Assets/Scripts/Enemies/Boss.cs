using System;
using UnityEngine;

public class Boss : ShootingEnemy
{
    [field: SerializeField] public AudioClip BossMusic { get; private set; }
}
