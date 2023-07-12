using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    [Header("References")]
    [SerializeField] public CharacterController characterController;
    private PlayerAnimatorController animatorController;
    [SerializeField] float pushAnimationDuration;
    public Transform cameraFocus;

    [Header("Movement")]
    public float maxLinealSpeed = 7f;
    public float acceleration;
    [SerializeField] float rotationFractionPerFrame = 45f;
    Vector3 movement;
    private Vector2 tempDirection;
    private Vector2 movementAcceleration;


    [Header("Jumping")]
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float maxJumpTime = 0.5f;
    [SerializeField] private float gravityIncreseValue;
    [SerializeField] private float accelerationOnAirMultiplier;
    [SerializeField] private float linealSpeedAirMultiplier;
    bool onGround;
    float jumpForce;
    float gravity;
    float initialGravity;
    bool isJumping;
    bool canPush;

    [Header("Push")]
    [SerializeField] float closeEnoughtDetection = 0.3f;
    [SerializeField] float detectionHeight = 0.6f;
    [SerializeField] float pushForce = 40f;
    [SerializeField] float angleDot = 0.8f;
    [SerializeField] float distanceBetween = 0.1f;
    [SerializeField] Transform pushStartDetectionPoint;
    public PusheableObject currentObjectPushing;
    [NonSerialized]
    public bool bookOpened;
    bool inputBlocked;

    public delegate void BookActivated();
    public delegate void PlayerActivated();
    public delegate void ObjectPushed();
    public delegate void StoppedPushing();
    public event BookActivated OnBookActivated;
    public event PlayerActivated OnPlayerActivated;
    public event ObjectPushed OnObjectPushed;
    public event StoppedPushing OnStoppedPushing;

    private Vector3 myPosition;
    private bool isInputPressed;
    public float maxTimeForBreak = 6f;
    public float minTimeForBreak = 4f;
    private bool breakEnded = true;
    private IEnumerator idleCo;
    public bool isPaused;
    public float pauseDelay = 2f;
    public RoomTrigger lastRoomTriggerPlayer;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);
        SetUpJumpVariables();
        pushStartDetectionPoint.position = transform.position + new Vector3(0, detectionHeight, 0) + transform.forward * closeEnoughtDetection;
        animatorController = GetComponent<PlayerAnimatorController>();

    }
    private void Start()
    {
        LoadData();
    }
    public PlayerAnimatorController GetAnimator()
    {
        return animatorController;
    }
    private void OnEnable()
    {
        InputManager.GetAction("Move").action += OnMovementInput;
        InputManager.GetAction("ChangeMode").action += OnChangeModeInput;
        InputManager.GetAction("Push").action += OnPushInput;
        InputManager.GetAction("Jump").action += OnJumpInput;
    }
    private void OnDisable()
    {
        InputManager.GetAction("Move").action -= OnMovementInput;
        InputManager.GetAction("ChangeMode").action -= OnChangeModeInput;
        InputManager.GetAction("Push").action -= OnPushInput;
        InputManager.GetAction("Jump").action -= OnJumpInput;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pushStartDetectionPoint.position, pushStartDetectionPoint.position + transform.forward * closeEnoughtDetection);
    }
    void OnPushInput(InputAction.CallbackContext context)
    {
        if (context.started && !inputBlocked)
        {
            if (currentObjectPushing != null) StopPushing();
            else CheckPush();
        }
    }
    void OnChangeModeInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SwapControl();
        }
    }
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started) Jump();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        tempDirection = context.ReadValue<Vector2>();
        tempDirection.Normalize();
    }
    public void SwapControl()
    {
        if (isJumping || !onGround) return;
        if (bookOpened)
        {
            if (CanInteract()) return; //?
            InputManager.GetAction("Move").action += OnMovementInput;
            InputManager.GetAction("Push").action += OnPushInput;
            InputManager.GetAction("Jump").action += OnJumpInput;
            bookOpened = false;
            characterController.enabled = true;
            if (InputManager.GetAction("Move").GetEnabled())
            {
                tempDirection = InputManager.GetAction("Move").context.ReadValue<Vector2>().normalized;
            }
            movement = Vector3.zero;
            OnPlayerActivated?.Invoke();
            if(lastRoomTriggerPlayer !=null) lastRoomTriggerPlayer.ChangeRoom();
        }
        else
        {
            PressTutorial.instance.SetTutorial(false);
            StopPushing();
            InputManager.GetAction("Move").action -= OnMovementInput;
            InputManager.GetAction("Push").action -= OnPushInput;
            InputManager.GetAction("Jump").action -= OnJumpInput;
            bookOpened = true;
            animatorController.SetBool("isMoving", false);
            movement = Vector3.zero;
            characterController.enabled = false;
            OnBookActivated?.Invoke();
        }
    }
    private void SetUpJumpVariables()
    {
        maxJumpHeight += maxJumpHeight * 0.05f;
        float timeToApex = maxJumpTime / 2f; //The time to reach the maximum height of the jump.
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialGravity = gravity;
        jumpForce = (2 * maxJumpHeight) / timeToApex;
    }

    private void FixedUpdate()
    {
        if (currentObjectPushing != null && !isJumping) Push();
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = movement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = movement.z;
        Quaternion currentRotation = transform.rotation;

        if (tempDirection != Vector2.zero && positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFractionPerFrame * Time.deltaTime);
        }
    }
    void Update()
    {

        isInputPressed = (tempDirection != Vector2.zero || isJumping || currentObjectPushing != null) && !isPaused;
        if (!isInputPressed && breakEnded)
        {
            idleCo = TimeToBreakIdle();
            StartCoroutine(idleCo);
        }
        else if(!breakEnded && isInputPressed)
        {
            StopCoroutine(idleCo);
            breakEnded = true;
        }
        if (characterController.enabled)
        {
            HandleRotation();
            HandleAcceleration();
            CollisionFlags collisionFlags = characterController.Move(movement * Time.deltaTime);
            CheckCollision(collisionFlags);
            animatorController.SetFloat("VelY", movement.y);
            animatorController.SetBool("isMoving", tempDirection != Vector2.zero);
        }
        CheckPushAvailable();
        SetGravity();
        animatorController.SetBool("Grounded", onGround);
        animatorController.SetBool("InputPressed", isInputPressed);
    }

    public void UnsetPause()
    {
        isPaused = false;
        
    }
   
    IEnumerator TimeToBreakIdle()
    {
        float timer = 0f;
        float maxTime = Random.Range(minTimeForBreak, maxTimeForBreak);
        breakEnded = false;
        while (timer < maxTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        animatorController.PlayRandomIdle();
        idleCo = TimeToBreakIdle();
        StartCoroutine(idleCo);
    }
    IEnumerator DelayStartPushing()
    {
        yield return new WaitForSeconds(pushAnimationDuration);
        canPush = true;
        animatorController.SetBool("CanPush", canPush);
    }

    private void HandleAcceleration()
    {
        if (tempDirection != Vector2.zero)
        {
            movementAcceleration += isJumping ? tempDirection * acceleration * accelerationOnAirMultiplier * Time.deltaTime : tempDirection * acceleration * Time.deltaTime;
            movementAcceleration = Vector2.ClampMagnitude(movementAcceleration, tempDirection.magnitude);
        }
        else
        {

            movementAcceleration -= movementAcceleration * acceleration * Time.deltaTime;
            movementAcceleration = Vector2.ClampMagnitude(movementAcceleration, 1);

        }

        Vector2 currentSpeedVector = isJumping ? maxLinealSpeed * movementAcceleration * linealSpeedAirMultiplier : maxLinealSpeed * movementAcceleration;
        movement.x = currentSpeedVector.x;
        movement.z = currentSpeedVector.y;
    }
    private void Jump()
    {
        if (CanJump())
        {
            movement.y = jumpForce * .5f;
            isJumping = true;
            onGround = false;
            animatorController.SetTrigger("Jump");

        }
    }
    void CheckPushAvailable()
    {
        if (inputBlocked)
        {
            WorldScreenUI.instance.HideIcon(IconType.Push);
            return;
        }
        PusheableObject pusheable;
        if (CanInteract() && PusheableDetected(out pusheable, out RaycastHit hit))
        {
            WorldScreenUI.instance.SetIcon(IconType.Push, characterController.bounds.center + new Vector3(0, 1, 0));
        }
        else WorldScreenUI.instance.HideIcon(IconType.Push);
    }
    public bool CanInteract()
    {
        if (isJumping || !onGround || isPaused) return false;
        if (currentObjectPushing != null) return false;
        if (bookOpened) return false;
        if (!characterController.enabled) return false;
        return true;
    }
    bool PusheableDetected(out PusheableObject pusheable, out RaycastHit hit)
    {
        pusheable = null;
        hit = new RaycastHit();

        if (isJumping || !onGround) return false;

        Ray ray = new Ray(pushStartDetectionPoint.position, transform.forward);


        if (Physics.Raycast(ray, out hit, closeEnoughtDetection, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            pusheable = hit.collider.GetComponentInParent<PusheableObject>();
            if (pusheable != null && Vector3.Dot(transform.forward, -hit.normal) >= angleDot)
            {
                if(pusheable.canBePushed)
                {
                    transform.forward = -hit.normal;
                    return true;
                }
            }
        }
        return false;
    }
    void CheckPush()
    {
        PusheableObject pusheable;
        RaycastHit hit;
        if (PusheableDetected(out pusheable, out hit) && CanInteract())
        {
            canPush = false;
            animatorController.SetBool("CanPush", canPush);
            currentObjectPushing = pusheable;
            transform.SetParent(currentObjectPushing.transform);
            currentObjectPushing.MakePusheable();
            characterController.enabled = false;
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z) + hit.normal * (characterController.radius + distanceBetween);
            transform.forward = -hit.normal;
            StartCoroutine(DelayStartPushing());
            animatorController.SetBool("isPushing", true);

            OnObjectPushed?.Invoke();
        }
    }
    public void StopPushing()
    {
        if (!canPush) return;
        if (currentObjectPushing == null) return;
        OnStoppedPushing?.Invoke();
        transform.SetParent(null);
        currentObjectPushing.NotPusheable();
        animatorController.SetBool("isPushing", false);
        currentObjectPushing = null;
        characterController.enabled = true;
    }
    private void Push()
    {
        if (!canPush) return;
        currentObjectPushing.AddForceTowardsDirection(pushForce, tempDirection);
        if (tempDirection != Vector2.zero)
        {
            HandlePushAnimation();
        }
        else
        {
            animatorController.SetFloat("VelX", 0);
            animatorController.SetFloat("VelZ", 0);
        }

    }

    private void HandlePushAnimation()
    {

        var inputDirection = new Vector3(tempDirection.x, 0, tempDirection.y);
        var lookingDirection = new Vector3(transform.forward.x, 0, transform.forward.z);

        var angle = Vector3.Angle(lookingDirection, Vector3.forward);
        var cross = Vector3.Cross(lookingDirection, Vector3.forward);
        if (cross.y < 0)
            angle = -angle;

        var lookingRotation = Quaternion.AngleAxis(angle, Vector3.up);
        var inputRotation = lookingRotation * inputDirection;
        animatorController.SetFloat("VelX", inputRotation.x);
        animatorController.SetFloat("VelZ", inputRotation.z);


    }



    private bool CanJump()
    {
        return currentObjectPushing == null && !isJumping && onGround && !isPaused;
    }

    void SetGravity()
    {

        if (isJumping || !onGround)
        {
            gravity -= gravityIncreseValue * Time.deltaTime;


        }
        if (gravity >= -0.1f && gravity <= 0f)
        {
            gravity = Mathf.RoundToInt(gravity);
        }
        float previousYVelocity = movement.y;
        float newYVelocity = movement.y + (gravity * Time.deltaTime);
        float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;

        movement.y = nextYVelocity;


    }



    void CheckCollision(CollisionFlags collisionFlag)
    {
        if ((collisionFlag & CollisionFlags.Above) != 0 && movement.y > 0.0f)
        {
            movement.y = 0.0f;
        }

        if ((collisionFlag & CollisionFlags.Below) != 0 && movement.y < 0.0f)
        {
            onGround = true;
            movement.y = 0.0f;
            gravity = initialGravity;
            isJumping = false;

        }
        if ((collisionFlag & CollisionFlags.Below) == 0 && movement.y < -0.5f)
        {
            onGround = false;
        }
    }

    public bool GetIsJumping()
    {
        return isJumping;
    }

    public bool GetIsGrounded()
    {
        return onGround;
    }
    public Vector2 GetDirection()
    {
        if (currentObjectPushing != null) return tempDirection;
        else
        {
            Vector2 dir = new Vector2(movement.x, movement.z);
            dir = dir.normalized * dir.magnitude / maxLinealSpeed;
            return dir.magnitude > 0.01 ? dir : Vector2.zero;
        }
    }
    public void BlockPlayerInputs(bool state)
    {
        InputManager.ActionEnabled("Move", state);
        InputManager.ActionEnabled("ChangeMode", state);
        InputManager.ActionEnabled("Jump", state);
        movement = Vector3.zero;
        tempDirection = Vector2.zero;
        inputBlocked = !state;
        StopPushing();

    }
    private void LoadData()
    {
        transform.position = GameSaveManager.instance.GetSpawnPoint().position;
    }

}
