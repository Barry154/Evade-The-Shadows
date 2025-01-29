using System;
using System.Collections.Generic;
using System.Linq;
using Map;
using Scripts.Player;
using Scripts.UI;
using SuperTiled2Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Map
{
    public class MapEntry : MonoBehaviour
    {
        [Serializable]
        public struct MapXDoor
        {
            public MapEntry map;
            public DoorController door;
        }

        [Serializable]
        public struct EnemySpawnInfo
        {
            public EnemyType type;
            public float chance;
        }

        private static int GlobalIdIndex = 0;
        
        public int Id { get; private set; }
        [SerializeField] private SuperMap map;
        [SerializeField] private Grid grid;
        public OpenDoorCondition OpenDoorCondition;

        [Header("Announcement Text")]
        [SerializeField] string announcementText;

        [Header("Settings")]
        [SerializeField] EnemySpawnInfo[] spawnInfos;
        [SerializeField] private bool preSpawnEnemies = false;
        [SerializeField] private bool keepSpawningEnemies = false;
        [SerializeField] private float spawnInterval = 2;
        [SerializeField] private int maxEnemiesAtOnce = 100;
        [SerializeField] private bool stopSpawnWhenDoorsOpen = true;

        public MapXDoor[] adjacentMaps;

        private DoorController[] doors;
        public EnemySpawnPoint[] spawnPoints;
        private PlayerSpawnPoint playerSpawnPoint;
        private GameObject enemiesContainer;

        private int enemySpawnIndex;
        private List<int> enemySpawnOrder;

        private const short mapMinPosX = 0;
        private const short mapMinPosY = -1;

        private bool hasOpenDoorsCondition;
        private bool isOpenDoorConditionComplete;

        private float lastSpawnTimeStamp;

        public bool IsVisited { get; private set;  }

        public void Awake()
        {
            Id = GlobalIdIndex;
            GlobalIdIndex++;

            spawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
            playerSpawnPoint = GetComponentInChildren<PlayerSpawnPoint>();
            doors = GetComponentsInChildren<DoorController>();

            hasOpenDoorsCondition = OpenDoorCondition != null;
            if (!hasOpenDoorsCondition)
            {
                foreach (var door in doors)
                {
                    door.Toggle(true);
                }
            }

            enemySpawnOrder = Enumerable.Range(0, spawnPoints.Length).ToList();
            enemySpawnOrder.Sort((a, b) => Random.value.CompareTo(Random.value));

            enemiesContainer = new GameObject("EnemiesContainer");
            enemiesContainer.transform.SetParent(transform);

            spawnInfos = spawnInfos.OrderBy(x => x.chance).ToArray();
        }

        public void RegisterEnemyKill()
        {
            if (hasOpenDoorsCondition)
            {
                OpenDoorCondition.RegisterEnemyKill();
                BigBadSingleton.Instance.UIManager.RoomCompletionInfo.SetKillCount(OpenDoorCondition.EnemiesToGo);
            }
        }

        public void Visit()
        {
            if (!IsVisited)
            {
                if (preSpawnEnemies)
                {
                    foreach (var spawnPoint in spawnPoints)
                    {
                        SpawnEnemy(ChooseRandomEnemyType(), spawnPoint.transform.position);
                    }
                }

                if (hasOpenDoorsCondition)
                {
                    OpenDoorCondition.OnVisit();
                }
            }

            UpdateRoomUI();
            IsVisited = true;
        }

        void SpawnEnemy(EnemyType enemyType, Vector3 pos)
        {
            if (enemyType == EnemyType.None)
            {
                Debug.LogError("Can't spawn enemy of type None!");
                return;
            }

            var enemyGameObject = BigBadSingleton.Instance.GameplayManager.SpawnEnemy(enemyType, pos, null);
            var crab = enemyGameObject.GetComponent<CrabController>();
            if (crab)
            {
                crab.SetRoom(this);
                return;
            }
            var ghost = enemyGameObject.GetComponent<GhostBehaviour>();
            if (ghost)
            {
                ghost.SetRoom(this);
            }
        }

        void UpdateRoomUI()
        {
            if (hasOpenDoorsCondition)
            {
                switch (OpenDoorCondition.GetType)
                {
                    case OpenDoorCondition.Type.None:
                    case OpenDoorCondition.Type.OnVisit:
                        BigBadSingleton.Instance.UIManager.RoomCompletionInfo.ShowType(UIRoomCompletionInfo.InfoType.None);
                        break;
                    case OpenDoorCondition.Type.KillEnemies:
                        BigBadSingleton.Instance.UIManager.RoomCompletionInfo.ShowType(UIRoomCompletionInfo.InfoType.KillCount);
                        break;
                    case OpenDoorCondition.Type.WaitForSeconds:
                        BigBadSingleton.Instance.UIManager.RoomCompletionInfo.ShowType(UIRoomCompletionInfo.InfoType.Survive);
                        break;
                }
                BigBadSingleton.Instance.UIManager.RoomCompletionInfo.SetTimer(OpenDoorCondition.SecondsToGo);
                BigBadSingleton.Instance.UIManager.RoomCompletionInfo.SetKillCount(OpenDoorCondition.EnemiesToGo);
            }
        }

        void Update()
        {
            if (!IsVisited)
            {
                return;
            }

            if (hasOpenDoorsCondition && !isOpenDoorConditionComplete)
            {
                BigBadSingleton.Instance.UIManager.RoomCompletionInfo.SetTimer(OpenDoorCondition.SecondsToGo);

                if (OpenDoorCondition.IsConditionComplete())
                {
                    foreach (var adjacentMap in adjacentMaps)
                    {
                        if (adjacentMap.door)
                        {
                            adjacentMap.door.Toggle(true);
                            adjacentMap.map.gameObject.SetActive(true);
                        }
                    }

                    isOpenDoorConditionComplete = true;
                    BigBadSingleton.Instance.UIManager.RoomCompletionInfo.Hide();
                    if (OpenDoorCondition.HasToAnnounceCompletion)
                    {
                        BigBadSingleton.Instance.UIManager.Announce("Complete!", 2f, null);
                    }
                }
            }

            if (keepSpawningEnemies &&
                (!stopSpawnWhenDoorsOpen || !isOpenDoorConditionComplete))
            {
                if (enemiesContainer.transform.childCount < maxEnemiesAtOnce && Time.time > lastSpawnTimeStamp)
                {
                    SpawnRandomEnemy();
                    lastSpawnTimeStamp = Time.time + spawnInterval;
                }
            }
        }

        public void SpawnRandomEnemy()
        {
            if (spawnPoints.Length <= 0)
            {
                Debug.LogWarning("Trying to spawn enemy but there is no spawn point available!");
                return;
            }

            int currentEnemyPointIndex = enemySpawnOrder[enemySpawnIndex];

            var point = spawnPoints[currentEnemyPointIndex];
            SpawnEnemy(ChooseRandomEnemyType(), point.transform.position);

            enemySpawnIndex = (enemySpawnIndex + 1) % spawnPoints.Length;
            if (enemySpawnIndex == 0)
            {
                enemySpawnOrder.Sort((a, b) => Random.value.CompareTo(Random.value));
            }
        }

        EnemyType ChooseRandomEnemyType()
        {
            float totalChance = 0;
            foreach (var info in spawnInfos)
            {
                totalChance += info.chance;
            }

            if (totalChance == 0)
            {
                return EnemyType.None;
            }

            float chanceAccumulative = 0;
            float randomValue = Random.value;
            foreach (var info in spawnInfos)
            {
                chanceAccumulative += info.chance / totalChance;
                if (chanceAccumulative > randomValue)
                {
                    return info.type;
                }
            }

            return EnemyType.None;
        }

        public bool TryGetPlayerSpawnPosition(out Vector3 position)
        {
            if (playerSpawnPoint != null && IsWorldPosInsideMap(playerSpawnPoint.transform.position, out var gridPos))
            {
                position = GetWorldPosAtCenterOfGridPos(gridPos);
                return true;
            }

            position = Vector3.zero;
            return false;
        }

        // Spawn an enemy at a random spawn-point
        public bool TryGetRandomSpawnPosition(out Vector3 position)
        {
            if (spawnPoints.Length > 0)
            {
                var randomPoint = spawnPoints[Mathf.RoundToInt(Random.value * (spawnPoints.Length - 1))]
                    .transform.position;
                if (IsWorldPosInsideMap(randomPoint, out var gridPos))
                {
                    position = GetWorldPosAtCenterOfGridPos(gridPos);
                    return true;
                }
            }

            position = Vector3.zero;
            return false;
        }

        // Spawn enemies at a specific spawn point (used in GameplayManager to spawn an enemy at each spawn-point at once)
        public bool TryGetSpawnPosition(int spawnPoint, out Vector3 position)
        {
            if (spawnPoints.Length > 0)
            {
                if (IsWorldPosInsideMap(spawnPoints[spawnPoint].transform.position, out var gridPos))
                {
                    position = GetWorldPosAtCenterOfGridPos(gridPos);
                    return true;
                }
            }

            position = Vector3.zero;
            return false;
        }

        public bool IsWorldPosInsideMap(Vector3 worldPos, out Vector3Int gridPos)
        {
            gridPos = grid.WorldToCell(worldPos);
            
            return gridPos.x >= mapMinPosX &&
                   gridPos.y <= mapMinPosY &&
                   gridPos.x <= (map.m_Width - 1) &&
                   gridPos.y >= -map.m_Height;
        }

        public Vector3 GetWorldPosAtCenterOfGridPos(Vector3Int gridPos)
        {
            return grid.GetCellCenterWorld(gridPos);
        }

        public void ToggleAdjacentMaps(bool isActive)
        {
            foreach (var adjacentMap in adjacentMaps)
            {
                if (adjacentMap.door && !adjacentMap.door.IsOpen)
                {
                    adjacentMap.map.gameObject.SetActive(false);
                }
                else
                {
                    adjacentMap.map.gameObject.SetActive(isActive);
                }
            }
        }

        public bool TryGetAnnouncementText(out string text)
        {
            text = announcementText;
            return !string.IsNullOrEmpty(text);
        }
    }
}