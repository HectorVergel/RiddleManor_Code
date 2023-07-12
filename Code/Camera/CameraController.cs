using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }
    Camera cam;
    BoxCollider box;
    Transform target;
    Transform lastTarget;
    public RoomTrigger firstRoom;
    public float height;
    public float depth;
    public float angleOffset;
    public float lateralOffset;
    public float lateralSpeed;
    public float verticalOffset;
    public float verticalSpeed;
    public float changeFocusMultiplier;
    public float catchUpSpeed;
    public float cinematicMultiplier;
    float currentLateralSpeed;
    float currentVerticalSpeed;
    float angle;
    float extraDepth;
    float extraHeight;
    float xMin;
    float xMax;
    float zMin;
    float zMax;
    Vector2 tempDirection;
    Vector2 direction;
    bool cinematic;
    PusheableObject pusheable;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        currentLateralSpeed = lateralSpeed;
        currentVerticalSpeed = verticalSpeed;
        Load();
        cam = GetComponent<Camera>();
        transform.rotation = Quaternion.Euler(angle,0,0);
        ChangeFocus(PlayerController.instance.cameraFocus);
        firstRoom.ChangeRoom();
        Vector3 targetPos = target.position + new Vector3(direction.x,0,direction.y) * lateralOffset;
        float xPos = Mathf.Clamp(targetPos.x,xMin,xMax);
        float yPos = targetPos.y + height + extraHeight;
        float zPos = Mathf.Clamp(target.position.z - depth - extraDepth,zMin,zMax);
        transform.position = new Vector3(xPos,yPos,zPos);
    }
    private void OnEnable() {
        InputManager.GetAction("Move").action += OnMovementInput;
        PlayerController.instance.OnBookActivated += () => ChangeFocus(Book.instance.bookGhost.transform);
        PlayerController.instance.OnPlayerActivated += () => ChangeFocus(PlayerController.instance.cameraFocus);
    }
    private void OnDisable() {
        InputManager.GetAction("Move").action -= OnMovementInput;
        PlayerController.instance.OnBookActivated -= () => ChangeFocus(Book.instance.bookGhost.transform);
        PlayerController.instance.OnPlayerActivated -= () => ChangeFocus(PlayerController.instance.cameraFocus);
    }
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        tempDirection = context.ReadValue<Vector2>();
        tempDirection.Normalize();
    }

    private void Update()
    {
        SetSpeed();
        HandleDirection();
        HandlePosition();
        HandleRotation();
    }
    void SetSpeed()
    {
        if(cinematic) return;

        if(currentLateralSpeed<lateralSpeed)
        {
            currentLateralSpeed = Mathf.Clamp(currentLateralSpeed+=catchUpSpeed*Time.deltaTime,0,lateralSpeed);
        }
        if(currentVerticalSpeed<verticalOffset)
        {
            currentVerticalSpeed = Mathf.Clamp(currentVerticalSpeed+=catchUpSpeed*Time.deltaTime,0,verticalSpeed);
        }
    }
    void HandleDirection()
    {
        if(cinematic) direction = Vector2.zero;
        else
        {
            if(pusheable!=null)
            {
                if(pusheable.focus!=null)
                {
                    Vector2 pushDirection = cam.WorldToScreenPoint(pusheable.focus.position) - cam.WorldToScreenPoint(PlayerController.instance.cameraFocus.position);
                    pushDirection = Vector2.ClampMagnitude(pushDirection,lateralOffset);
                    pushDirection = Vector2.ClampMagnitude(pushDirection,pushDirection.magnitude/lateralOffset);

                    direction += pushDirection * PlayerController.instance.acceleration * Time.deltaTime;
                }
                else
                {
                    if (tempDirection != Vector2.zero) direction += tempDirection * PlayerController.instance.acceleration * Time.deltaTime;
                    else direction -= direction * PlayerController.instance.acceleration * Time.deltaTime;
                }
            }
            else
            {
                if (tempDirection != Vector2.zero) direction += tempDirection * PlayerController.instance.acceleration * Time.deltaTime;
                else direction -= direction * PlayerController.instance.acceleration * Time.deltaTime;
            }

            direction = Vector2.ClampMagnitude(direction, 1);
        }
    }
    private void HandlePosition()
    {
        Vector3 targetPos = target.position + new Vector3(direction.x,0,direction.y) * lateralOffset;
        float xPos = Mathf.Clamp(targetPos.x,xMin,xMax);
        float yPos = targetPos.y + height + extraHeight;
        float zPos = Mathf.Clamp(targetPos.z - depth - extraDepth,zMin,zMax);
        targetPos = new Vector3(xPos,yPos,zPos);
        if(!cinematic) transform.position = new Vector3(Mathf.Lerp(transform.position.x,xPos,Time.deltaTime * currentLateralSpeed),Mathf.Lerp(transform.position.y,yPos,Time.deltaTime * currentLateralSpeed),Mathf.Lerp(transform.position.z,zPos,Time.deltaTime * currentLateralSpeed));
        else 
        {
            yPos = target.position.y + height + extraHeight;
            zPos = target.position.z - depth - extraDepth;
            targetPos = new Vector3(target.position.x,yPos,zPos);
            transform.position = Vector3.Lerp(transform.position,targetPos,Time.deltaTime*currentLateralSpeed);
        }
    }
    private void HandleRotation()
    {
        transform.rotation = Quaternion.Euler(Mathf.Lerp(transform.rotation.eulerAngles.x,angle-direction.y*verticalOffset,Time.deltaTime * currentVerticalSpeed),0,0);
    }
    public void ChangeRoom(BoxCollider box, float extraHeight, float extraDepth)
    {
        this.box = box;
        this.extraHeight = extraHeight;
        this.extraDepth = extraDepth;
        angle = 90 - Vector2.Angle(new Vector2(depth+extraDepth,height+extraHeight).normalized, new Vector2(0,height+extraHeight).normalized);
        angle-=angleOffset;
        xMax = box.bounds.center.x + box.bounds.extents.x;
        xMin = box.bounds.center.x - box.bounds.extents.x;
        zMax = box.bounds.center.z + box.bounds.extents.z;
        zMin = box.bounds.center.z - box.bounds.extents.z;
        currentLateralSpeed = lateralSpeed*changeFocusMultiplier;
        currentVerticalSpeed = verticalSpeed*changeFocusMultiplier;
    }
    public void ChangeFocus(Transform target)
    {
        if(!cinematic) this.target = target;
        lastTarget = target;
        pusheable = target.GetComponentInParent<PusheableObject>();
        currentLateralSpeed = lateralSpeed*changeFocusMultiplier;
        currentVerticalSpeed = verticalSpeed*changeFocusMultiplier;
    }
    public float MaxBookHeight()
    {
        return box.bounds.center.y + box.bounds.extents.y;
    }
    public void Cinematic(Cinematic cine,Transform target, float time)
    {
        this.target = target;
        cinematic = true;
        StopAllCoroutines();
        StartCoroutine(CinematicTime(cine,time));
    }
    IEnumerator CinematicTime(Cinematic cine, float time)
    {
        currentLateralSpeed=cinematicMultiplier*lateralSpeed;
        currentVerticalSpeed=verticalSpeed*cinematicMultiplier;
        PlayerController.instance.BlockPlayerInputs(false);
        cinematic = true;
        yield return new WaitForSeconds(time);
        PlayerController.instance.BlockPlayerInputs(true);
        cinematic = false;
        currentLateralSpeed=lateralSpeed;
        currentVerticalSpeed=verticalSpeed;
        if(cine.nextCinematic!=null) cine.nextCinematic.InstantCinematic();
        else ChangeFocus(lastTarget);
    }
    private void Load()
    {
        firstRoom = GameSaveManager.instance.GetCurrentRoom();
    }
}