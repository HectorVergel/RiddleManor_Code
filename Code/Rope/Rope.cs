using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform startPos;
    public Transform holder;
    public LineRenderer rope;
    public float maxLength;
    float currentLenght;
    public float maxWidth;
    public float minWidth;
    [Range(0f,0.1f)]
    public float tolerance = 0.01f;
    public GameObject colliderPrefab;
    List<RopePoint> ropePositions = new List<RopePoint>();
    bool onUse;
    LayerMask layerMask;
    List<BoxCollider> colliders = new List<BoxCollider>();
    PusheableObject holderPusheable;

    private void Awake()
    {
       AddPosToRope(startPos.transform.position,Vector3.zero,null);
    }
    private void Start() {
        holderPusheable = holder.GetComponent<PusheableObject>();
        holder.SetParent(null);
        startPos.SetParent(null);
        transform.position = Vector3.zero;
        layerMask = Physics.AllLayers;
        layerMask &= ~(1 << LayerMask.NameToLayer("Rope"));
        layerMask &= ~(1 << LayerMask.NameToLayer("Player"));
        layerMask &= ~(1 << LayerMask.NameToLayer("Character"));
        MaxLenghtReached();
        UpdateRopeGraphics();
        AddLastCollider();
        UpdateLineWidth();
    }
    /* private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if(ropePositions.Count <= 2) return;
        Vector3 lastPointToPlayer = holder.position-ropePositions[ropePositions.Count - 2].point;
        Vector3 lastPoint = ropePositions[ropePositions.Count - 2].point + lastPointToPlayer.normalized*0.5f + lastPointToPlayer*0.3f;
        Gizmos.DrawLine(holder.position,ropePositions[ropePositions.Count - 3].point);
        Gizmos.DrawLine(lastPoint,ropePositions[ropePositions.Count - 3].point);
    } */
    private void Update()
    {
        if(onUse) UpdateRope();
    }
    void UpdateRope()
    {
        UpdateRopeGraphics();
        LastSegmentGoToPlayerPos();
        DetectCollisionEnter();
        if (ropePositions.Count > 2) DetectCollisionExits();
        CheckLength();
        UpdateLineWidth();
    }

    void DetectCollisionEnter()
    {
        RaycastHit hit;
        Vector3 pointPos = ropePositions[ropePositions.Count - 2].point;
        if (Physics.Linecast(holder.position+new Vector3(0,0.8f,0), pointPos, out hit,layerMask,QueryTriggerInteraction.Ignore))
        {
            Vector3 hitPoint = hit.point+hit.normal*tolerance;
            AddPosToRope(hitPoint,hit.normal,hit.collider.GetComponent<Pilar>());
        }
    }

    void DetectCollisionExits()
    {
        RaycastHit hit;
        Vector3 pointPos = ropePositions[ropePositions.Count - 3].point;
        Vector3 lastPointToPlayer = holder.position+new Vector3(0,0.8f,0)-ropePositions[ropePositions.Count - 2].point;
        Vector3 lastPoint = ropePositions[ropePositions.Count - 2].point + lastPointToPlayer*0.2f;
        if (!Physics.Linecast(holder.position+new Vector3(0,0.8f,0), pointPos, out hit,layerMask,QueryTriggerInteraction.Ignore))
        {
            if(!Physics.Linecast(lastPoint, pointPos, out hit,layerMask,QueryTriggerInteraction.Ignore))
            {
                if(ropePositions[ropePositions.Count - 2].pilar!=null) ropePositions[ropePositions.Count - 2].pilar.OnExitCollision();
                ropePositions.RemoveAt(ropePositions.Count - 2);
                RemoveLastCollider();
            }
        }
    }

    void AddPosToRope(Vector3 _pos,Vector3 _normal,Pilar pilar)
    {
        if(ropePositions.Count>0) ropePositions.RemoveAt(ropePositions.Count - 1);
        ropePositions.Add(new RopePoint(_pos,_normal,pilar));
        if(pilar!=null) pilar.OnCollision();
        ropePositions.Add(new RopePoint(holder.position+new Vector3(0,0.8f,0),Vector3.zero,null));
        if(ropePositions.Count>2) AddCollider();
    }

    void UpdateRopeGraphics()
    {
        rope.positionCount = ropePositions.Count;

        for (int i = 0; i < rope.positionCount; i++)
        {
            Vector3 graphicPoint = ropePositions[i].point + ropePositions[i].normal * (rope.widthMultiplier/2-tolerance);
            rope.SetPosition(i,graphicPoint);
        }
    }
    void UpdateLineWidth()
    {
        float currentWidth = currentLenght/maxLength*(maxWidth-minWidth);
        currentWidth = maxWidth-currentWidth;
        rope.widthMultiplier = currentWidth;
        for (int i = 0; i < colliders.Count; i++)
        {
            if(i+1 > rope.positionCount-1) continue;
            Vector3 pointA = rope.GetPosition(i+1);
            Vector3 pointB = rope.GetPosition(i);
            Vector3 pointAB = pointB-pointA;
            
            colliders[i].transform.position = pointB-pointAB/2;
            colliders[i].size = new Vector3(colliders[i].size.x,currentWidth,currentWidth);
        }
    }
    void AddCollider()
    {
        Vector3 pointA = ropePositions[ropePositions.Count-2].point;
        Vector3 pointB = ropePositions[ropePositions.Count-3].point;
        Vector3 pointAB = pointB-pointA;
        GameObject colGameObject = Instantiate(colliderPrefab,pointB-pointAB/2,Quaternion.identity,transform);
        colGameObject.transform.forward = Vector3.Cross(pointAB,Vector3.up);
        BoxCollider box = colGameObject.GetComponent<BoxCollider>();
        box.size = new Vector3(Vector3.Distance(pointA,pointB),rope.widthMultiplier,rope.widthMultiplier);
        colliders.Add(box);
    }
    void AddLastCollider()
    {
        Vector3 pointA = rope.GetPosition(rope.positionCount-1);
        Vector3 pointB = rope.GetPosition(rope.positionCount-2);
        Vector3 pointAB = pointB-pointA;
        GameObject colGameObject = Instantiate(colliderPrefab,pointB-pointAB/2,Quaternion.identity,transform);
        colGameObject.transform.forward = Vector3.Cross(pointAB,Vector3.up);
        BoxCollider box = colGameObject.GetComponent<BoxCollider>();
        box.size = new Vector3(Vector3.Distance(pointA,pointB),rope.widthMultiplier,rope.widthMultiplier);
        colliders.Add(box);
    }
    void RemoveLastCollider()
    {
        Destroy(colliders[colliders.Count-1].gameObject);
        colliders.RemoveAt(colliders.Count-1);
    }
    void CheckLength()
    {
        if(MaxLenghtReached()) holderPusheable.SetConstraint(true,rope.GetPosition(rope.positionCount-2)-rope.GetPosition(rope.positionCount-1));
        else holderPusheable.SetConstraint(false,Vector3.zero);
    }
    public bool MaxLenghtReached()
    {
        float distance = 0;
        for (int i = 1; i < rope.positionCount; i++)
        {
            distance += Vector3.Distance(rope.GetPosition(i-1),rope.GetPosition(i));
        }
        currentLenght = distance;
        return distance > maxLength;
    }
    public void SetUse(bool state)
    {
        onUse = state;
        if(onUse) RemoveLastCollider();
        else AddLastCollider();
    }
    void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, holder.position+new Vector3(0,0.8f,0));
    public struct RopePoint
    {
        public RopePoint(Vector3 _point,Vector3 _normal,Pilar _pilar)
        {
            point = _point;
            normal = _normal;
            pilar = _pilar;
        }
        public Vector3 point;
        public Vector3 normal;
        public Pilar pilar;
    }
}
