using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Map;
using Scripts.Player;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
//using static System.Net.Mime.MediaTypeNames;

namespace Scripts
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private Transform debugGridPositionImage;
        [SerializeField] private EnemyPalette enemyPalette;
        [SerializeField] private FXPalette fxPalette;
        [SerializeField] private SlowMoFX slowMoFX;
        [SerializeField] private ProtoCameraController cameraController;
        [SerializeField] private PlayerController playerPrefab;
        //[SerializeField] private BossController bossPrefab;

        [Header("Containers")]
        [SerializeField] private Transform fxContainer;
        [SerializeField] private Transform enemiesContainer;
        [SerializeField] private Transform temporaryObjectsContainer;

        [Header("Settings")]
        public bool DoSlowMoFx = true;

        [Header("UI")]
        [SerializeField] private UILifeBar lifebar;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Spawn variables for enemy wave spawning
        [Header("Enemy Wave Spawn Perameters")]
        [SerializeField] public bool stopSpawning;
        [SerializeField] private float spawnTimer;
        [SerializeField] private float spawnDelay;

        // GameObject which controls background audio
        [Header("Background Audio Object/Script")]
        [SerializeField] private BackgroundAudio bgAudio;

        //Trigger Audio and UI events
        [Header("Events")]
        [SerializeField] private UnityEvent GameOver;
        [SerializeField] private UnityEvent GameWon;
        public bool gameIsOver = false;
        private bool gameIsWon = false;

        // Array of enemies
        //private List<GameObject> enemies;
        private GameObject[] enemies;

        // Wave and enemy type variables
        private int waveNumber = 0;
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private PlayerController player;
        private MapEntry[] allMapsInScene = Array.Empty<MapEntry>();
        private MapEntry currentMap = null;

        private const int s_targetFrameRate = 60;

        private float lastPlayerGridPosRequestTimeStamp = 0;
        private Vector3Int cachedPlayerGridPos;

        private float lastPlayerPosRequestTimeStamp = 0;
        private Vector3 cachedPlayerPos;

        public Rigidbody2D PlayerRigidBody => player.Rigidbody;

        public PlayerController Player => player;

        private bool fadeInComplete = false;

        private int totalChestsOpened = 0; 
        private const int totalChestCount = 3; 

        public void OnChestOpened()
        {
            totalChestsOpened++;
            if (totalChestsOpened <= totalChestCount)
            {
                string announcementMessage = $"Well done, you have {totalChestsOpened} out of {totalChestCount} keys!";
                BigBadSingleton.Instance.UIManager.Announce(announcementMessage, 3f, () => { });

                // Check if all keys have been collected
                if (totalChestsOpened == totalChestCount)
                {
                    StartCoroutine(AnnounceBossFight());
                }
            }

            // Play 'puzzle complete' sound
            bgAudio.playKeyCollected();
        }

        public void OnHintInteraction(string text)
        {
            BigBadSingleton.Instance.UIManager.Announce(text, 3f, () => { });

            // Check if all keys have been collected
            if (totalChestsOpened == totalChestCount)
            {
                StartCoroutine(AnnounceBossFight());
            }
        }

        IEnumerator AnnounceBossFight()
        {
            yield return new WaitForSeconds(2.5f); // Annoucement delay
            BigBadSingleton.Instance.UIManager.Announce("Go fight the boss!", 2f, () => { });

            // Play door unlock sound
            bgAudio.playBossDoorOpens();
        }

        void Awake()
        {
            Application.targetFrameRate = s_targetFrameRate;
        }

        public void LoadSceneMaps()
        {
            allMapsInScene = FindObjectsOfType<MapEntry>();
        }

        public void LoadPlayer()
        {
            var playerController = FindObjectsOfType<PlayerController>().FirstOrDefault();
            if (!playerController)
            {
                var playerSpawnPoint = FindObjectsOfType<PlayerSpawnPoint>().FirstOrDefault();
                if (playerSpawnPoint != null)
                {
                    playerController = Instantiate(playerPrefab,
                        playerSpawnPoint.transform.position,
                        Quaternion.identity,
                        transform);
                }
            }

            Assert.IsTrue(playerController != null, "Couldn't find player instance or PlayerSpawner!");
            SetupPlayer(playerController);

            cameraController.SetTarget(playerController.gameObject);
        }

        private void SetupPlayer(PlayerController playerController)
        {
            player = playerController;
            player.Init();

            InitCurrentMap();
        }

        public Vector3Int GetGridPosition(Vector3 worldPos, out int mapId)
        {
            mapId = 0;
            foreach (var mapEntry in allMapsInScene)
            {
                if (mapEntry.IsWorldPosInsideMap(worldPos, out var gridPos))
                {
                    mapId = mapEntry.Id;
                    return gridPos;
                }
            }

            return Vector3Int.zero;
        }

        public Vector3Int GetPlayerGridPosition()
        {
            if (lastPlayerGridPosRequestTimeStamp < Time.time)
            {
                lastPlayerGridPosRequestTimeStamp = Time.time;
                cachedPlayerGridPos = GetGridPosition(GetPlayerWorldPosition(), out _);
            }
            return cachedPlayerGridPos;
        }

        public Vector3 GetPlayerWorldPosition()
        {
            if (lastPlayerPosRequestTimeStamp < Time.time)
            {
                lastPlayerPosRequestTimeStamp = Time.time;
                cachedPlayerPos = player.transform.position;
            }
            return cachedPlayerPos;
        }

        public void Debug_FocusWorldPositionInGrid(Vector3 worldPos, bool logPos)
        {
            foreach (var mapEntry in allMapsInScene)
            {
                if (mapEntry.IsWorldPosInsideMap(worldPos, out var gridPos))
                {
                    var gridCenterInWorldPos = mapEntry.GetWorldPosAtCenterOfGridPos(gridPos);
                    debugGridPositionImage.position = gridCenterInWorldPos;
                    if (logPos)
                    {
                        Debug.Log(gridPos);
                    }
                    mapEntry.gameObject.SetActive(true);
                }
                else
                {
                    mapEntry.gameObject.SetActive(false);
                }
            }
        }

        public Vector3 getGridCenterInWorldPos(Vector3 characterPos)
        {
            Vector3 gridCenterInWorldPos = new Vector3();

            foreach (var mapEntry in allMapsInScene)
            {
                if (mapEntry.IsWorldPosInsideMap(characterPos, out var gridPos))
                {
                    gridCenterInWorldPos = mapEntry.GetWorldPosAtCenterOfGridPos(gridPos);
                    mapEntry.gameObject.SetActive(true);
                }
                else
                {
                    mapEntry.gameObject.SetActive(false);
                }
            }

            return gridCenterInWorldPos;
        }

        void InitCurrentMap()
        {
            foreach (var mapEntry in allMapsInScene)
            {
                if (mapEntry.IsWorldPosInsideMap(player.transform.position, out var gridPos))
                {
                    mapEntry.gameObject.SetActive(true);
                    SetCurrentMap(mapEntry);
                }
                else
                {
                    mapEntry.gameObject.SetActive(false);
                }
            }

            currentMap.ToggleAdjacentMaps(true);
        }

        void UpdateCurrentMap()
        {
            if (!currentMap.IsWorldPosInsideMap(player.transform.position, out _))
            {
                foreach (var mapEntry in currentMap.adjacentMaps)
                {
                    if (mapEntry.map.IsWorldPosInsideMap(player.transform.position, out _))
                    {
                        currentMap.ToggleAdjacentMaps(false);
                        SetCurrentMap(mapEntry.map);
                    }
                }
            }
        }

        void SetCurrentMap(MapEntry nextMap)
        {
            currentMap = nextMap;
            currentMap.gameObject.SetActive(true);
            currentMap.ToggleAdjacentMaps(true);

            if (fadeInComplete)
            {
                if (!currentMap.IsVisited && currentMap.TryGetAnnouncementText(out var text))
                {
                    BigBadSingleton.Instance.UIManager.Announce(text, 3f, currentMap.Visit);
                }
                else
                {
                    currentMap.Visit();
                }
            }
        }

        public void SpawnFXAt(FXType fx, Vector3 worldPos)
        {
            Instantiate(fxPalette.GetFX(fx), worldPos, Quaternion.identity, fxContainer);
        }
        
        public void SpawnBigExplosionAt(Vector3 worldPos)
        {
            Instantiate(fxPalette.GetRandomBigExplosion(), worldPos, Quaternion.identity, fxContainer);
        }

        public void SpawnBomberExplosionAt(Vector3 worldPos)
        {
            Instantiate(fxPalette.GetBomberExplosion(), worldPos, Quaternion.identity, fxContainer);
        }

        public void SpawnBossExplosionAt(Vector3 worldPos)
        {
            Instantiate(fxPalette.GetBossExplosion(), worldPos, Quaternion.identity, fxContainer);
            gameIsWon = true;
        }

        // Checks if the current 'state of the game' (is the player dead, is the game paused, is the player in combat, etc.)
        // This is to trigger events which control background music play and possibly UI changes
        void CheckGameState()
        {
            // Player dies and Game Over
            if (player.CurrentLife <= 0)
            {
                CancelInvoke("SpawnEnemies");
                GameOver?.Invoke();
                Time.timeScale = 0.0f;
                gameIsOver = true;
            }

            // Boss dies and Game Won
            else if (gameIsWon)
            { 
                CancelInvoke("SpawnEnemies");
                GameWon?.Invoke();
                Time.timeScale = 0.0f;
                gameIsOver = true;
            }
        }

        // Spawns enemies into the scene and plays the combat music
        public void EnemyWaveSpawner()
        {
            if(bgAudio.combatMusic.isPlaying == false)
            {
                //bgAudio.combatMusic.volume = 1.0f;
                bgAudio.playCombatMusic();
            }

            // Spawn multiple enemies
            for (int i = 0; i < currentMap.spawnPoints.Length; i++)
            {
                if (currentMap.TryGetSpawnPosition(i, out var position))
                {
                    // 1st Wave
                    if (waveNumber == 1)
                    {
                        var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Crab);
                        Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);

                        if (i > 2)
                        {
                            break;
                        }
                    }


                    // 2nd Wave
                    else if (waveNumber == 2)
                    {
                        if (i == 0 || i == 1 || i == 4)
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Bomber);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }
                        else
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Ghost);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }

                        if (i > 2)
                        {
                            break;
                        }
                    }

                    // 3rd Wave
                    else if (waveNumber == 3)
                    {
                        if (i % 2 == 0)
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Ghost);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }
                        else
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Crab);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }

                        if (i > 4)
                        {
                            break;
                        }
                    }

                    else if (waveNumber == 4)
                    {
                        if (i % 2 == 0)
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Bomber);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }
                        else
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Crab);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }

                        if (i > 6)
                        {
                            break;
                        }
                    }

                    // All other waves
                    else
                    {
                        if (i % 2 == 0)
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Ghost);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }
                        else if (i % 3 == 0)
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Bomber);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }
                        else
                        {
                            var newEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Crab);
                            Instantiate(newEnemy, position, Quaternion.identity, enemiesContainer);
                        }
                    }
                }
            }
        }

        // Playes wave incoming SFX, then delays the spawning of the enemies into the scene
        public void SpawnEnemies()
        {
            if (bgAudio.combatMusic.isPlaying == false)
            {
                bgAudio.WaveAlertSound();
            }

            waveNumber++;
            //print(waveNumber);

            // Delay the spawning of enemies until the count-down is complete
            Invoke("EnemyWaveSpawner", 4.25f);
        }

        private void Start()
        {

            // Play background music at game startup
            bgAudio.playBackgroundMusic();

            // Conditionals to check if enemies must be spawned given a certain time interval
            if (!stopSpawning)
            {
                InvokeRepeating("SpawnEnemies", spawnTimer, spawnDelay);
            }

            BigBadSingleton.Instance.UIManager.RoomCompletionInfo.ShowType(UIRoomCompletionInfo.InfoType.None);

            Time.timeScale = 0;
            fadeInComplete = false;

            string roomText;
            if (currentMap.TryGetAnnouncementText(out roomText))
            {
                BigBadSingleton.Instance.UIManager.SetAnnounceText(roomText);
            }

            BigBadSingleton.Instance.UIManager.FadeScreen.DoFadeIn(1f, 0.4f, () =>
            {
                BigBadSingleton.Instance.UIManager.Announce(roomText, 3f, () =>
                {
                    currentMap.Visit();
                });
                fadeInComplete = true;
            });
        }

        // Felipe's enemy spawn code
        public T SpawnTemporaryObject<T>(T prefab, Vector3 origin) where T : MonoBehaviour
        {
            return Instantiate<T>(prefab, origin, Quaternion.identity, temporaryObjectsContainer);
        }

        public GameObject SpawnEnemy(EnemyType type, Vector3 worldPos, Transform parent = null)
        {
            var newEnemy = enemyPalette.GetEnemyPrefab(type);
            if (parent == null)
            {
                parent = enemiesContainer;
            }
            var instance = Instantiate(newEnemy, worldPos, Quaternion.identity, enemiesContainer);
            return instance;
        }

        void Update()
        {
            lifebar.SetLife(player.CurrentLife);

            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Check if enemy wave has been cleared and play background music
            if(enemies.Length == 0 && bgAudio.outOfCombatMusic.isPlaying == false)
            {
                //float startingVol = bgAudio.combatMusic.volume;

                //try to fade out combat music (needs ammending)
                //while (bgAudio.combatMusic.volume > 0)
                //{
                //    bgAudio.combatMusic.volume -= startingVol - Time.deltaTime / 5;
                //}

                bgAudio.playBackgroundMusic();
            }

            UpdateCurrentMap();

            if (stopSpawning)
            {
                CancelInvoke("SpawnEnemies");
            }

            CheckGameState();

            if (gameIsOver)
            {
                this.enabled = false;
            }
        }

        public void DoSlowmoFX(float delay, float duration)
        {
            slowMoFX.StartDelay(Time.timeScale, delay, duration, 1);
        }

        public void DoCameraShake()
        {
            cameraController.DoShake();
        }
    }
}