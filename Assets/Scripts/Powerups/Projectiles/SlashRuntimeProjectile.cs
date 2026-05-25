using UnityEngine;

public class SlashRuntimeProjectile : MonoBehaviour
{
    private Projectile _projectile;
    private ProjectileRuntimeData _projRuntimeData;
    private ProjectileHitState _hitState;
    
    private BoxCollider2D _collider;
    private Transform _cachedTransform;
    
    private Vector3 _scale;
    private Vector3 _position;
    
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _cachedTransform = transform;
        _scale = _cachedTransform.localScale;
        _position = _cachedTransform.localPosition;
    }
    
    public void Launch(ProjectileRuntimeData data, Slash slash, bool isAttackingBehind)
    {
        _projRuntimeData = data;
        _hitState = _projRuntimeData.GenerateHitState(_hitState);
        
        var newScaleX = slash.GetScaleX(data.ownedPowerup.CurrentTier) + _scale.x;
        newScaleX = isAttackingBehind ? -newScaleX : newScaleX;
        
        _cachedTransform.localScale = new Vector3(newScaleX, _scale.y, _scale.z);
        _cachedTransform.localPosition = GetRandomPosition(isAttackingBehind);
        
        gameObject.SetActive(true);
    }

    public void EnableCollisions()
    {
        _collider.enabled = true;
    }

    public void DisableCollisions()
    {
        _collider.enabled = false;
    }

    public void ReturnToPool()
    {
        _projRuntimeData.ReturnPoolObject(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<EnemyRuntime>(out var enemy)) return;

        _hitState.DealDamage(enemy);
    }

    private Vector2 GetRandomPosition(bool isAttackingBehind)
    {
        float posX = Random.Range(0, 0.07f);
        float posY = Random.Range(-0.07f, 0.07f);

        if (isAttackingBehind) posX = -posX;
        
        Vector2 newPos = new Vector2(posX + _position.x, posY + _position.y);

        return newPos;
    }
}
