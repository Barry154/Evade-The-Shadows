using UnityEngine;
using System.Collections;

public class FadingCanvas : MonoBehaviour
{
    public CanvasGroup optionsMenuCanvasGroup;
    public float delayBeforeFadeIn = 2.0f;
    public float fadeInDuration = 1f;
    public float delayBeforeFadeOut = 3f;
    public float fadeOutDuration = 1f;

    // Start is called before first frame update
    void Start()
    {
        // Canvas starts transparent and is not interactable
        optionsMenuCanvasGroup.alpha = 0f;
        optionsMenuCanvasGroup.interactable = false;
        optionsMenuCanvasGroup.blocksRaycasts = false;

        StartCoroutine(FadeInAndOutRoutine());
    }

    IEnumerator FadeInAndOutRoutine()
    {
        // Waiting for the delay before fading in
        yield return new WaitForSeconds(delayBeforeFadeIn);

        // Fades in
        for (float t = 0.0f; t < fadeInDuration; t += Time.deltaTime)
        {
            optionsMenuCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        optionsMenuCanvasGroup.alpha = 1f;
        optionsMenuCanvasGroup.interactable = true;
        optionsMenuCanvasGroup.blocksRaycasts = true;

        // Waiting for another delay before fading out
        yield return new WaitForSeconds(delayBeforeFadeOut);

        // Fades out
        for (float t = 0.0f; t < fadeOutDuration; t += Time.deltaTime)
        {
            optionsMenuCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        optionsMenuCanvasGroup.alpha = 0f;
        optionsMenuCanvasGroup.interactable = false;
        optionsMenuCanvasGroup.blocksRaycasts = false;
    }
}