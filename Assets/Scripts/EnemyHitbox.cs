using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyRuntime>(out var enemy)) return;
        
        _player.HandleEnemyCollision(enemy);
    }
}
