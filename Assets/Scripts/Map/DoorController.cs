using UnityEngine;

namespace Map
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] Transform visual;
        [SerializeField] BoxCollider2D collider;
        
        public bool IsOpen { get; private set; }

        public void Toggle(bool isOpen)
        {
            IsOpen = isOpen;
            gameObject.SetActive(!isOpen);
        }
    }
}