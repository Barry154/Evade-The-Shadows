using System;
using Scripts.Player;
using UnityEngine;

namespace Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] AnnouncementUI announcementUI;
        [SerializeField] UILifeBar lifeBar;
        [SerializeField] UIFadeScreen fadeScreen;
        [SerializeField] UIRoomCompletionInfo roomCompletionInfo;

        PlayerController player;

        public UIFadeScreen FadeScreen => fadeScreen;
        public UIRoomCompletionInfo RoomCompletionInfo => roomCompletionInfo;
        
        void Start()
        {
            player = BigBadSingleton.Instance.GameplayManager.Player;
            lifeBar.SetTotalLife(player.TotalLife);
        }

        public void Announce(string text, float duration, Action callback)
        {
            announcementUI.Announce(text, duration, callback);
        }

        public void SetAnnounceText(string text)
        {
            announcementUI.SetText(text);
        }

        void Update()
        {
            lifeBar.SetLife(player.CurrentLife);
        }
    }
}