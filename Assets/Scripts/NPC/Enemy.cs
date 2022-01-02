using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;



/// <remarks>https://ebookreading.net/view/book/EB9781783553655_68.html</remarks>
// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(CapsuleCollider))]
public class Enemy : CharacterSkeleton, IPoolableObject {
    protected NavMeshAgent agent;
    protected bool         canMove             = true;
    protected Vector2      smoothDeltaPosition = Vector2.zero;
    public    GameObject   target;
    public    Transform    targetDirection;
    public    int          ID;
    
    [Range(.1f, 1.2f)] public float   velocity         = .1f;
    private const             float   BaseGravity      = 20f;
    public                    float   rotationAngle    = 0f;
    public                    float   rotationSpeed    = .5f;
    public                    float   rotationSpeedCap = 2f;
    private                   Vector3 moveDirection    = Vector3.zero;

    public enum PlayerState {
        Idle,
        Died,
    }

    /*
    public enum State {
        Idle,
        Wander,
        Hunting,
        ChasingPlayer,
        ChasingEnemy,
        Attacking,
        Fleeing,
        Dead,
    }*/

    [Header("Movement")]
    public Transform activeAttackTarget;
    public PlayerHealth targetPlayerHealth;
    public Health       nonPlayerTargetHealth;
    /*public float        noticeRange   = 150f;
    public float        trackingRange = 20.0f;
    public float        chaseRange    = 20.0f;
    public float        rotSpeed      = 8.0f;
    public float        FOVAngle;*/



    private void OnEnable() {
        if (agent) { return; }
        NavMeshHit closestHit;
        if (NavMesh.SamplePosition(transform.position, out closestHit, 500f, 1)) {
            transform.position = closestHit.position;
            agent = gameObject.GetComponent<NavMeshAgent>();
            agent.baseOffset = 0f;
        }
    }


    public override void Initiate() {
        // rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        agent = gameObject.GetComponent<NavMeshAgent>();

        // rigidbody.useGravity = true;
        // rigidbody.freezeRotation = true;

        // 
        /*if (NavMesh.SamplePosition(transform.position, out NavMeshHit closestHit, 100f, 1)) {
            transform.position = closestHit.position;
        }*/
    }


    protected virtual void Update() {
        // UpdateStats();
    }


    protected internal bool NoTarget() {
        return (activeAttackTarget == null);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://learning.oreilly.com/library/view/unity-game-development/</remarks>
    /// <returns></returns>
    protected internal float GetDistanceToTarget() {
        return (transform.position - activeAttackTarget.transform.position).magnitude;
    }

    protected internal Vector3 GetTargetPosition() {
        return transform.position - activeAttackTarget.transform.position;
    }

    /*public KeyValuePair<float, Avatar> Was for multiplayer */
    // protected float GetPlayerDistance() {
    // Vector3 player = AvatarController.Instance.transform.position;
    // return Vector3.Distance(transform.position, player);
    // }


    /*
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float AttackTargetPruning() {
        float bestScore = float.MaxValue;
        foreach (Enemy enemy in GameManager.Instance.Enemies.Where(enemy => enemy.alliance != alliance)) {
            Debug.Assert(enemy != null);
            float newScore = 0f;
            // distance check
            float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToTarget > noticeRange) continue;
            newScore += distanceToTarget;

            float angleDifference = Mathf.Abs(Vector3.SignedAngle(transform.forward, enemy.transform.position - activeAttackTarget.position, Vector3.up));
            if (angleDifference > FOVAngle) { continue; }
            newScore += angleDifference;

            // evaluate scoring
            if (newScore <= bestScore) {
                bestScore = newScore;
                // Maybe Vector3 over Transform?
                activeAttackTarget = enemy.transform;
            }
        }

        return Vector3.Distance(transform.position, activeAttackTarget.transform.position);
    }
    */


    protected virtual void UpdateStats() {}


    protected virtual void OverriddenDefense() {}


    /*protected virtual void RotateTowardsTarget() {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(activeAttackTarget.position - transform.position), rotSpeed * Time.deltaTime);
    }*/

    public override void ApplyBuff(Health   h, ConditionInventory con)             { throw new NotImplementedException(); }
    public override void ApplyDebuff(Health h, ConditionInventory con)             { throw new NotImplementedException(); }
    public override void GetFromPool(string a, Vector3            b, Quaternion c) { throw new NotImplementedException(); }

    public void MoveTowardsTarget() {
        if (canMove) {
            if (target) {
                targetDirection = target.transform;
            }
            else {
                Debug.Log("Enemy: No Target");
            }
            Vector3 currSpeed = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            moveDirection = transform.forward * currSpeed.z + transform.right * currSpeed.x;
            agent.Move(moveDirection * velocity);
            animator.SetFloat("VelocityX", currSpeed.x, 0.1f, Time.deltaTime);
            animator.SetFloat("VelocityZ", currSpeed.z, 0.1f, Time.deltaTime);

            rotationAngle = Vector3.Angle(targetDirection.position - transform.position, transform.forward);
            rotationAngle = Mathf.Clamp(rotationAngle, -rotationSpeed, rotationSpeedCap);
        }
    }

    public override void Death() {
        // OnDeath();
        Destroy(this);
    }
    public override void DoDamage(Health other, float amount) {
        other.OnDamageTaken(amount, this);
    }

    public override void TakeDamage(float damageAmount) {
        // health.currentHealth -= damageAmount;
        // OnDamageTaken?.Invoke();
        // if (health.currentHealth <= 0) {
        //     OnDeath?.Invoke();
        // }
    }

}
