using UnityEngine;

public class XPMagnetHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<XPDropRuntime>(out var xpRuntime)) return;
        
        xpRuntime.Attract();
    }
}