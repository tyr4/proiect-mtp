using UnityEngine;

public class CollisionHitbox : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }
    
    public void PlayerTakeDamage(float value)
    {
        _player.TakeDamage(value);
    }
}