using Scripts.Map.Movement;
using Scripts.Map;
using Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossFight : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private EnemyPalette enemyPalette;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private GameObject bossSpawnPoint;
    [SerializeField] private Transform enemiesContainer;
    [SerializeField] private BackgroundAudio bgAudio;

    public bool stopCoroutines = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameplayManager.stopSpawning = true;
        var bossEnemy = enemyPalette.GetEnemyPrefab(EnemyType.Boss);
        Instantiate(bossEnemy, bossSpawnPoint.transform.position, Quaternion.identity, enemiesContainer);
        bgAudio.playCombatMusic();
        boxCollider.enabled = false;
    }

    private void Update()
    {
        stopCoroutines = gameplayManager.gameIsOver;
    }
}
