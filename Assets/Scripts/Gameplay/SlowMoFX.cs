using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class SlowMoFX : MonoBehaviour
    {
        private Coroutine coroutine;

        private float originalTimeScale;
        private float delay;
        private float duration;
        private float frames;
    
        public void StartDelay(float originalTimeScale, float delay, float duration, float frames)
        {
            if (!BigBadSingleton.Instance.GameplayManager.DoSlowMoFx)
            {
                return;
            }
            
            this.delay = delay;
            this.duration = Mathf.Max(duration, 0.01f);
            this.frames = Mathf.Max(frames, 1);
            
            if (coroutine == null)
            {
                this.originalTimeScale = originalTimeScale;
                coroutine = StartCoroutine(SlomoRoutine());
            }
            else
            {
                StopCoroutine(coroutine);
                coroutine = StartCoroutine(SlomoRoutine());
            }
        }
    
        private IEnumerator SlomoRoutine()
        {
            Time.timeScale = 0;

            yield return new WaitForSecondsRealtime(delay);

            while (Time.timeScale < originalTimeScale && !gameObject.GetComponent<GameplayManager>().gameIsOver)
            {
                yield return new WaitForSecondsRealtime(duration);
                Time.timeScale = originalTimeScale;
            }
        }
    }
}