using UnityEngine;

namespace Map
{
    public class OpenDoorCondition_SecondsToOpen : OpenDoorCondition
    {
        [SerializeField] private float secondsToOpen;

        public override Type GetType => Type.WaitForSeconds;
        public override float SecondsToGo => timer;
        
        public override bool HasToAnnounceCompletion => true;

        private float timer;

        public override void ResetCondition()
        {
            timer = secondsToOpen;
        }

        void FixedUpdate()
        {
            if (isRoomVisited)
            {
                timer -= Time.fixedDeltaTime;
            }
        }

        public override bool IsConditionComplete()
        {
            return timer <= 0;
        }
    }
}