using UnityEngine;

public class BoomerangRuntimeProjectile : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Transform _cachedTransform;
    
    private float _lifetime;

    private Boomerang _boomerang;
    private ProjectileRuntimeData _projRuntimeData;
    private ProjectileHitState _hitState;

    private Transform _playerTransform;
    private Vector3 _peakPos;
    private Vector3 _originPos;
    
    private float _t;
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _cachedTransform = transform;
    }

    private void Start()
    {
        _playerTransform = Player.Instance.transform;
    }

    public void Launch(ProjectileRuntimeData runtimeData, Vector2 spawnPos, Vector2 direction, Boomerang boomerang)
    {
        _boomerang = boomerang;
        _projRuntimeData = runtimeData;
        _hitState = _projRuntimeData.GenerateHitState(_hitState);
        
        // _lifetime = boomerang.FlightDuration;

        Vector2 perp = new Vector2(-direction.y, direction.x);
        _peakPos = spawnPos + direction * _boomerang.Range.GetValue(_projRuntimeData.ownedPowerup.CurrentTier) 
                            + perp * _boomerang.ArcWidth.GetValue(_projRuntimeData.ownedPowerup.CurrentTier);

        _originPos = spawnPos;
        _cachedTransform.position = spawnPos;
        // _sr.sprite = bow.ProjectileSprite;

        _t = 0;
        
        gameObject.SetActive(true);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<EnemyRuntime>(out var enemy)) return;

        if (_hitState.CanDealDamage())
        {
            _hitState.DealDamage(enemy);
            return;
        }
        
        _projRuntimeData.ReturnPoolObject(gameObject);
    }
    
    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;
        
        _t += dt / _boomerang.FlightDuration.GetValue(_projRuntimeData.ownedPowerup.CurrentTier);

        _cachedTransform.position = SampleArc(_t);
        _cachedTransform.Rotate(0f, 0f, _boomerang.SpinSpeed * dt);
        
        
        // _lifetime -= Time.fixedDeltaTime;
        if (_t >= 1) _projRuntimeData.ReturnPoolObject(gameObject);
    }
    
    private Vector2 SampleArc(float t)
    {
        Vector2 returnTarget = _playerTransform.position;

        Vector2 a = Vector2.Lerp(_originPos, _peakPos, t);
        Vector2 b = Vector2.Lerp(_peakPos, returnTarget, t);
        return Vector2.Lerp(a, b, t);
    }
}