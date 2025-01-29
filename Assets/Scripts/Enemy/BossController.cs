using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts;
using Scripts.Map;
using Scripts.Map.Movement;
using Scripts.Weapons;
using UnityEngine;
using UnityEngine.Events;

public class BossController : MonoBehaviour
{
    private enum BossState
    {
        Spawning = 0,
        Active = 1,
        Corpse = 2
    }

    [Header("Components")]
    [SerializeField] private PlayerFollowerType playerFollowerType;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private Transform visualParent;
    [SerializeField] private Animator visualAnimator;
    [SerializeField] private DamageColorFX damageFX;
    [SerializeField] private EnemyCollisionSensor collisionSensor;
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private BossProjectile projectileNorm;
    [SerializeField] private BossProjectile projectileEnraged;
    [SerializeField] private BossSounds bossAudio;

    [Header("Settings")] 
    [SerializeField] public int lifePoints;
    [SerializeField] private int enragedLifePointThreshhold;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackWaitTime;
    [SerializeField] private float attackRange;

    [Header("Projectile Settings")]
    [SerializeField] private float timeBetweenFires;
    [SerializeField] private float fireWaitTime;
    [SerializeField] private float fireRange;
    [SerializeField] private float fireAnimTime = 0.5f;
    [SerializeField] private float hitWaitTime = 1;

    [Header("Game State Check")]
    [SerializeField] BossFight bossFight;

    [Header("Events")]
    [SerializeField] UnityEvent OnDie;

    private IPlayerFollower playerFollower;
    private BossState currentState;
    
    private float attackTimeStamp;
    private float fireTimeStamp;
    private float fireAnimTimer;

    private MapEntry room;
    
    private static readonly int animTrigger_attack = Animator.StringToHash("attack-melee");
    private static readonly int animTrigger_die = Animator.StringToHash("dead");

    private bool CanAttack => attackTimeStamp < Time.time;
    private bool CanFire => fireTimeStamp < Time.time;
    private bool beginAttacking = false;
    private bool bossIsEnraged = false;

    void Awake()
    {
        playerFollower = playerFollowerType.CreateInstance();
    }

    void Start()
    {
        currentState = BossState.Active;
        attackTimeStamp = 0;

        StartCoroutine(BossEnterStop());
    }

    void Update()
    {
        var dt = Time.deltaTime;

        if (lifePoints == enragedLifePointThreshhold)
        {
            StartCoroutine(EnrageBoss());
            lifePoints -= 1;
        }

        // Check if game is over and stop all Coroutines (projectile firing)
        if (bossFight.stopCoroutines)
        {
            rigidbody2D.velocity = Vector3.zero;
            StopAllCoroutines();
        }
    }

    private void FixedUpdate()
    {
        if (currentState == BossState.Corpse)
        {
            return;
        }
        
        var dt = Time.fixedDeltaTime;
        var moveDir = playerFollower.GetMoveDirection(transform.position, out var sqrDistance);
        if (sqrDistance <= attackRange * attackRange && CanAttack)
        {
            Attack(dt, moveDir);
        }
        else if (sqrDistance <= fireRange * fireRange && CanFire && beginAttacking == true)
        {
            StartCoroutine(FireProjectile(moveDir));
        }
        else
        {
            MoveTowardsPlayer(dt, moveDir);
        }
    }

    private IEnumerator BossEnterStop()
    {
        bossAudio.BossRoarsSound();
        visualAnimator.SetBool("boss-enter-animation", true);
        BoxCollider2D bossCollider = collisionSensor.GetComponent<BoxCollider2D>();
        bossCollider.enabled = false;
        rigidbody2D.velocity = Vector3.zero;

        yield return new WaitForSeconds(3.2f);

        beginAttacking = true;
        bossCollider.enabled = true;
        visualAnimator.SetBool("boss-enter-animation", false);
    }

