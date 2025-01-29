using Scripts.Map.Movement;
using Scripts.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Map
{
    public class GhostBehaviour : MonoBehaviour
    {
        private enum GhostState
        {
            Spawning = 0,
            Chasing = 1,
            Firing = 2,
            Hit = 3,
            Dying = 4
        }
        
        [Header("Components")]
        [SerializeField] private PlayerFollowerType playerFollowerType;
        [SerializeField] private Rigidbody2D rigidbody2D;
        [SerializeField] private Transform visualParent;
        [SerializeField] private Animator visualAnimator;
        [SerializeField] private DamageColorFX damageFX;
        [SerializeField] private EnemyCollisionSensor collisionSensor;
        [SerializeField] private EnemyProjectile projectile;
        
        [Header("Settings")] 
        [SerializeField] private int lifePoints = 3;
        [SerializeField] private float moveSpeed = 5;
        [SerializeField] private float timeBetweenFires = 5;
        [SerializeField] private float fireWaitTime = 1;
        [SerializeField] private float fireRange = 2;
        [SerializeField] private float attackScaleChange = 2;
        [SerializeField] private float attackScaleChangeSpeed = 1;
        [SerializeField] private float fireAnimTime = 0.5f;
        [SerializeField] private float minDistanceToFlee = 0.5f;
        [SerializeField] private float hitWaitTime = 1;
        
        [Header("Events")]
        [SerializeField] UnityEvent OnDie;
        
        private static readonly int animTrigger_hit = Animator.StringToHash("hit");
        private static readonly int animTrigger_die = Animator.StringToHash("die");
        
        private IPlayerFollower playerFollower;
        private GhostState currentState;
        
        private float currentScale = 1;
        
        private float fireTimeStamp;
        
        private float fireAnimTimer;
        private float stateWaitTimer;
        
        private MapEntry room;

        private bool CanFire => fireTimeStamp < Time.time;
        
        void Awake()
        {
            playerFollower = playerFollowerType.CreateInstance();
        }
        
        void Start()
        {
            currentState = GhostState.Chasing;
        }

        void Update()
        {
            var dt = Time.deltaTime;

            if (fireAnimTimer > 0)
            {
                fireAnimTimer -= dt;
                var minTime = Mathf.Max(fireAnimTime, 0.01f);
                visualAnimator.SetLayerWeight(1, Mathf.Lerp(0, 1, fireAnimTimer / minTime));
            }

            currentScale = Mathf.Lerp(currentScale, 1, attackScaleChangeSpeed * dt);
            visualParent.localScale = Vector3.one * currentScale;
        }
        
        private void FixedUpdate()
        {
            if (currentState == GhostState.Dying)
            {
                return;
            }
        
            var dt = Time.fixedDeltaTime;

            switch (currentState)
            {
                case GhostState.Chasing:
                    var moveDir = playerFollower.GetMoveDirection(transform.position, out var sqrDistance);
                    if (sqrDistance <= minDistanceToFlee * minDistanceToFlee)
                    {
                        //flee from player
                        MoveTowardsPlayer(dt, -moveDir);
                    }
                    else if (sqrDistance <= fireRange * fireRange)
                    {
                        //fire projectiles
                        if (CanFire)
                        {
                            FireProjectile(moveDir);
                        }
                
                        //rigidbody2D.velocity = Vector3.zero;
                        //rigidbody2D.angularVelocity = 0f;
                    }
                    else
                    {
                        //walk towards player
                        MoveTowardsPlayer(dt, moveDir);
                    }
                    break;
                case GhostState.Firing:
                case GhostState.Hit:
                    stateWaitTimer -= dt;
                    if (stateWaitTimer <= 0)
                    {
                        currentState = GhostState.Chasing;
                    }
                    break;
            }
        }
        
        void MoveTowardsPlayer(float dt, Vector2 moveDir)
        {
            rigidbody2D.AddForce(moveDir * (moveSpeed * dt));
        }

        private void FireProjectile(Vector3 projectileDir)
        {
            fireAnimTimer = fireAnimTime;
            currentScale = attackScaleChange;
            fireTimeStamp = Time.time + timeBetweenFires;
            
            var projectile = BigBadSingleton.Instance.GameplayManager.
                SpawnTemporaryObject(this.projectile, transform.position);
            projectile.Fire(BigBadSingleton.Instance.GameplayManager.PlayerRigidBody);

            currentState = GhostState.Firing;
            stateWaitTimer = fireWaitTime;
        }
        
        public void OnHit(WeaponBase weapon)
        {
            if (currentState == GhostState.Dying)
            {
                return;
            }
            
            Vector2 collisionDir = transform.position - weapon.transform.position;
            
            lifePoints -= weapon.Damage;
            damageFX.Flash();
        
            if (lifePoints <= 0)
            {
                rigidbody2D.freezeRotation = false;
                Die();
            }
            else
            {
                visualAnimator.SetTrigger(animTrigger_hit);
                stateWaitTimer = hitWaitTime;
                currentState = GhostState.Hit;
            }
            
            AddDamageForce(collisionDir, 6f);
        }

        private void AddDamageForce(Vector2 collisionDir, float collisionForce)
        {
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.AddForce(collisionDir * collisionForce, ForceMode2D.Impulse);
        }
        
        public void SetRoom(MapEntry room)
        {
            this.room = room;
        }

        private void Die()
        {
            currentState = GhostState.Dying;
            visualAnimator.SetTrigger(animTrigger_die);
            if (room != null)
            {
                room.RegisterEnemyKill();
            }
            OnDie?.Invoke();
        }
        
        public void SpawnExplosion()
        {
            BigBadSingleton.Instance.GameplayManager.SpawnBigExplosionAt(transform.position);
        }
    }
}