using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float floatSpeed;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float lifetime;

    private TextMeshPro _tmp;
    private Transform _cachedTransform;
    private float _timer;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshPro>();
        _cachedTransform = transform;
    }

    public void Initialize(Transform spawnPos, float value)
    {
        _cachedTransform.position = spawnPos.position;

        _tmp.text = $"{value}";
        _tmp.alpha = 1;
        _timer = 0;
        
        _tmp.DOFade(0f, lifetime).SetLink(gameObject);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        _cachedTransform.position += Vector3.up * (floatSpeed * Time.deltaTime);
        
        if (_timer >= lifetime)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        DamageNumberManager.Instance.ReturnToPool(this, gameObject);
    }
}
