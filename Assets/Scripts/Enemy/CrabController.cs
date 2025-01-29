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

public class CrabController : MonoBehaviour
{
    private enum CrabState
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

    [Header("Settings")] 
    [SerializeField] private int lifePoints = 1;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float attackSpeed = 5;
    [SerializeField] private float attackWaitTime = 1;
    [SerializeField] private float attackRange = 2;
    [SerializeField] private float attackScaleChange = 2;
    [SerializeField] private float attackscaleChangeSpeed = 1;
    
    [Header("Events")]
    [SerializeField] UnityEvent OnDie;

    private IPlayerFollower playerFollower;
    private CrabState currentState;
    
    private float attackTimeStamp = 0;
    private float currentScale = 1;

    private MapEntry room;
    
    private static readonly int animTrigger_attack = Animator.StringToHash("attack");
    private static readonly int animTrigger_die = Animator.StringToHash("die");

    private bool CanAttack => attackTimeStamp < Time.time;

    void Awake()
    {
        playerFollower = playerFollowerType.CreateInstance();
    }

    void Start()
    {
        currentState = CrabState.Active;
        attackTimeStamp = 0;
    }

    void Update()
    {
        var dt = Time.deltaTime;
        
        currentScale = Mathf.Lerp(currentScale, 1, attackscaleChangeSpeed * dt);
        visualParent.localScale = Vector3.one * currentScale;
    }

    private void FixedUpdate()
    {
        if (currentState == CrabState.Corpse)
        {
            return;
        }
        
        var dt = Time.fixedDeltaTime;
        var moveDir = playerFollower.GetMoveDirection(transform.position, out var sqrDistance);
        if (sqrDistance <= attackRange * attackRange && CanAttack)
        {
            Attack(dt, moveDir);
        }
        else
        {
            MoveTowardsPlayer(dt, moveDir);
        }
    }

    void Attack(float dt, Vector2 moveDir)
    {
        currentScale = attackScaleChange;
        attackTimeStamp = Time.time + attackWaitTime;
        
        rigidbody2D.AddForce(moveDir * (attackSpeed * dt), ForceMode2D.Impulse);
        visualAnimator.SetTrigger(animTrigger_attack);
    }

    void MoveTowardsPlayer(float dt, Vector2 moveDir)
    {
        rigidbody2D.AddForce(moveDir * (moveSpeed * dt));
    }

    public void OnHit(WeaponBase weapon)
    {
        Vector2 collisionDir = transform.position - weapon.transform.position;
        
        if (currentState == CrabState.Corpse)
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

    public void SpawnBigExplosion()
    {
        bloodParticles.transform.SetParent(transform.parent);
        BigBadSingleton.Instance.GameplayManager.SpawnBigExplosionAt(transform.position);
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
        currentState = CrabState.Corpse;
        collisionSensor.gameObject.layer = LayerMask.NameToLayer("EnemyCorpse");
        visualAnimator.SetTrigger(animTrigger_die);
        if (room != null)
        {
            room.RegisterEnemyKill();
        }
        OnDie?.Invoke();
    }
}
