using UnityEngine;

public class AnimEventProxy : MonoBehaviour
{
    private EnemyRuntime _runtime;

    private void Awake()
    {
        _runtime = GetComponentInParent<EnemyRuntime>();
    }

    public void OnAnimEvent(string key)
    {
        _runtime.OnAnimEvent(key);
    }
}