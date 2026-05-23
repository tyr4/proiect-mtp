using DG.Tweening;
using UnityEngine;

public class EnemyRuntime : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    
    public Enemy Data { get; private set; }
    private IEnemyBehaviour _behaviour;
    public Transform cachedTransform;
    
    private float _health;
    public float Damage;
    private float _movementSpeed;
    
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Material _defaultMaterial;
    
    // dont update the direction every fixedupdate call
    private const float PathfindingRefreshRate = 0.2f;
    private float _timer;
    
    private Vector3 _direction;
    private Vector3 _finalDirection;
    private bool _isDead;

    public Vector2 Velocity => _finalDirection * _movementSpeed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _defaultMaterial = _sr.material;
        cachedTransform = transform;
        // cachedPosition = cachedTransform.position;
    }
    
    public void Initialize(Enemy enemy)
    {
        Data = enemy;
        _health = Data.Health;
        Damage = Data.Damage;
        _movementSpeed = Data.MovementSpeed;
        _isDead = false;

        // apply a random speed multiplier
        _movementSpeed *= Random.Range(0.8f, 1.1f);

        _behaviour = GetComponent<IEnemyBehaviour>();
        _behaviour?.Initialize(this, enemy);
    }
    
    public void Tick(float deltaTime, Transform playerTransform)
    {
        // basic pathfinding global for all enemies
        _timer += deltaTime;

        if (_timer >= PathfindingRefreshRate)
        {
             _direction = playerTransform.position - cachedTransform.position;
            _finalDirection = _direction.normalized;

            _timer = 0;
        }
        
        _rb.linearVelocity = _finalDirection * _movementSpeed;

        if (_direction.x != 0)
        {
            cachedTransform.rotation = Quaternion.Euler(0f, _direction.x < 0 ? 180f : 0f, 0f);
        }
        
        // execute custom logic
        _behaviour.Tick(deltaTime);
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;
        
        _health -= damage;

        if (_health <= 0)
        {
            Kill();
            return;
        }

        TakeDamageAnimation();
    }

    private void Kill()
    {
        if (_isDead) return;
        _isDead = true;
        
        EnemyManager.Instance.Unregister(this);
        
        DieAnimation();
    }

    // TODO: hurt logic here
    private void TakeDamageAnimation()
    {
        _sr.DOKill();
        
        _sr.material = flashMaterial;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            if (!_isDead)
            {
                _sr.material = _defaultMaterial;
            }
        }).SetLink(gameObject);
    }

    private void DieAnimation()
    {
        _sr.material = _defaultMaterial;
        _sr.DOKill();

        _sr.DOFade(0f, 0.15f)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                ResetVisual();
                WaveManager.Instance.ReturnToPool(Data, this);
                XPManager.Instance.SpawnXP(cachedTransform.position);
            });
        
        // WaveManager.Instance.ReturnToPool(Data, this);
        // _sr.DOFade(0f, 0.2f).OnComplete(() => WaveManager.Instance.ReturnToPool(Data, this));
    }

    private void ResetVisual()
    {
        var c = _sr.color;
        c.a = 1f;
        _sr.color = c;

        _sr.material = _defaultMaterial;
    }
    
    private void OnDrawGizmos()
    {
        float cellSize = 2f;

        Vector3 pos = transform.position;

        int x = Mathf.FloorToInt(pos.x / cellSize);
        int y = Mathf.FloorToInt(pos.y / cellSize);

        Vector3 cellCenter = new Vector3(
            x * cellSize + cellSize * 0.5f,
            y * cellSize + cellSize * 0.5f,
            0f
        );

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            cellCenter,
            Vector3.one * cellSize
        );
    }
}