    private IEnumerator EnrageBoss()
    {
        beginAttacking = false;
        bossAudio.BossRoarsSound();
        visualAnimator.SetBool("boss-becomes-enraged", true);
        BoxCollider2D bossCollider = collisionSensor.GetComponent<BoxCollider2D>();
        bossCollider.enabled = false;

        yield return new WaitForSeconds(3.2f);
        
        visualAnimator.SetBool("boss-becomes-enraged", false);
        bossIsEnraged = true;
        beginAttacking = true;
        bossCollider.enabled = true;

    }

    void Attack(float dt, Vector2 moveDir)
    {
        attackTimeStamp = Time.time + attackWaitTime;
        
        rigidbody2D.AddForce(moveDir * (attackSpeed * dt), ForceMode2D.Impulse);
        visualAnimator.SetTrigger(animTrigger_attack);
    }

    private IEnumerator FireProjectile(Vector3 projectileDir)
    {
        // Disable boss collider so that projectiles may fire
        BoxCollider2D bossCollider = collisionSensor.GetComponent<BoxCollider2D>();
        bossCollider.enabled = false;

        if (!bossIsEnraged)
        {
            for (var i = 0; i < 5; i++)
            {
                fireTimeStamp = Time.time + timeBetweenFires;

                if (Time.timeScale != 0)
                {
                    var projectile = BigBadSingleton.Instance.GameplayManager.SpawnTemporaryObject(this.projectileNorm, transform.position);
                    projectile.Fire(BigBadSingleton.Instance.GameplayManager.PlayerRigidBody);
                }
                else
                {
                    rigidbody2D.velocity = Vector3.zero;
                    StopAllCoroutines();
                    break;
                }

                yield return new WaitForSeconds(0.25f);
            }
        }
        else
        {
            for (var i = 0; i < 4; i++)
            {

                yield return new WaitForSeconds(1f);
                fireTimeStamp = Time.time + timeBetweenFires;

                if (Time.timeScale != 0)
                {
                    var projectile = BigBadSingleton.Instance.GameplayManager.SpawnTemporaryObject(this.projectileEnraged, transform.position);
                    projectile.Fire(BigBadSingleton.Instance.GameplayManager.PlayerRigidBody);
                }
                else
                {
                    rigidbody2D.velocity = Vector3.zero;
                    StopAllCoroutines();
                    break;
                }
            }
        }
        
        bossCollider.enabled = true;
    }

    void MoveTowardsPlayer(float dt, Vector2 moveDir)
    {
        SpriteRenderer spriteRenderer = visualParent.GetComponent<SpriteRenderer>();

        if (moveDir.x < rigidbody2D.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;    
        }

        rigidbody2D.AddForce(moveDir * (moveSpeed * dt));
    }

    public void OnHit(WeaponBase weapon)
    {
        Vector2 collisionDir = transform.position - weapon.transform.position;
        
        if (currentState == BossState.Corpse)
        {
            AddDamageForce(collisionDir, 1f);
        }
        else
        {
            float collisionForce = 0.1f;
            
            lifePoints -= weapon.Damage;
            damageFX.Flash();
        
            if (lifePoints <= 0)
            {
                rigidbody2D.freezeRotation = false;
                collisionForce = 1f;
                Die();
            }
            
            AddDamageForce(collisionDir, collisionForce);
        }
    }

    private void AddDamageForce(Vector2 collisionDir, float collisionForce)
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(collisionDir * collisionForce, ForceMode2D.Impulse);
    }

    public void SpawnBossExplosion()
    {
        //bloodParticles.transform.SetParent(transform.parent);
        BigBadSingleton.Instance.GameplayManager.SpawnBossExplosionAt(transform.position);
    }

    public void SpawnExplosion()
    {
        BigBadSingleton.Instance.GameplayManager.SpawnFXAt(FXType.Explosion, transform.position);
    }

    public void SetRoom(MapEntry room)
    {
        this.room = room;
    }

    private void Die()
    {
        currentState = BossState.Corpse;
        collisionSensor.gameObject.layer = LayerMask.NameToLayer("EnemyCorpse");
        visualAnimator.SetTrigger(animTrigger_die);
        if (room != null)
        {
            room.RegisterEnemyKill();
        }
        OnDie?.Invoke();
    }
}
