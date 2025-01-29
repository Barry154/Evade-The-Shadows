using System;
using UnityEngine;

namespace Map
{
    public class OpenDoorCondition_KillEnemies : OpenDoorCondition
    {
        [SerializeField] private int enemiesToGo;

        public override Type GetType => Type.KillEnemies;
        public override int EnemiesToGo => enemyCounter;
        
        public override bool HasToAnnounceCompletion => true;

        int enemyCounter;

        public override void ResetCondition()
        {
            enemyCounter = enemiesToGo;
        }

        public override void RegisterEnemyKill()
        {
            enemyCounter--;
        }

        public override bool IsConditionComplete()
        {
            return enemyCounter <= 0;
        }
    }
}