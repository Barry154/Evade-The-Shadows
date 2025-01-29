using UnityEngine;

namespace Map
{
    public class OpenDoorCondition_OnVisit : OpenDoorCondition
    {
        public override Type GetType => Type.OnVisit;
        public override bool IsConditionComplete()
        {
            return isRoomVisited;
        }
    }
}