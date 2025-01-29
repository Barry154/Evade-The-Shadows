using UnityEngine;

namespace Scripts.Map.Movement
{
    public enum PlayerFollowerType
    {
        Simple,
    }
    
    public interface IPlayerFollower
    {
        Vector3 GetMoveDirection(Vector3 worldPosition, out float sqrDistance);
    }

    public static class PlayerFollowerTypeExtensions
    {
        public static IPlayerFollower CreateInstance(this PlayerFollowerType type)
        {
            switch (type)
            {
                case PlayerFollowerType.Simple:
                default:
                    return new SimplePlayerFollower();
            }
        }
    }
}