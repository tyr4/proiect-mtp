using System;
using DG.Tweening;
using UnityEngine;

public class XPDropRuntime : MonoBehaviour
{
    private XPDrop _data;
    private Transform _cachedTransform;
    private SpriteRenderer _sr;

    private const float DirectionCooldown = 0.15f;
    private float _directionTimer;

    private Vector3 _direction;
    
    public void Initialize(XPDrop data)
    {
        _data = data;
        _cachedTransform = transform;
        _sr =  GetComponent<SpriteRenderer>();
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Player>(out var player)) return;
    }

    public void Tick(float dt, Transform playerTransform)
    {
        _directionTimer += dt;

        if (_directionTimer >= DirectionCooldown)
        {
            _direction = (playerTransform.position - _cachedTransform.position).normalized;
            _directionTimer = 0f;
        }

        _cachedTransform.position += _direction * (_data.MoveSpeed * dt);
    }

    public void Attract()
    {
        XPManager.Instance.RegisterAttracted(this);
    }

    public void Despawn()
    {
        _sr.DOKill();
        
        _sr.DOFade(0f, 0.2f).OnComplete(() =>
        {
            XPManager.Instance.UnregisterAttracted(this);
            XPManager.Instance.ReturnToPool(_data, this);
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
