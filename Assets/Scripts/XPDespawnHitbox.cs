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
    
        Debug.Log($"am intrat {xpRuntime.transform.position}, {_player.transform.position}");
        _player.HandleXpPickup(xpRuntime);

        xpRuntime.Despawn();
        xpRuntime.Collider.enabled = false;
    }
}
