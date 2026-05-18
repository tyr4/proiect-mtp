using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class XPDropRuntime : MonoBehaviour
{
    public XPDrop Data;
    private Transform _cachedTransform;
    private SpriteRenderer _sr;
    
    public BoxCollider2D Collider;

    private const float DirectionCooldown = 0.15f;
    private float _directionTimer;

    private Vector3 _direction;
    private bool _isAttracted;

    private void Awake()
    {
        _sr =  GetComponent<SpriteRenderer>();
        Collider = GetComponent<BoxCollider2D>();
        _cachedTransform = transform;
    }
    
    public void Initialize(XPDrop data)
    {
        Data = data;
        _direction = Vector3.zero;
        
        Collider.enabled = true;
        
        if (_isAttracted)
        {
            XPManager.Instance.UnregisterAttracted(this);
        }
        
        _isAttracted = false;
    }
    
    // public void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (!other.TryGetComponent<Player>(out var player)) return;
    // }

    public void Tick(float dt, Transform playerTransform)
    {
        _directionTimer += dt;

        if (_directionTimer >= DirectionCooldown)
        {
            var playerPos = playerTransform.position;
            var cachedPos = _cachedTransform.position;
            
            // _cachedTransform.DOKill();
            
            _direction = (playerPos - cachedPos).normalized;
            // var distance = Vector3.Distance(cachedPos, playerPos);
            // var duration = distance / Data.MoveSpeed;
            //
            // _cachedTransform.DOMove(playerPos, duration);
            
            _directionTimer = 0f;
            
        }

        _cachedTransform.position += _direction * (Data.MoveSpeed * dt);
    }

    public void Attract()
    {
        if (_isAttracted) return;
        
        _isAttracted = true;
        XPManager.Instance.RegisterAttracted(this);
        _directionTimer = DirectionCooldown;
    }

    public void Despawn()
    {
        _sr.DOKill();
        XPManager.Instance.UnregisterAttracted(this);
        
        _sr.DOFade(0f, 0.2f).OnComplete(() =>
        {
            XPManager.Instance.ReturnToPool(Data, this);
            ResetVisual();
        });
    }

    private void ResetVisual()
    {
        var c = _sr.color;
        c.a = 1f;
        _sr.color = c;
    }
}
