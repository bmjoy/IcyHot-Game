using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



/// <summary>
///  A character creation system similar to the system used by Divinity Original Sin 2 Official Mod tutorial
/// </summary>
/// /* Tutorial: https://kybernetik.com.au/animancer/docs/introduction/features */
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Avatar : CharacterSkeleton {
    [HideInInspector] public uint player_ID;

    [Header("Components")]
    public PlayerAvatar playerAvi;
    public Rigidbody           rigbody;
    public SpellToolbox        toolbox;
    public List<SpellScript>   scripts;
    public CharacterController controller;
    public Plane               surfacePlane;
    public NavMeshAgent        agent;


    [Header("Firing")]
    public KeyCode specialCast = KeyCode.Q;
    public GameObject projectilePrefab;
    public Transform  aimPoint;

    [Header("Ability Settings")]
    public KeyCode SpecCastButton = KeyCode.Q;
    public KeyCode PrimaryCastButton = KeyCode.Mouse0;

    [Header("Camera Settings")]
    public Camera playerCamera;
    public GameObject TargetIndicatorPrefab;
    public GameObject TargetIndicator;
    public enum CameraDirection { x, z }
    public CameraDirection cameraDirection = CameraDirection.x;
    // public float           cameraHeight    = 30f;
    // public float           cameraDistance  = 30f;

    [Header("Movement")]
    public float jumpHeight = 20f;
    [Range(.1f, 1.2f)] public float   moveSpeed         = .1f;
    private const             float   BaseGravity       = 20f;
    public                    float   rotationAngle     = 0f;
    public                    float   rotationSpeed     = .5f;
    public                    float   rotationSpeedCap  = 2f;
    private                   Vector3 moveDirection     = Vector3.zero;
    [HideInInspector] public  bool    didCast           = false;
    [HideInInspector] public  bool    didAct            = false;
    [HideInInspector] public  bool    isMoving          = false;
    [HideInInspector] public  float   maxVelocityChange = 2f;
    [HideInInspector] public  bool    canMove           = true;
    [HideInInspector] public  bool    canJump           = true;
    [HideInInspector] public  bool    isRunning         = false;
    [HideInInspector] public  bool    canCast           = true;

    private LayerMask LayerGround;
    private LayerMask LayerPlayer;
    private LayerMask LayerEnemy;
    private LayerMask LayerDefault;

    [Header("Living Status")]
    private static readonly int _Dying = Animator.StringToHash("Dying");
    private static readonly int _Alive  = Animator.StringToHash("Alive");
    private static readonly int _Moving = Animator.StringToHash("isMoving");

    [Header("Skills")]
    public CharacterTrait _ManaRegen;
    public CharacterTrait _MaxMana;
    public CharacterTrait _SwingSpeed;
    public CharacterTrait _SpellAttackRange;
    public CharacterTrait _AbilityAttackRange;

    [Header("Diagnostics")]
    public float jumpSpeed;
    public bool isGrounded = true;
    public bool isFalling;

    public bool Allocated { get;                    private set; }
    public bool DidCast   { get { return didCast; } set { didCast = value; } }
    public bool DidAct    { get { return didAct; }  set { didAct = value; } }


    private class ActionButtonState {
        private readonly string p_ID;
        private          bool   p_Down;
        private          bool   p_Held;
        private          bool   p_Up;
        private          bool   p_Released;

        public bool DOWN     { get { return p_Down; }     set {} }
        public bool HELD     { get { return p_Held; }     set {} }
        public bool UP       { get { return p_Up; }       set {} }
        public bool RELEASED { get { return p_Released; } set {} }

        public ActionButtonState(string buttonId) { p_ID = buttonId; }

        public void EvaluateInput() {
            p_Held = Input.GetButton(p_ID);
            p_Down = Input.GetButtonDown(p_ID);
            p_Released = Input.GetButtonUp(p_ID);
            p_Up = !p_Held;
        }
    }

    private readonly ActionButtonState SpecActionButtonState = new ActionButtonState("SpecialCast");
    public           bool              SpecCastDown     { get { return SpecActionButtonState.DOWN; } }
    public           bool              SpecCastHeld     { get { return SpecActionButtonState.HELD; } }
    public           bool              SpecCastReleased { get { return SpecActionButtonState.RELEASED; } }

    private readonly ActionButtonState PrimaryActionButtonState = new ActionButtonState("BasicCast");
    public           bool              PrimaryCastDown     { get { return PrimaryActionButtonState.DOWN; } }
    public           bool              PrimaryCastHeld     { get { return PrimaryActionButtonState.HELD; } }
    public           bool              PrimaryCastReleased { get { return PrimaryActionButtonState.RELEASED; } }

    private readonly ActionButtonState CancelCastActionButtonState = new ActionButtonState("CancelCast");
    public           bool              CancelSpellDown     { get { return CancelCastActionButtonState.DOWN; } }
    public           bool              CancelSpellHeld     { get { return CancelCastActionButtonState.HELD; } }
    public           bool              CancelSpellReleased { get { return CancelCastActionButtonState.RELEASED; } }


    // Start Character
    public void Initiate(PlayerAvatar playerObj) {
        base.Initiate();
        playerAvi = playerObj;
        rigbody = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();
        toolbox = GetComponent<SpellToolbox>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        // aimPoint = playerCamera.transform;

        animator.SetFloat("VelocityX", 0);
        animator.SetFloat("VelocityZ", 0);
        animator.SetBool("isJumping", false);
        animator.SetBool("isCasting", false);
        animator.SetBool("Dying", false);
        animator.SetBool("Alive", true);

        if (TargetIndicatorPrefab) {
            TargetIndicator.SetActive(false);
        }
        health.OnDeath += (damage) => {
            animator.SetTrigger(_Dying);
            // playeravi.PlayerDead();
        };
        ((PlayerHealth)health).OnRespawn += (heal) => {
            animator.SetTrigger(_Alive);
        };

        health.maxHealth = maxHealth.CurrentValue;
        health.maxArmor = maxArmor.CurrentValue;
        playerCamera.enabled = true;
        rigbody.useGravity = false;
        rigbody.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Allocated = true;
    }


    void Awake() {
        if (TargetIndicatorPrefab) {
            TargetIndicator = Instantiate(TargetIndicatorPrefab, Vector3.zero, Quaternion.identity);
        }
    }


    void Update() {
        if (!Allocated) { Initiate(); }
        PrimaryActionButtonState.EvaluateInput();

        animator.ResetTrigger("isJumping");
        TargetIndicator.transform.position = GetAimTargetPos();
        TargetIndicator.transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
        // aimPoint.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), 0);

        // Character rotation
        Vector3 currSpeed = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        CheckMoving();
        // currSpeed = canMove ? (isMoving ? currSpeed * moveSpeed : Vector3.zero) : Vector3.zero;
        animator.SetFloat("VelocityX", currSpeed.x, 0.1f, Time.deltaTime);
        animator.SetFloat("VelocityZ", currSpeed.z, 0.1f, Time.deltaTime);

        if (isGrounded) {
            isFalling = false;
        }
        if ((isGrounded || !isFalling) && jumpSpeed < 1f && Input.GetKey(KeyCode.Space)) {
            jumpSpeed = Mathf.Lerp(jumpSpeed, 1f, 0.5f);
        }
        else if (!isGrounded) {
            isFalling = true;
            jumpSpeed = 0;
        }
        if (canMove) {
            moveDirection = transform.forward * currSpeed.z + transform.right * currSpeed.x;
            agent.Move(moveDirection * moveSpeed);

            rotationAngle += -Input.GetAxis("Mouse Y") * rotationSpeed;
            rotationAngle = Mathf.Clamp(rotationAngle, -rotationSpeed, rotationSpeedCap);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationAngle, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);
        }

        // UpdateSkillSet();

        CheckInput();
    }


    /// <summary>
    /// 
    /// </summary>
    private void CheckInput() {
        if (PrimaryCastDown || SpecCastDown) {
            // Check if uninterrupted.
            if (canCast) {
                // Can only cast one spell at a time. Primary filler spell takes priority.
                if (PrimaryCastDown) {
                    Debug.Log("BaseCast Pressed!");
                    BasicCast();
                    // if (PrimaryCastHeld) {
                    //     Debug.Log("BaseCast Held!");
                    //     toolbox.Postpone();
                    // }
                    // else {
                    //     BasicCast();
                    // }
                }
                // Specialized spell if Primary is not called.
                else if (SpecCastDown) {
                    Debug.Log("SpecCast Pressed!");
                    if (SpecCastHeld) {
                        Debug.Log("SpecCast Held!");
                        toolbox.Postpone();
                    }
                    else {
                        SpecialCast();
                    }
                }
            }
        }

        CancelCastActionButtonState.EvaluateInput();
        if (CancelCastActionButtonState.DOWN) {
            CancelSpell();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SelectSpell(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SelectSpell(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SelectSpell(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            SelectSpell(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            SelectSpell(4);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private void OnCollisionStay() {
        canMove = true;
    }


    /// <summary>
    /// 
    /// </summary>
    private void UpdateSkillSet() {
        health.maxHealth = maxHealth.CurrentValue;
        health.maxArmor = maxArmor.CurrentValue;

        toolbox.switchDelay = 1 / _SwingSpeed.CurrentValue;
        toolbox._ManaRegen = _ManaRegen.CurrentValue;
        toolbox._MaximumMana = _MaxMana.CurrentValue;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="spellIndex"></param>
    private void SelectSpell(int spellIndex) {
        toolbox.ScriptSelection(spellIndex);
    }


    /// <summary>
    /// 
    /// </summary>
    private void SpecialCast() {
        toolbox.SpecialFire();
        DidCast = true;
    }


    /// <summary>
    /// 
    /// </summary>
    public void BasicCast() {
        Debug.Log("BasicCast() called!");
        toolbox.BasicFire();
        // Instantiate(projectilePrefab, aimPoint);
        // StartCoroutine(SpellCooldown(basicSpellScript, CooldownType.Standard));
    }


    /// <summary>
    /// 
    /// </summary>
    private void CancelSpell() {
        Debug.Log("CancelSpell() called!");
        toolbox.CancelCasting();
        DidCast = false;
    }


    /// <summary>
    /// Animation Trigger
    /// </summary>
    private void OnAttack() {
        animator.SetTrigger("Fire");
    }


    // void onColisionStay() {}

    /// <summary>
    /// Hit Trigger
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Projectile>() != null) {
            // other.GetComponent<Projectile>(). --health.currentHealth;
            if (health.currentHealth == 0) { GameManager.Destroy(gameObject); }
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="mousePoint"></param>
    private void CastReport(Vector3 mousePoint) {
        Debug.Log("CastReport called");


    }



    // ---------------------------------------------------- Required Overrides ---------------------------------------------------- \\
    public override void ApplyBuff(Health   h,     ConditionInventory con)    { playerAvi.PostBuff(h, con); }
    public override void ApplyDebuff(Health h,     ConditionInventory con)    { playerAvi.PostDebuff(h, con); }
    public override void DoDamage(Health    other, float              amount) { playerAvi.DealDamage(other, amount); }
    public override void TakeDamage(float damageAmount) {
        health.currentHealth -= damageAmount;
        playerAvi.ReceiveDamage(damageAmount);
    }
    public override void GetFromPool(string a, Vector3 b, Quaternion c) { throw new NotImplementedException(); }
    public override void Death() {
        // TODO: Start game over sequence.
    }



    // ----------------------------------------------------Checkers---------------------------------------------------- \\
    public bool IsGrounded() {
        float distToGround = GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private void CheckMoving() {
        isMoving = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private float CalculateJumpVerticalSpeed() {
        return Mathf.Sqrt(2 * jumpHeight * BaseGravity);
    }


    /*
    * https://sharpcoderblog.com/blog/top-down-character-controller-example
    */
    private Vector3 GetAimTargetPos() {
        // Update surface plane
        surfacePlane.SetNormalAndPosition(Vector3.up, transform.position);
        // Mouse position
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (surfacePlane.Raycast(ray, out float enter)) {
            // Get point if clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }

        //No raycast hit, hide the aim target by moving it far away
        return new Vector3(-5000, -5000, -5000);
    }

}
