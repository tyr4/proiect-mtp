using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class FadeAnimation
{
    public static Coroutine FadeIn(SpriteRenderer sr, float duration, MonoBehaviour caller, Action onComplete = null)
    {
        return caller.StartCoroutine(FadeInCoroutine(sr, duration, onComplete));
    }
    
    public static Coroutine FadeOut(SpriteRenderer sr, float duration, MonoBehaviour caller, Action onComplete = null)
    {
        return caller.StartCoroutine(FadeOutCoroutine(sr, duration, onComplete));
    }

    public static Coroutine FadeColorUI(Image target, Color start, Color end, float duration, MonoBehaviour caller, Action onComplete = null)
    {
        return caller.StartCoroutine(FadeColorUICoroutine(target, start, end, duration, onComplete));
    }
    
    public static Coroutine FadeColorUI2(Image target, Color start, Color end, float duration, MonoBehaviour caller, Action onComplete = null)
    {
        return caller.StartCoroutine(FadeColorUICoroutine2(target, start, end, duration, onComplete));
    }
    
    public static Coroutine FadeTint(Image target, Color tint, float duration, MonoBehaviour caller, Action onComplete = null)
    {
        return caller.StartCoroutine(FadeTintCoroutine(target, tint * target.color, duration, onComplete));
    }

    public static Coroutine FadeTintOut(Image target, Color endColor, float duration, MonoBehaviour caller, Action onComplete = null)
    {
        return caller.StartCoroutine(FadeTintCoroutine(target, endColor, duration, onComplete));
    }
    
    private static IEnumerator FadeInCoroutine(SpriteRenderer sr, float duration, Action onComplete)
    {
        float timer = 0f;
        
        Color color = sr.color;
        sr.color = color;
        color.a = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(timer / duration);
            sr.color = color;
            yield return null;
        }
        
        onComplete?.Invoke();
    }
    
    private static IEnumerator FadeOutCoroutine(SpriteRenderer sr, float duration, Action onComplete)
    {
        float timer = 0f;
        
        Color color = sr.color;
        sr.color = color;

        float initialAlpha = color.a;
        duration *= initialAlpha;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Clamp01(initialAlpha - timer / duration);
            sr.color = color;
            
            yield return null;
        }

        onComplete?.Invoke();
    }

    private static IEnumerator FadeColorUICoroutine(Image target, Color start, Color end, float duration, Action onComplete)
    {
        float timer = 0f;
        Vector3 startNoAlpha = new Vector3(start.r, start.g, start.b);
        Vector3 endNoAlpha = new Vector3(end.r, end.g, end.b);
        
        target.color = end;
        Color c = target.color;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            c = Color.Lerp(start, end, timer / duration);
            
            if (startNoAlpha != endNoAlpha)
            {
                if (start.a == 1f)
                    c.a = Mathf.Lerp(0, end.a, timer / duration);
                else if (start.r < end.r && start.g < end.g && start.b < end.b)
                {
                    c.a = Mathf.Lerp(start.a, 0, timer / duration);
                }
            }
            
            target.color = c;
            yield return null;
        }
        
        onComplete?.Invoke();
    }
    
    private static IEnumerator FadeColorUICoroutine2(Image target, Color start, Color end, float duration, Action onComplete)
    {
        float timer = 0f;
        // Vector3 startNoAlpha = new Vector3(start.r, start.g, start.b);
        // Vector3 endNoAlpha = new Vector3(end.r, end.g, end.b);
        
        target.color = end;
        Color c = target.color;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            c = Color.Lerp(start, end, timer / duration);
            
            target.color = c;
            yield return null;
        }
        
        onComplete?.Invoke();
    }
    
    private static IEnumerator FadeTintCoroutine(Image target, Color endColor, float duration, Action onComplete)
    {
        Color startColor = target.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            target.color = Color.Lerp(startColor, endColor, timer / duration);
            yield return null;
        }

        target.color = endColor;
        onComplete?.Invoke();
    }
}