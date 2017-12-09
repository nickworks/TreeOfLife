using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSticky : MonoBehaviour {

    List<Transform> stuckObjects = new List<Transform>();

    Vector3 prevPosition = new Vector3();

    void OnTriggerEnter(Collider other)
    {
        stuckObjects.Add(other.transform);
    }
    void OnTriggerExit(Collider other)
    {
        stuckObjects.Remove(other.transform);
    }
    void LateUpdate()
    {
        Vector3 delta = transform.position - prevPosition;
        prevPosition = transform.position;

        foreach(Transform t in stuckObjects)
        {
            PawnAABB3D pawn = t.GetComponent<PawnAABB3D>();
            if (pawn)
            {
                PawnAABB3D.CollisionResults results = pawn.Move(delta);
                
                // TODO: move into PAWNAABB3D class?

                // convert local distance into world space
                Vector3 worldSpaceDistance = t.TransformVector(results.distanceLocal);
                // add to player position
                t.transform.position += worldSpaceDistance;
            }
            else
            {
                t.transform.Translate(delta);
            }
        }
    }
}
