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

public class BomberController : MonoBehaviour
{
    private enum BomberState
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
    [SerializeField] private Collider2D bombCollider;

    [Header("Settings")] 
    [SerializeField] private int lifePoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackWaitTime;
    [SerializeField] private float attackRange;
    
    [Header("Events")]
    [SerializeField] UnityEvent OnDie;

    private IPlayerFollower playerFollower;
    private BomberState currentState;
    
    private float attackTimeStamp = 0;
    private float currentScale = 1;

    private MapEntry room;
    
    private static readonly int animTrigger_attack = Animator.StringToHash("attack");

    void Awake()
    {
        playerFollower = playerFollowerType.CreateInstance();
    }

    void Start()
    {
        currentState = BomberState.Active;
        attackTimeStamp = 0;
    }

    void Update()
    {
        var dt = Time.deltaTime;
        
        visualParent.localScale = Vector3.one * currentScale;
    }

    private void FixedUpdate()
    {
        if (currentState == BomberState.Corpse)
        {
            return;
        }
        
        var dt = Time.fixedDeltaTime;
        var moveDir = playerFollower.GetMoveDirection(transform.position, out var sqrDistance);

        if (sqrDistance <= attackRange * attackRange)
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
        visualAnimator.SetTrigger(animTrigger_attack);
        moveSpeed = 0;
        attackSpeed = 0;
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.angularVelocity = 0;

        Invoke("Die", 1f);
    }

    void MoveTowardsPlayer(float dt, Vector2 moveDir)
    {
        rigidbody2D.AddForce(moveDir * (moveSpeed * dt));
    }

    public void OnHit(WeaponBase weapon)
    {
        Vector2 collisionDir = transform.position - weapon.transform.position;
        
        if (currentState == BomberState.Corpse)
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
        visualAnimator.SetTrigger(animTrigger_attack);
        Invoke("Die", 1f);
    }

    private void AddDamageForce(Vector2 collisionDir, float collisionForce)
    {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(collisionDir * collisionForce, ForceMode2D.Impulse);
    }

    public void SpawnBomberExplosion()
    {
        bloodParticles.transform.SetParent(transform.parent);
        BigBadSingleton.Instance.GameplayManager.SpawnBomberExplosionAt(transform.position);
    }

    public void SetRoom(MapEntry room)
    {
        this.room = room;
    }

    private void Die()
    {
        currentState = BomberState.Corpse;
        collisionSensor.gameObject.layer = LayerMask.NameToLayer("EnemyCorpse");
        //visualAnimator.SetTrigger(animTrigger_die);
        if (room != null)
        {
            room.RegisterEnemyKill();
        }
        OnDie?.Invoke();
    }
}
