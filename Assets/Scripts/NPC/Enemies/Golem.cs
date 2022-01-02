using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;



/// <summary>
/// 
/// </summary>
// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(NavMeshAgent))]
// [RequireComponent(typeof(CapsuleCollider))]
public class Golem : Enemy {
    public Allegiance type = Allegiance.Golem;
    public GameObject GolemPrefab;
    public EnemyType  rank;
    public Enemy[]    Allies;

    private Node root;



    [Header("Attacking")]
    // public Action MeleeAttack;
    public Spell mainAttack;
    public Spell secondaryAttack;

    [Header("Follow")]
    public Health followTarget;
    public float followRange;


    // [Header("Components")]
    // private State rootState;
    // public List<State> states;

    public bool IsSpawner = false;
    public bool IsHunting;
    public bool IsAttacking;
    public bool IsFleeing;

    public  int  aggressionScale;
    public  bool ranged;
    public  bool melee;
    public  int  attackPreference;
    private bool Allocated = false;

    [Header("Stats")]
    public GameObject activeTargetObj;
    public SpellScript    baseSpellAttack;
    public CharacterTrait aggressionRange;
    public CharacterTrait attackRange;
    public CharacterTrait maxAttackRange;
    public CharacterTrait minAttackRange;
    public CharacterTrait _ManaRegen;
    public CharacterTrait _MaxMana;
    public CharacterTrait _Mana;
    public CharacterTrait _SwingSpeed;
    public CharacterTrait _RangeAttackRange;
    public CharacterTrait _MeleeAttackRange;
    public CharacterTrait _MeleeAttackCooldown;
    public CharacterTrait _RangeAttackCooldown;
    public CharacterTrait maxDamage;
    public CharacterTrait moveSpeed;

    public  GameObject      ProjectileRed;
    private List<Transform> _ShrapnelTransforms;



