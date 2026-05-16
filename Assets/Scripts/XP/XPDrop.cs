using UnityEngine;

[CreateAssetMenu(fileName = "XPDrop", menuName = "XP Drop")]
public class XPDrop : ScriptableObject
{
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public float Value { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
}
