using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithPath : MonoBehaviour {

    public PathNode currentNode;
	
	// Update is called once per frame
	void Update () {
        AlignRotation();
        AlignPosition();
	}
    void AlignPosition()
    {
        Vector3 pos = transform.position;
        Vector3 p = ClosestPoint();
        pos.x = p.x;
        pos.z = p.z;
        transform.position = pos;
    }
    Vector3 ClosestPoint()
    {
        // TODO: this might be unnecessary...
        // If we align the player's rotation so that they're aligned with the path,
        // Then we know they simply need to be moved along their forward or backward
        // vector to reach the path...

        if (!currentNode || !currentNode.right) return transform.position;
        Vector3 nodeToThis = transform.position - currentNode.transform.position;
        nodeToThis.y = 0;

        Vector3 segment = VectorToNextNode();
        segment.y = 0;

        Vector3 axis = segment.normalized;
        float projectedPoint = Vector3.Dot(axis, nodeToThis);

        if (projectedPoint <= 0) return currentNode.transform.position;
        if (projectedPoint >= VectorToNextNode().magnitude) return currentNode.right.transform.position;

        Vector3 result = axis * projectedPoint;

        return result + currentNode.transform.position;
    }
    Vector3 VectorToNextNode()
    {
        if (!currentNode || !currentNode.right) return Vector3.zero;
        return currentNode.right.transform.position - currentNode.transform.position;
    }
    void AlignRotation()
    {
        if (!currentNode || !currentNode.right) return;
        Vector3 diff = VectorToNextNode();
        float angle = Mathf.Atan2(-diff.z, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
