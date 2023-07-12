using System.Collections.Generic;
using UnityEngine;

public class MirrorObject : MonoBehaviour
{
    private bool isValid;
    public int id;
    private List<WorldObject> worldObjects = new List<WorldObject>();
    private float distanceToDetect;
    private Collider myCollider;

    private void Start()
    {
        SetColliderPosition();
    }
    private void Update()
    {
        CheckList();
    }

    private void SetColliderPosition()
    {
        myCollider = GetComponent<Collider>();

        float distanceToReference = MirrorPuzzleManager.instance.planeReference.position.z - myCollider.transform.position.z;
        Vector3 newPosition = Vector3.zero;

        newPosition.x = myCollider.transform.position.x;
        newPosition.z = (myCollider.transform.position.z + (2 * distanceToReference));
        newPosition.y = myCollider.transform.position.y;

        myCollider.transform.position = newPosition;
    }
    private void CheckList()
    {
        if (worldObjects.Count != 0)
        {

            foreach (WorldObject item in worldObjects)
            {
                float distance = Vector3.Distance(item.transform.position, gameObject.transform.position);
                if (distance < distanceToDetect)
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }

            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WorldObject>() != null)
        {
            if (other.GetComponent<WorldObject>().id == id)
            {
                worldObjects.Add(other.GetComponent<WorldObject>());
            }
        }
    }
    public void SetDistance(float d)
    {
        distanceToDetect = d;
    }
    public bool GetIsValid()
    {
        return isValid;
    }
}