    public override void Initiate() {
        base.Initiate();

        IsSpawner = false;
        IsAttacking = false;
        health.IsDead = false;

        // mainAttack.Initiate();
        // secondaryAttack.Initiate();

        health.OnDeath += (damage) => animator.SetTrigger("Die");
        health.OnDamaged += (damage) => animator.SetTrigger("Take Damage");
        // OnDeath += Explode;
        mainAttack.OnDone += () => { IsAttacking = false; };
        secondaryAttack.OnDone += () => { IsAttacking = false; };

        switch (rank) {
            case EnemyType.Drone: {
                health.currentHealth = 50;
                health.maxHealth = 50;
                health.armor = 20;
                health.maxArmor = 20;

                maxDamage.standardVal = 10;
                maxDamage.bonus = 0;
                maxDamage.activeMultiplier = 0;

                _MeleeAttackRange.standardVal = 8;
                _MeleeAttackCooldown.standardVal = 2f;
                _RangeAttackRange.standardVal = 0;

                break;
            }
            case EnemyType.Champion: {
                health.currentHealth = 100;
                health.maxHealth = 100;
                health.armor = 50;
                health.maxArmor = 50;

                maxDamage.standardVal = 20;
                maxDamage.bonus = 0;
                maxDamage.activeMultiplier = 0;

                _MeleeAttackRange.standardVal = 8;
                _MeleeAttackCooldown.standardVal = 2f;
                _RangeAttackRange.standardVal = 0;

                break;
            }
            case EnemyType.Major: {
                health.currentHealth = 150;
                health.maxHealth = 150;
                health.armor = 50;
                health.maxArmor = 50;

                maxDamage.standardVal = 20;
                maxDamage.bonus = 0;
                maxDamage.activeMultiplier = 0;

                _MeleeAttackRange.standardVal = 8;
                _MeleeAttackCooldown.standardVal = 2f;
                _RangeAttackRange.standardVal = 0;

                break;
            }
            case EnemyType.Noble: {
                health.currentHealth = 200;
                health.maxHealth = 200;
                health.armor = 75;
                health.maxArmor = 75;

                maxDamage.standardVal = 25;
                maxDamage.bonus = 0;
                maxDamage.activeMultiplier = 0;

                _MeleeAttackRange.standardVal = 8;
                _MeleeAttackCooldown.standardVal = 2f;
                _RangeAttackRange.standardVal = 18;
                _RangeAttackCooldown.standardVal = 5f;

                break;
            }
            case EnemyType.Lord: {
                health.currentHealth = 200;
                health.maxHealth = 200;
                health.armor = 100;
                health.maxArmor = 100;

                maxDamage.standardVal = 25;
                maxDamage.bonus = 0;
                maxDamage.activeMultiplier = 0;

                _MeleeAttackRange.standardVal = 8;
                _MeleeAttackCooldown.standardVal = 2f;
                _RangeAttackRange.standardVal = 18;
                _RangeAttackCooldown.standardVal = 5f;

                break;
            }
            case EnemyType.King: {
                health.currentHealth = 300;
                health.maxHealth = 300;
                health.armor = 200;
                health.maxArmor = 200;

                maxDamage.standardVal = 30;
                maxDamage.bonus = 0;
                maxDamage.activeMultiplier = 0;

                _MeleeAttackRange.standardVal = 8;
                _MeleeAttackCooldown.standardVal = 2f;
                _RangeAttackRange.standardVal = 18;
                _RangeAttackCooldown.standardVal = 5f;

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }

        animator = GetComponent<Animator>();
        moveSpeed.standardVal = 8.0f;
        root = new Selector(new Node[] { new Node(FollowState), new Node(AttackState), new Node(Idle) });
        Allocated = true;
    }


    protected override void Update() {
        base.Update();

        if (!Allocated) { Initiate(); }
        // this.UpdateStats();
        if (health.IsDead || IsSpawner || !agent || IsAttacking) { return; }
        root.Evaluate();
        // float forwardSpeed = Vector3.Project(agent.desiredVelocity, transform.right).magnitude;
        // animator.SetBool("Walk Forward", forwardSpeed > 0);
    }


    /// <summary>
    /// Base state, always returns success
    /// </summary>
    /// <remarks>https://ebookreading.net/view/book/EB9781783553655.html</remarks>
    /// <returns>State.Success</returns>
    private State Idle() => State.SUCCESS;


    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    private State FollowState() {
        Debug.Log("Follow: Enter");
        // if (NoTarget() || (GetDistanceToTarget() > noticeRange)) { return State.FAILURE; }
        //
        // transform.position = Vector3.MoveTowards(transform.position, activeAttackTarget.position, Time.deltaTime * moveSpeed);
        // RotateTowardsTarget();
        // float forwardSpeed = Vector3.Project(agent.desiredVelocity, transform.right).magnitude;
        // animator.SetBool("Walk Forward", forwardSpeed > 0);

        if (!target) {
            return State.FAILURE; // No target
        }

        MoveTowardsTarget(/*direction, followRange*/);

        // if (direction.sqrMagnitude < followRange * followRange) {
        //     return State.FAILURE; // Within range
        // }
        //
        // Ray     dir            = new Ray(followTarget.transform.position, direction);
        // Vector3 targetPosition = dir.GetPoint(followRange * .8f);

        Debug.Log("Follow: Exit");
        // Evaluate();
        return State.SUCCESS;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    private State AttackState() {
        if (!NoTarget()) {
            float distance = GetTargetPosition().sqrMagnitude;

            if (distance > maxAttackRange.CurrentValue) {
                agent.SetDestination(new Ray(activeAttackTarget.transform.position, GetTargetPosition()).GetPoint(maxAttackRange.CurrentValue));
            }
            else if (distance < minAttackRange.CurrentValue) {
                // move away if range preferred.
                if (attackPreference == 2) {
                    // Vector3 gapPosition = activeAttackTarget.transform.position.normalized * -3f;
                    agent.SetDestination(new Ray(activeAttackTarget.transform.position, GetTargetPosition()).GetPoint(minAttackRange.CurrentValue));
                }
            }
            else {
                IsAttacking = true;
                agent.SetDestination(transform.position);
                mainAttack.transform.LookAt(activeAttackTarget.transform.position);
                mainAttack.Cast();
            }
        }
        else {
            // AttackTargetPruning();
            // activeAttackTarget = alliance.FindClosestAlly(transform.position, aggressionRange).transform;
            return State.FAILURE;
        }

        return State.SUCCESS;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    private State DieState() {
        Debug.Log("Die: Enter");
        Destroy(this.gameObject);
        return State.SUCCESS;
    }


    protected override void UpdateStats() {
        base.UpdateStats();
        agent.speed = moveSpeed.CurrentValue;
    }


    /// <summary>
    /// Golem Death Trait. Explode, sending damaging projectiles in random directions.
    /// </summary>
    private void Explode() {
        List<GameObject> shrapnel = new List<GameObject>();
        foreach (Transform shrap in _ShrapnelTransforms) {
            shrapnel.Add(Instantiate(ProjectileRed, shrap.position, shrap.rotation));
        }
        // TODO: add script to projectiles and apply force.

        Death();
    }

}
