using System;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.UI
{
    public class UIAnimation : MonoBehaviour
    {
        [Serializable]
        public struct SimpleAnimation
        {
            public RectTransform target;
            public Vector2 fromToWidth;
            public Vector2 fromToHeight;
            public AnimationCurve wCurve;
            public AnimationCurve hCurve;
            public float animDuration;

            public UnityEvent OnFinish;

            private float animTimer;

            [HideInInspector] public bool IsComplete;

            public void Play()
            {
                animTimer = 0;
                
                target.transform.localScale = new Vector3(fromToWidth.x, fromToHeight.x, 1);
                
                IsComplete = false;
            }
        
            public void Update(float dt)
            {
                if (animDuration <= 0)
                {
                    return;
                }
            
                animTimer += dt;
                var t = Mathf.Min(animTimer / animDuration, 1);
                var x = Mathf.Lerp(fromToWidth.x, fromToWidth.y, wCurve.Evaluate(t));
                var y = Mathf.Lerp(fromToHeight.x, fromToHeight.y, hCurve.Evaluate(t));
                target.transform.localScale = new Vector3(x, y, 1);

                if (t >= 1)
                {
                    OnFinish?.Invoke();
                    IsComplete = true;
                }
            }
        }

        [SerializeField] SimpleAnimation FadeIn;
        [SerializeField] SimpleAnimation FadeOut;

        SimpleAnimation currentAnimation;
        bool isPlaying = false;


        void Update()
        {
            if (isPlaying)
            {
                currentAnimation.Update(Time.unscaledDeltaTime);
                if (currentAnimation.IsComplete)
                {
                    isPlaying = false;
                }
            }
        }

        private void Play(SimpleAnimation anim)
        {
            currentAnimation = anim;
            anim.Play();
            isPlaying = true;
        }
        
        public void PlayFadeIn()
        {
            Play(FadeIn);
        }

        
        public void PlayFadeOut()
        {
            Play(FadeOut);
        }
    }
    
}