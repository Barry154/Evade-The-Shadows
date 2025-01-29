using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public enum EnemyType
    {
        None = 0,
        Crab = 1,
        Ghost = 2,
        Bomber = 3,
        Boss = 4
    }

    [Serializable]
    public class EnemyDictionary : SerializableDictionary<EnemyType, GameObject> { }
    
    [CreateAssetMenu(fileName = "EnemyPalette", menuName = "ScriptableObjects/EnemyPalette", order = 1)]
    public class EnemyPalette : ScriptableObject
    {
        [SerializeField] private EnemyDictionary enemyPrefabs;

        public GameObject GetEnemyPrefab(EnemyType type)
        {
            return enemyPrefabs[type];
        }
    }
}