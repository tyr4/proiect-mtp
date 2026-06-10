using Unity.Cinemachine;
using UnityEngine;

public class RunesProjectileRuntime : MonoBehaviour
{
    private RunesRuntime _runtime;
    
    private Transform _cachedTransform;
    private Rigidbody2D _playerRigidbody;
    
    private Camera _camera;
    private Transform _cameraTransform;
    private float _camW;
    private float _camH;

    private Vector2 _baseVelocity;
    private Vector3 _localPos;
    private float _currentSpeed;
    private Vector3 _lastCameraPos;

    private void Awake()
    {
        _camera = Camera.main;
        if (_camera == null)
        {
            throw new System.Exception("n ai camera bos");
        }

        _cameraTransform = _camera.transform;
        _camH = _camera.orthographicSize;
        _camW = _camH * _camera.aspect;
        
        _cachedTransform = transform;
    }

    private void Start()
    {
        _playerRigidbody = Player.Instance.GetComponent<Rigidbody2D>();
    }
    
    private void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    private void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    public void Initialize(RunesRuntime runtime)
    {
        _runtime = runtime;
        _currentSpeed = _runtime.RuntimeData.GetSpeed();
       
        _baseVelocity = Random.insideUnitCircle.normalized * _currentSpeed;
        
        _cachedTransform.SetParent(_cameraTransform);
        _cachedTransform.position = Player.Instance.transform.position;
        _lastCameraPos = _cameraTransform.position;
    }
    
    private void OnCameraUpdated(CinemachineBrain brain)
    {
        var dt = Time.deltaTime;

        _cachedTransform.position += (Vector3)(_baseVelocity * dt);

        // world space bounds
        var camPos = _cameraTransform.position;
        float minX = camPos.x - _camW;
        float maxX = camPos.x + _camW;
        float minY = camPos.y - _camH;
        float maxY = camPos.y + _camH;

        var pos = _cachedTransform.position;
        if (pos.x < minX) { pos.x = minX; _baseVelocity.x =  Mathf.Abs(_baseVelocity.x); }
        if (pos.x > maxX) { pos.x = maxX; _baseVelocity.x = -Mathf.Abs(_baseVelocity.x); }
        if (pos.y < minY) { pos.y = minY; _baseVelocity.y =  Mathf.Abs(_baseVelocity.y); }
        if (pos.y > maxY) { pos.y = maxY; _baseVelocity.y = -Mathf.Abs(_baseVelocity.y); }
        _cachedTransform.position = pos;
    }
    
    public void UpdateSpeed(float newSpeed)
    {
        _baseVelocity *= newSpeed / _currentSpeed;
        _currentSpeed = newSpeed;
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<EnemyRuntime>(out var enemy)) return;
        
        _runtime.RuntimeData.DealDamage(enemy);
    }
}
