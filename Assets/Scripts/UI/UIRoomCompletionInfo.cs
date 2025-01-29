using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UIRoomCompletionInfo : MonoBehaviour
    {
        public enum InfoType
        {
            None,
            KillCount,
            Survive
        }
        
        [Header("KillCount")] 
        [SerializeField] RectTransform killCountContainer;
        [SerializeField] Text killCountLabel;
        [SerializeField] private UIAnimation killCountAnim;
        [Header("Survive")] 
        [SerializeField] RectTransform surviveContainer;
        [SerializeField] Text surviveTimerLabel;
        [SerializeField] private UIAnimation surviveAnim;

        InfoType currentInfo;

        void Start()
        {
            killCountContainer.gameObject.SetActive(false);
            surviveContainer.gameObject.SetActive(false);
        }

        public void Hide()
        {
            switch (currentInfo)
            {
                case InfoType.Survive:
                    surviveAnim.PlayFadeOut();
                    break;
                case InfoType.KillCount:
                    killCountAnim.PlayFadeOut();
                    break;
            }
        }

        public void ShowType(InfoType type)
        {
            switch (type)
            {
                case InfoType.Survive:
                    surviveContainer.gameObject.SetActive(true);
                    surviveTimerLabel.text = "0:00";
                    surviveAnim.PlayFadeIn();
                    break;
                case InfoType.KillCount:
                    killCountContainer.gameObject.SetActive(true);
                    killCountLabel.text = "x0";
                    killCountAnim.PlayFadeIn();
                    break;
            }

            currentInfo = type;
        }

        public void SetTimer(float seconds)
        {
            var s = Mathf.Max(Mathf.FloorToInt(seconds % 60), 0);
            var m = Mathf.Max(Mathf.FloorToInt(seconds / 60), 0);
            surviveTimerLabel.text = $"{m}:{s:D2}";
        }

        public void SetKillCount(int total)
        {
            killCountLabel.text = $"x{total}";
        }
    }
}