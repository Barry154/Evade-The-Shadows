using Scripts.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Map
{
    public class EnemyCollisionSensor : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponHitEvent : UnityEvent<WeaponBase> { }
        
        [Header("Events")]
        [SerializeField] WeaponHitEvent OnHitByWeapon;

        public void Hit(WeaponBase weapon)
        {
            OnHitByWeapon?.Invoke(weapon);
        }
    }
}