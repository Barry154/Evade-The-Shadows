using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeScreen : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private AnimationCurve fadeInCurve;
    [SerializeField] private AnimationCurve fadeOutCurve;

    private float timer;
    private AnimationCurve currentCurve;

    private bool isPlaying = false;
    private float start = 0;
    private float target = 1;

    private Coroutine coroutine;

    public void DoFadeIn(float delay, float duration, Action callback)
    {
        fadeImage.fillAmount = 1;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(DoFade(delay, duration, callback, true));
    }
    
    public void DoFadeOut(float delay, float duration, Action callback)
    {
        fadeImage.fillAmount = 0;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(DoFade(delay, duration, callback, false));
    }

    private IEnumerator DoFade(float delay, float duration, Action callback, bool isFadeIn)
    {
        if (duration <= 0)
        {
            Debug.LogError("Can't set duration <= 0!");
            coroutine = null;
            yield break;
        }

        yield return new WaitForSecondsRealtime(delay);
        
        currentCurve = isFadeIn ? fadeInCurve : fadeOutCurve;
        start = isFadeIn ? 1 : 0;
        target = isFadeIn ? 0 : 1;
        timer = 0;

        while (true)
        {
            timer += Time.unscaledDeltaTime;
            float t = currentCurve.Evaluate(timer / duration);
            fadeImage.fillAmount = Mathf.Lerp(start, target, t);
            if (t >= 1)
            {
                callback?.Invoke();
                coroutine = null;
                yield break;
            }

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
