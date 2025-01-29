using UnityEngine;

namespace Scripts.Player
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private static PlayerSpawnPoint Instance;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Warning: More than one player spawn point in map, ignoring second one...");
                Destroy(gameObject);
            }
            Instance = this;
        }
        
        
    }
}