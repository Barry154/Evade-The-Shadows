using System;
using Scripts.Weapons;
using UnityEngine;

namespace Scripts.Map
{
    public class EnemyProjectile : WeaponProjectile
    {
        [Header("Player following settings")] 
        [SerializeField] private bool followPlayerPos;
        [SerializeField, Range(0, 1)] private float followStrength;
        [SerializeField, Range(0, 1)] private float launchFuturePosWeight;
        [SerializeField] private float maxDuration = -1f;
        
        private Rigidbody2D playerRigidBody;

        private float dieTimeStamp;

        public void Fire(Rigidbody2D playerRigidBody)
        {
            this.playerRigidBody = playerRigidBody;

            Vector3 playerCurrentPos = playerRigidBody.transform.position;
            Vector3 playerFuturePos = playerCurrentPos + (Vector3)playerRigidBody.velocity;
            Vector3 targetPos = Vector3.Lerp(playerCurrentPos, playerFuturePos, launchFuturePosWeight);
            
            dir = (targetPos - transform.position).normalized;

            dieTimeStamp = Time.time + maxDuration;
        }

        public void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            
            if (followPlayerPos)
            {
                dir = Vector3.Lerp(dir, (playerRigidBody.transform.position - transform.position).normalized, followStrength);
            }
            
            transform.position += dir * (speed * dt);

            if (maxDuration > 0 && Time.time > dieTimeStamp)
            {
                Destroy(gameObject);
            }
        }
    }
}