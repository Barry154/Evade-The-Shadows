using UnityEngine;

namespace Map
{
    public class OpenDoorCondition : MonoBehaviour
    {
        public enum Type
        {
            None,
            KillEnemies,
            WaitForSeconds,
            OnVisit
        }

        protected bool isRoomVisited;

        public virtual Type GetType => Type.None;
        public virtual int EnemiesToGo => 0;
        public virtual float SecondsToGo => 0;
        public virtual void RegisterEnemyKill() {}
        public virtual bool HasToAnnounceCompletion => false;
        
        private void Awake()
        {
            ResetCondition();
        }

        public virtual void OnVisit()
        {
            if (!isRoomVisited)
            {
                ResetCondition();
            }
            
            isRoomVisited = true;
        }

        public virtual void ResetCondition() { }

        public virtual bool IsConditionComplete()
        {
            return true;
        }
    }
}