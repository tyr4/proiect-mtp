using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public static class Animations
{
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
}
