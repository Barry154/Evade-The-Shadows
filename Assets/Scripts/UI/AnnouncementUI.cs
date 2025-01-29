using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementUI : MonoBehaviour
{
    [SerializeField] Text announcementLabel;
    [SerializeField] UIAnimation animations;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] private float timeScaleMultiplier = 0.2f;
    
    private Coroutine coroutine;

    private Action onAnnouncementComplete;

    private float duration;

    private bool isPlaying = false;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public void Announce(string text, float duration, Action onAnnouncementComplete)
    {
        announcementLabel.text = text;
        this.duration = duration;
        
        animations.PlayFadeIn();
        
        canvasGroup.alpha = 1;
        
        Time.timeScale = timeScaleMultiplier;

        this.onAnnouncementComplete = onAnnouncementComplete;

        isPlaying = true;
    }

    public void SetText(string text)
    {
        announcementLabel.text = text;
    }

    public void WaitForFadeOut()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(WaitForFadeOutRoutine());
        }
    }
    
    private IEnumerator WaitForFadeOutRoutine()
    {
        yield return new WaitForSecondsRealtime(duration);
        animations.PlayFadeOut();
        coroutine = null;
    }

    void Update()
    {
        if (isPlaying)
        {
            Time.timeScale = timeScaleMultiplier;
        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        onAnnouncementComplete?.Invoke();
        onAnnouncementComplete = null;

        isPlaying = false;
        
        Time.timeScale = 1f;
    }
}
