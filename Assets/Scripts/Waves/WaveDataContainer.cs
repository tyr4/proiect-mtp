using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Data Container", menuName = "Waves/Wave Data Container")]
public class WaveDataContainer : ScriptableObject
{
    public List<WaveData> waves;    
}
