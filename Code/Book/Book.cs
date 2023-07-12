using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;
using System.Reflection;

public class Book : MonoBehaviour
{
    public static Book instance;
    Transform player;
    [Header("Shapeshift Params")]
    public float playerWidth;
    public int angleIterations;
    public int extraAngleIterationsOnDistance;
    public float distanceIterations = 1;
    public float iterationDistance;
    public float groundDetectionDistance;
    [Header("Book Params")]
    public GameObject bookGraphics;
    public GameObject bookGhost;
    public Transform dialoguePosition;
    public VisualEffect particles;
    GameObject shapeshiftedObject;
    public Material defaultRuneMat;
    public Material fixedRuneMat;
    public float runeFadeSpeed;
    public Transform attractor;
    public Material ghostMat;

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bookGhost.transform.position,bookGhost.transform.position+new Vector3(0,-groundDetectionDistance,0));
        Gizmos.DrawLine(bookGhost.transform.position,bookGhost.transform.position+new Vector3(iterationDistance,0,0));
        Gizmos.DrawLine(bookGhost.transform.position,bookGhost.transform.position+new Vector3(-playerWidth,0,0));
    }
    private void Awake() {
        if(instance==null)
        {
            instance = this;
        }
        else Destroy(this);
    }
    private void Start() {
        particles.Stop();
        Color newColor = defaultRuneMat.GetColor("_BaseColor");
        defaultRuneMat.SetColor("_BaseColor", new Color(newColor.r,newColor.g,newColor.b,0));
        player = PlayerController.instance.transform;
        bookGhost.SetActive(false);
        attractor = attractor = GameObject.FindGameObjectWithTag("BookAttractor").transform;
    }
    private void OnEnable() {
        PlayerController.instance.OnBookActivated += ActivateBook;
        PlayerController.instance.OnPlayerActivated += DeactivateBook;
    }
    private void OnDisable() {
        PlayerController.instance.OnBookActivated -= ActivateBook;
        PlayerController.instance.OnPlayerActivated -= DeactivateBook;
    }
    public void ActivateBook()
    {
        StopAllCoroutines();
        StartCoroutine(ShowRunes());
        ResetBookGraphics();
        bookGhost.SetActive(true);
        AudioManager.Play("bookTransform"+Random.Range(1,5).ToString()).Volume(1f);
    }
    public void DeactivateBook()
    {
        StopAllCoroutines();
        StartCoroutine(HideRunes());
        bookGhost.SetActive(false);
        AudioManager.Play("bookDetransformation").Volume(1f);
    }
    public void ResetBookGraphics()
    {
        if(shapeshiftedObject!=null)
        {
            MirrorFocusManager.instance.RemoveMirror();
            Destroy(shapeshiftedObject.gameObject);
            shapeshiftedObject = null;
            particles.transform.position = bookGraphics.transform.position;
            particles.Play();
        }
        bookGraphics.SetActive(true);
    }
    IEnumerator ShowRunes()
    {
        while(defaultRuneMat.GetColor("_BaseColor").a < 1)
        {
            defaultRuneMat.SetColor("_BaseColor",defaultRuneMat.GetColor("_BaseColor") + new Color(0,0,0,runeFadeSpeed*Time.deltaTime));
            yield return null;
        }
        Color newColor = defaultRuneMat.GetColor("_BaseColor");
        defaultRuneMat.SetColor("_BaseColor", new Color(newColor.r,newColor.g,newColor.b,1));
    }
    IEnumerator HideRunes()
    {
        while(defaultRuneMat.GetColor("_BaseColor").a > 0)
        {
            defaultRuneMat.SetColor("_BaseColor",defaultRuneMat.GetColor("_BaseColor") - new Color(0,0,0,runeFadeSpeed*Time.deltaTime));
            yield return null;
        }
        Color newColor = defaultRuneMat.GetColor("_BaseColor");
        defaultRuneMat.SetColor("_BaseColor", new Color(newColor.r,newColor.g,newColor.b,0));
    }
    void SpotFound(Vector3 pos, GameObject clone)
    {
        AudioManager.Play("bookTransformation").Volume(0.5f);
        shapeshiftedObject = Instantiate(clone,pos, clone.transform.rotation);
        MirrorFocusManager.instance.AddMirror(shapeshiftedObject.GetComponent<PusheableObject>());
        Shape shape = shapeshiftedObject.GetComponent<Shape>();
        Destroy(shape);
        particles.transform.position = bookGraphics.transform.position;
        particles.Play();
        bookGraphics.SetActive(false);
        shape.SetRune(fixedRuneMat);
        PlayerController.instance.SwapControl();
    }
    public void Shapehift(Shape shape, Vector3 extents)
    {
        ShapeType type = shape.type;
        float largerExtent = Mathf.Max(extents.x, extents.z);
        Vector3 verticalExtent = new Vector3(0,extents.y,0);

        for (int distI = 0; distI < distanceIterations; distI++)
        {
            float distance = playerWidth + largerExtent + iterationDistance*distI;
            
            for (int angI = 0; angI <= (angleIterations+(distI*extraAngleIterationsOnDistance))/2; angI++)
            {
                int extraAngles = distI*extraAngleIterationsOnDistance;
                int angle = 360/(angleIterations+extraAngles)*angI;
                bool inverted = false;
                for (int i = 0; i < 2; i++)
                {
                    if(angle == 180 || angle == 0) i++;
                    Vector3 desiredPosition =  player.position + OrientationToVector(player.eulerAngles.y + (inverted? -angle : angle)).normalized * distance + new Vector3(0,-0.06f,0);
                    inverted = true;
                    if(Overlapping(type,desiredPosition,largerExtent,verticalExtent,extents)) continue;
                    if(!VisibleToPlayer(desiredPosition+verticalExtent)) continue;
                    if(!OnGround(desiredPosition,new Vector2(extents.x,extents.z))) continue;
                    SpotFound(desiredPosition,shape.gameObject);
                    return;
                }
            }
        }
        AudioManager.Play("bookTransformationError").Volume(0.3f);//
        PlayerController.instance.SwapControl();
        BookUI.instance.ShowWarning();
    }
    bool Overlapping(ShapeType _type, Vector3 desiredPosition,float largerExtent, Vector3 verticalExtent, Vector3 extents)
    {
        switch (_type)
        {
            case ShapeType.Box:
            return Physics.CheckBox(desiredPosition+verticalExtent,extents,Quaternion.identity,Physics.AllLayers,QueryTriggerInteraction.Ignore);

            case ShapeType.Sphere:
            return Physics.CheckSphere(desiredPosition+verticalExtent,largerExtent,Physics.AllLayers,QueryTriggerInteraction.Ignore);

            case ShapeType.Capsule:
            Vector3 bottomPoint = new Vector3(0,largerExtent,0);
            Vector3 topPoint = new Vector3(0,2*verticalExtent.y - largerExtent,0);
            return Physics.CheckCapsule(desiredPosition+bottomPoint,desiredPosition+topPoint,largerExtent,Physics.AllLayers,QueryTriggerInteraction.Ignore);

            default:
            return true;
        }
    }
    bool VisibleToPlayer(Vector3 pos)
    {
        LayerMask _layer = Physics.AllLayers;
        _layer &= ~(1 << LayerMask.NameToLayer("Player"));
        _layer &= ~(1 << LayerMask.NameToLayer("Character"));
        return !Physics.Raycast(player.position, pos - player.position,Vector3.Distance(player.position,pos),_layer,QueryTriggerInteraction.Ignore);
    }
    bool OnGround(Vector3 pos,Vector2 bounds)
    {
        bool corner1 = Physics.Raycast(pos+new Vector3(bounds.x,0,bounds.y),Vector3.down,groundDetectionDistance);
        bool corner2 = Physics.Raycast(pos+new Vector3(bounds.x,0,-bounds.y),Vector3.down,groundDetectionDistance);
        bool corner3 = Physics.Raycast(pos+new Vector3(-bounds.x,0,-bounds.y),Vector3.down,groundDetectionDistance);
        bool corner4 = Physics.Raycast(pos+new Vector3(-bounds.x,0,bounds.y),Vector3.down,groundDetectionDistance);
        return corner1 && corner2 && corner3 && corner4;
    }
    Vector3 OrientationToVector(float angle)
    {
        angle = angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos (angle);
        float sin = Mathf.Sin (angle);

        return new Vector3 (sin, 0, cos);
    }
}
