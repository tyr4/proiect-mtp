using System;
using UnityEngine;

public class Boss : Enemy
{
    [field: SerializeField] public AudioClip BossMusic { get; private set; }
}
