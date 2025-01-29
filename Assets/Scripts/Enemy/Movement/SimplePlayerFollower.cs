using UnityEngine;

namespace Scripts.Map.Movement
{
    public class SimplePlayerFollower : IPlayerFollower
    {
        public Vector3 GetMoveDirection(Vector3 worldPosition, out float sqrDistance)
        {
            var playerGridPos = BigBadSingleton.Instance.GameplayManager.GetPlayerWorldPosition();

            var dir = playerGridPos - worldPosition;
            sqrDistance = dir.sqrMagnitude;

            return dir.normalized;
        }
    }
}