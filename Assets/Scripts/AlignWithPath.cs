using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithPath : MonoBehaviour {

    public PathNode currentNode;


	// Update is called once per frame
	void Update () {
        if (!currentNode) return;

        PathNode.ProjectionResults results = currentNode.Constrain(transform);
        if(results.newNode != null)
        {
            currentNode = results.newNode;
        } else
        {
            results.position.y = transform.position.y;
            transform.rotation = results.rotation;
            transform.position = results.position;
        }
    }
}
