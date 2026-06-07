using DG.Tweening;
using DG.Tweening.Core;
using Unity.Cinemachine;
using UnityEngine;

public static class Animations
{
    public static WaitUntil WaitForAnimationEnd(Animator animator)
    {
        return new WaitUntil(() => 
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            // Debug.Log($"state: {info.fullPathHash}, normalizedTime: {info.normalizedTime}, isTransition: {_animator.IsInTransition(0)}");
            return info.normalizedTime >= 1f && !animator.IsInTransition(0);
        });
    }
    
    
    public static Tween LerpValue(DOGetter<float> getter, DOSetter<float> setter, float startValue, float endValue, float endDuration)
    {
        setter(startValue);
        
        return DOTween.To(
            getter,
            setter,
            endValue,
            endDuration
        ).SetUpdate(true);
    }

    public static Tween LerpTimescale(int start, int end, float duration)
    {
        return LerpValue(
            () => Time.timeScale, 
            x => Time.timeScale = x,
            start, end, duration);
    }

    public static Tween LerpPanelAlpha(CanvasGroup panel, int start, int end, float duration)
    {
        return LerpValue(
            () => panel.alpha, 
            x => panel.alpha = x,
            start, end, duration);
    }

    public static Tween LerpSpriteRendererAlpha(SpriteRenderer sr, int start, int end, float duration)
    {
        return LerpValue(
            () => sr.color.a,
            x => sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, x),
            start, end, duration);
    }

    public static Tween LerpAudioSourceVolume(AudioSource source, float start, float end, float duration)
    {
        return LerpValue(
            () => source.volume,
            x => source.volume = x,
            start, end, duration);
    }
}
