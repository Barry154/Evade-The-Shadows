using System;
using System.Linq;
using Scripts.Map;
using Scripts.Player;
using Scripts.UI;
using UnityEngine;

namespace Scripts
{
    [RequireComponent(typeof(GameplayManager), typeof(UIManager))]
    public class BigBadSingleton : MonoBehaviour
    {
        [SerializeField] GameplayManager gamePlayManager;
        [SerializeField] UIManager uiManager;
        
        public static BigBadSingleton Instance { get; private set; }
        public GameplayManager GameplayManager => gamePlayManager;
        public UIManager UIManager => uiManager;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            gamePlayManager.LoadSceneMaps();
            gamePlayManager.LoadPlayer();
        }

        private void Reset()
        {
            gamePlayManager = GetComponent<GameplayManager>();
            uiManager = GetComponent<UIManager>();
        }
    }
}