using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts
{
    public enum FXType
    {
        Explosion,
        BigExplosion1,
        BigExplosion2,
        BigExplosion3,
        BomberExplosion,
        BossExplosion
    }

    [Serializable]
    public class FXDictionary : SerializableDictionary<FXType, GameObject> { }
    
    [CreateAssetMenu(fileName = "FxPalette", menuName = "ScriptableObjects/FxPalette", order = 2)]
    public class FXPalette : ScriptableObject
    {
        [SerializeField] private FXDictionary fxDictionary;

        public GameObject GetFX(FXType type)
        {
            return fxDictionary[type];
        }

        public GameObject GetRandomBigExplosion()
        {
            switch (Mathf.RoundToInt(Random.value * 2))
            {
                case 0:
                    return GetFX(FXType.BigExplosion1);
                case 1:
                    return GetFX(FXType.BigExplosion2);
                case 2:
                default:
                    return GetFX(FXType.BigExplosion3);
            }
        }

        public GameObject GetBomberExplosion()
        {
            return GetFX(FXType.BomberExplosion);
        }

        public GameObject GetBossExplosion()
        {
            return GetFX(FXType.BossExplosion);
        }
    }
}