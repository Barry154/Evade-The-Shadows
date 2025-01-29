using System;
using System.Collections.Generic;
using Scripts.Map;
using UnityEngine;

namespace Scripts.Weapons
{
    public enum WeaponType
    {
        None = 0,
        Sword = 1,
        Shield = 2,
    }
    
    [Serializable]
    public class WeaponBase : MonoBehaviour
    {
        [SerializeField] private WeaponType type;
        [SerializeField] private int damage;
        [SerializeField] private float attackSpeed = 1;
        [SerializeField] private float attackRate = 0.2f;
        [SerializeField] private float attackDuration = 0.1f;
        [SerializeField] private float attackDelay = 0.05f;
        [SerializeField] private Rect collisionRect;

        [SerializeField] private ContactFilter2D contactFilter2D;

        private float lastFireTimeStamp = 0;
        private float attackCollisionTimeStampStart = 0;
        private float attackCollisionTimeStampEnd = 0;

        private Collider2D[] hitResults = new Collider2D[10];
        private HashSet<int> currentHits = new HashSet<int>();

        public int Damage => damage;
        
        private Vector3 RectPos => new Vector3(transform.position.x + collisionRect.center.x, 
            transform.position.y + collisionRect.center.y, 
            0.01f);
        
        private Vector3 RectSize => new Vector3(collisionRect.size.x, collisionRect.size.y, 0.01f);

        void FixedUpdate()
        {
            if (Time.time > attackCollisionTimeStampStart &&
                Time.time < attackCollisionTimeStampEnd)
            {
                int hitCount = Physics2D.OverlapBox(RectPos, 
                    RectSize,
                    transform.localRotation.eulerAngles.z,
                    contactFilter2D, 
                    hitResults);
                
                if (hitCount > 0)
                {
                    for (var i = 0; i < hitCount; i++)
                    {
                        var hit = hitResults[i];
                        var hitId = hit.gameObject.GetInstanceID();
                        if (currentHits.Contains(hitId))
                        {
                            continue;
                        }

                        currentHits.Add(hitId);
                        if (hit.TryGetComponent<EnemyCollisionSensor>(out var sensor))
                        {
                            BigBadSingleton.Instance.GameplayManager.DoSlowmoFX(0.1f, 0.00f);
                            BigBadSingleton.Instance.GameplayManager.DoCameraShake();
                            sensor.Hit(this);
                        }
                    }
                }
            }
        }

        public virtual bool CanFire()
        {
            return Time.time > lastFireTimeStamp;
        }

        public virtual bool Fire()
        {
            currentHits.Clear();
            lastFireTimeStamp = Time.time + attackRate;
            attackCollisionTimeStampStart = Time.time + attackDelay;
            attackCollisionTimeStampEnd = attackCollisionTimeStampStart + attackDuration;
            
            return true;
        }

        public virtual float GetAttackSpeed()
        {
            return attackSpeed;
        }
        
        void OnDrawGizmos()
        {
            // Green
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
            DrawRect();
        }
 
        void OnDrawGizmosSelected()
        {
            // Orange
            Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
            DrawRect();
        }
 
        void DrawRect()
        {
            Gizmos.DrawWireCube(RectPos, RectSize);
        }
    }
}