using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UIHeartContainer : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private float fillLerpSpeed = 5;
        [SerializeField] private float fillRangeMin = 0.277f;
        [SerializeField] private float fillRangeMax = 0.735f;

        private float targetFill = 0;

        private float GetRemappedFill(float fill) => Mathf.Lerp(fillRangeMin, fillRangeMax, fill);

        public void SetFill(float amount)
        {
            targetFill = GetRemappedFill(amount);
        }

        private void Update()
        {
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * fillLerpSpeed);
        }
    }
}