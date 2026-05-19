using UnityEngine;

public class KnifeRuntime : MonoBehaviour
{
    [SerializeField] private float rotationFactor;
    public PassiveEffectRuntimeData RuntimeData;
    private Transform _cachedTransform;

    private void Awake()
    {
        _cachedTransform = transform;
    }
    
    public void Initialize(PassiveEffectRuntimeData data)
    {
        RuntimeData = data;
        Debug.Log("am intrat in knife runtime " + data + " " + RuntimeData);
    }

    private void Update()
    {
        var dt = Time.deltaTime;
        _cachedTransform.Rotate(0, 0, rotationFactor * dt);
    }
}
