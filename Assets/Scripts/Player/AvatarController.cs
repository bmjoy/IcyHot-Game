// using System;
// using JetBrains.Annotations;
// using UnityEngine;
//
//
// /*
//     Tutorials: 
//     https://titanwolf.org/Network/Articles/Article?AID=bbfd7597-9471-4a0e-9c80-667ac8528213
// */
// // [RequireComponent(typeof(CapsuleCollider))]
// // [RequireComponent(typeof(Rigidbody))]
// public class AvatarController : MonoBehaviour {
//     public        Avatar           avatar;
//     private       LayerMask        LayerGround;
//     private       LayerMask        LayerPlayer;
//     private       LayerMask        LayerEnemy;
//     private       LayerMask        LayerDefault;
//     public        Plane            surfacePlane;
//
//     
//     [Header("Movement Settings")]
//     [Range(1, 20)] public float moveSpeed = 8f;
//     public                   float turnSensitivity   = 5f;
//     public                   float maxTurnSpeed      = 150f;
//     private const            float BaseGravity       = 20f;
//     public                   float rotationAngle     = 0f;
//     public                   float rotationSpeed     = 2f;
//     public                   float rotationSpeedCap  = 50f;
//     
//     [HideInInspector] public bool  isRunning         = false;
//     [HideInInspector] public bool  canCast           = true;
//     [HideInInspector] public float maxVelocityChange = 2f;
//     private static readonly  int   m_VelocityX       = Animator.StringToHash("velx");
//     private static readonly  int   m_VelocityY       = Animator.StringToHash("vely");
//
//     // [Range(1, 50)] private   float speed             = 20f;
//     // [HideInInspector] public float maxVelocityChange = 20.0f;
//     // private static readonly  int   s_Velx            = Animator.StringToHash("velx");
//     // private static readonly  int   s_Vely            = Animator.StringToHash("vely");
//     
//     [Header("Diagnostics")]
//     public float horizontal;
//     public                   float vertical;
//     public                   float turn;
//     public                   float jumpSpeed;
//     public                   bool  isGrounded = true;
//     public                   bool  isFalling;
//
//     public                   bool  isMoving = false;
//     [HideInInspector] public bool  canMove  = true;
//     [HideInInspector] public bool  canJump  = true;
//     
//     [Header("Animations")]
//     public string animationTrigger_Casting = "Cast Spell";
//     public string animationTrigger_Postpone = "isCasting";
//     public string animationTrigger_Throw = "Punch Attack";
//     public string animationTrigger_TakeHit = "Take Damage";
//     public string animationTrigger_Defense = "Defend";
//     public string animationTrigger_Die = "Die";
//     public string animationTrigger_Jump = "Jump";
//     
//
//
//     public void Awake() {
//         if (avatar.playerCamera == null) { avatar.playerCamera = GetComponent<Camera>(); }
//         avatar.rigbody = GetComponent<Rigidbody>();
//         avatar.rigbody.useGravity = false;
//         avatar.rigbody.freezeRotation = true;
//     }
//
//
//     public void Start() {
//         avatar = GetComponent<Avatar>();
//         avatar.playerCamera.orthographic = true;
//         avatar.playerCamera.transform.SetParent(transform);
//         avatar.playerCamera.transform.localPosition = new Vector3(0f, 3f, -8f);
//         avatar.playerCamera.transform.localEulerAngles = new Vector3(10f, 0f, 0f);
//
//         LayerGround = LayerMask.NameToLayer("Ground");
//         LayerPlayer = LayerMask.NameToLayer("Player");
//         LayerEnemy = LayerMask.NameToLayer("Enemy");
//         LayerDefault = LayerMask.NameToLayer("Default");
//
//     }
//
//
//     public void OnDisable() {
//         if (avatar.playerCamera != null) {
//             avatar.playerCamera.orthographic = true;
//             avatar.playerCamera.transform.SetParent(null);
//             avatar.playerCamera.transform.localPosition = new Vector3(0f, 70f, 0f);
//             avatar.playerCamera.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
//         }
//     }
//     
//     
//     void FixedUpdate() {
//         avatar.cameraHeight = Mathf.Clamp(avatar.cameraHeight + Input.GetAxis("Mouse ScrollWheel") * 2, 25, 60);
//         avatar.cameraDistance = Mathf.Clamp(avatar.cameraDistance + Input.GetAxis("Mouse ScrollWheel") * 2, 30, 100);
//
//         // isGrounded = Physics.Raycast(.position, Vector3.down, 0.1f, ground);
//
//         // cameraOffset
//         Vector3 cameraOffset = Vector3.zero;
//         if (avatar.cameraDirection == Avatar.CameraDirection.x) {
//             cameraOffset = new Vector3(avatar.cameraDistance, avatar.cameraHeight, 0);
//         }
//         else if (avatar.cameraDirection == Avatar.CameraDirection.z) {
//             cameraOffset = new Vector3(0, avatar.cameraHeight, avatar.cameraDistance);
//         }
//
//
//         if (canMove /*&& IsGrounded()*/) {
//             // float step = moveSpeed * Time.deltaTime; // calculate distance to move
//             avatar.animator.ResetTrigger("isJumping");
//             
//             Vector3 targetVelocity = Vector3.zero;
//             if (avatar.cameraDirection == Avatar.CameraDirection.x) {
//                 targetVelocity = new Vector3(Input.GetAxis("Vertical") * (avatar.cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Horizontal") * (avatar.cameraDistance >= 0 ? 1 : -1));
//             }
//             else if (avatar.cameraDirection == Avatar.CameraDirection.z) {
//                 targetVelocity = new Vector3(Input.GetAxis("Horizontal") * (avatar.cameraDistance >= 0 ? -1 : 1), 0, Input.GetAxis("Vertical") * (avatar.cameraDistance >= 0 ? -1 : 1));
//             }
//
//             targetVelocity *= moveSpeed;
//             Vector3 velocity       = avatar.rigbody.velocity;
//             Vector3 velocityChange = (targetVelocity - velocity);
//             velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
//             velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
//             velocityChange.y = 0f;
//             avatar.rigbody.AddForce(velocityChange, ForceMode.VelocityChange);
//
//             if (Input.GetKey(KeyCode.W)) {
//                 // transform.position += Vector3.ClampMagnitude(transform.forward, 1f) * step;
//                 avatar.rigbody.AddForce(-transform.forward * (moveSpeed), ForceMode.Force);
//                 // transform.position = Vector3.MoveTowards(transform.position, TargetIndicator.transform.position, step);
//             }
//             if (Input.GetKey(KeyCode.S)) {
//                 // transform.position -= Vector3.ClampMagnitude(transform.forward, 1f) * step;
//                 avatar.rigbody.AddForce(transform.forward * (moveSpeed), ForceMode.Force);
//             }
//             if (Input.GetKey(KeyCode.A)) {
//                 // transform.position -= Vector3.ClampMagnitude(transform.right, 1f) * step;
//                 avatar.rigbody.AddForce(-transform.right * (moveSpeed), ForceMode.Force);
//             }
//             if (Input.GetKey(KeyCode.D)) {
//                 // transform.position += Vector3.ClampMagnitude(transform.right, 1f) * step;
//                 avatar.rigbody.AddForce(transform.right * (moveSpeed), ForceMode.Force);
//             }
//             avatar.animator.SetFloat("VelocityX", velocity.x, 0.1f, Time.deltaTime);
//             avatar.animator.SetFloat("VelocityZ", velocity.z, 0.1f, Time.deltaTime);
//
//             // TODO
//             if (canJump && Input.GetButton("Jump")) {
//                 avatar.rigbody.AddForce(transform.up * avatar.jumpHeight);
//             }
//         }
//         avatar.rigbody.AddForce(new Vector3(0, -BaseGravity * avatar.rigbody.mass, 0));
//         // canMove = false;
//         
//         //Camera follow
//         // avatar.playerCamera.transform.position = Vector3.Slerp(transform.position, transform.position + cameraOffset, Time.deltaTime * 7.4f);
//         // avatar.playerCamera.transform.LookAt(avatar.transform);
//         // avatar.playerCamera.transform.position = new Vector3(avatar.playerCamera.transform.position.x + cameraOffset.x, avatar.playerCamera.transform.position.y + cameraOffset.y, avatar.playerCamera.transform.position.z + cameraOffset.z);
//
//         //Aim target position and rotation
//         avatar.TargetIndicator.transform.position = GetAimTargetPos();
//         avatar.TargetIndicator.transform.LookAt(new Vector3(transform.position.x, avatar.TargetIndicator.transform.position.y, transform.position.z));
//         //Player rotation
//         transform.LookAt(avatar.TargetIndicator.transform.position);
//         // transform.LookAt(new Vector3(avatar.TargetIndicator.transform.position.x, transform.position.y, avatar.TargetIndicator.transform.position.z));
//
//         //-------------------------------------------------------------------------------------------------------------\\
//         transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);
//
//         Vector3 direction = new Vector3(horizontal, jumpSpeed, vertical);
//         direction = Vector3.ClampMagnitude(direction, 1f);
//         direction = transform.TransformDirection(direction);
//         direction *= moveSpeed;
//
//         if (jumpSpeed > 0) { avatar.transform.up = (direction * Time.fixedDeltaTime); }
//         // else
//         // avatar.transform.SimpleMove(direction);
//
//         // isGrounded = avatar.isGrounded;
//     }
//
//     
//     /// <summary>
//     /// https://sharpcoderblog.com/blog/top-down-character-controller-example
//     /// </summary>
//     /// <returns></returns>
//     private Vector3 GetAimTargetPos() {
//         //Update surface plane
//         surfacePlane.SetNormalAndPosition(Vector3.up, transform.position);
//
//         //Create a ray from the Mouse click position
//         Ray ray = avatar.playerCamera.ScreenPointToRay(Input.mousePosition);
//
//         if (surfacePlane.Raycast(ray, out float enter)) {
//             //Get the point that is clicked
//             Vector3 hitPoint = ray.GetPoint(enter);
//             // hitPoint.y += 1;
//             return hitPoint;
//         }
//
//         //No raycast hit, hide the aim target by moving it far away
//         return new Vector3(-5000, -5000, -5000);
//     }
//     
//     
//     private bool CheckMoving() {
//         isMoving = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D));
//         return isMoving;
//     }
//     
//
// }
