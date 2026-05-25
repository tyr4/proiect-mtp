using UnityEngine;

public class XPDespawnHitbox : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<XPDropRuntime>(out var xpRuntime)) return;
    
        _player.HandleXpPickup(xpRuntime);

        xpRuntime.Despawn();
        xpRuntime.Collider.enabled = false;
    }
}
