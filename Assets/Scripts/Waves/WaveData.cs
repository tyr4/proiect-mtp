using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Data", menuName = "Waves/Wave Data")]
public class WaveData : ScriptableObject
{
    public int startTime;
    public int amount;
    public float spawnInterval;
    public bool spawnAllAtOnce;
    
    public Enemy defaultEnemy;
    public Enemy boss;
    public List<EnemyWaveData> specialEnemies;
}
