using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class OnDeathParticles : MonoBehaviour
{
    private Transform _cachedTransform;
    private ParticleSystem _ps;

    private void Awake()
    {
        _cachedTransform = transform;
        _ps = GetComponent<ParticleSystem>();
    }

    public void PlayAnimation(Transform spawnPos)
    {
        gameObject.SetActive(true);
        _cachedTransform.position = spawnPos.position;
        _ps.Play();
        
        StartCoroutine(WaitForParticleAnimation(() =>
        {
            gameObject.SetActive(false);
            DeathParticleManager.Instance.ReturnToPool(gameObject);
        }));
    }

    private IEnumerator WaitForParticleAnimation(Action onComplete = null)
    {
        yield return new WaitUntil(() => !_ps.isPlaying);
        
        onComplete?.Invoke();
    }
}
