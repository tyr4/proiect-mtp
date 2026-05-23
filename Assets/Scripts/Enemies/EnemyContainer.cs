using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemies/Enemy Container")]
public class EnemyContainer : ScriptableObject
{
    public List<Enemy> Enemies;
}