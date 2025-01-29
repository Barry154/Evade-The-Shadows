using System.Collections;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;

namespace Scripts.UI
{
    public class UILifeBar : MonoBehaviour
    {
        [Header("Settings")] [SerializeField, Range(1, 5)]
        private int heartLifePoints = 3;

        [Header("Components")] [SerializeField]
        private UIHeartContainer heartContainerPrefab;

        private List<UIHeartContainer> heartContainers = new List<UIHeartContainer>();

        private int cachedLife = -1;

        public void SetTotalLife(int totalLife)
        {
            heartContainers.Clear();
            for (var i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            int heartCount = Mathf.Max(Mathf.CeilToInt(totalLife / (float)heartLifePoints), 1);
            for (var i = 0; i < heartCount; i++)
            {
                heartContainers.Add(Instantiate(heartContainerPrefab, transform));
            }

            cachedLife = -1;
        }

        public void SetLife(int currentLife)
        {
            if (cachedLife == currentLife)
            {
                return;
            }
            
            for (int i = 0; i < heartContainers.Count; i++)
            {
                float currentAmount = Mathf.InverseLerp(0, heartLifePoints, currentLife);
                heartContainers[i].SetFill(currentAmount);
                currentLife -= heartLifePoints;
            }

            cachedLife = currentLife;
        }
    }
}
