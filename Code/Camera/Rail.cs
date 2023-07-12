using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rail : MonoBehaviour
{
    private Vector3[] railNodes;
    private int nodeCount;

    private void Start()
    {
        nodeCount = transform.childCount;
        railNodes = new Vector3[nodeCount];

        for (int i = 0; i < nodeCount; i++)
        {
            railNodes[i] = transform.GetChild(i).position;
        }
    }

    private void Update()
    {
        if (nodeCount > 1)
        {
            for (int i = 0; i < nodeCount - 1; i++)
            {
                Debug.DrawLine(railNodes[i], railNodes[i + 1], Color.green);
            }
        }
    }

    public Vector3 ProjectPositionOnRail(Vector3 pos)
    {
        int closestNodeIndex = GetClosestNode(pos);

        if (closestNodeIndex == 0)
        {
            return ProjectOnSegment(railNodes[0], railNodes[1], pos);
        }
        else if (closestNodeIndex == nodeCount - 1)
        {
            return ProjectOnSegment(railNodes[nodeCount - 1], railNodes[nodeCount - 2], pos);
        }
        else
        {
            Vector3 leftSegment = ProjectOnSegment(railNodes[closestNodeIndex - 1], railNodes[closestNodeIndex], pos);
            Vector3 rightSegment = ProjectOnSegment(railNodes[closestNodeIndex + 1], railNodes[closestNodeIndex], pos);

            Debug.DrawLine(pos, leftSegment, Color.red);
            Debug.DrawLine(pos, rightSegment, Color.blue);

            if ((pos - leftSegment).sqrMagnitude <= (pos - rightSegment).sqrMagnitude)
            {
                return leftSegment;
            }
            else
            {

                return rightSegment;
            }
        }
    }

    private int GetClosestNode(Vector3 pos)
    {
        int closestNodeIndex = -1;
        float shortestDistance = 0.0f;

        for (int i = 0; i < nodeCount; i++)
        {
            float sqrDistance = (railNodes[i] - pos).sqrMagnitude;
            if (shortestDistance == 0.0f || sqrDistance < shortestDistance)
            {
                shortestDistance = sqrDistance;
                closestNodeIndex = i;
            }
        }


        return closestNodeIndex;
    }

    private Vector3 ProjectOnSegment(Vector3 v1, Vector3 v2, Vector3 pos)
    {
        Vector3 v1ToPos = pos - v1;
        Vector3 segmentDirection = (v2 - v1).normalized;

        float distanceFromV1 = Vector3.Dot(segmentDirection, v1ToPos);

        if (distanceFromV1 < 0.0f)
        {
            return v1;
        }
        else if (distanceFromV1 * distanceFromV1 > (v2 - v1).sqrMagnitude)
        {
            return v2;
        }
        else
        {
            Vector3 fromV1 = segmentDirection * distanceFromV1;
            return v1 + fromV1;
        }
    }
}
