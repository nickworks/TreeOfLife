using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class allows a GameObject to align with a path. A path is comprised of one or more PathNode objects.
/// </summary>
public class AlignWithPath : MonoBehaviour {

    public PathNode currentNode;

    void Start()
    {
        Teleport();
    }
    public void Teleport(PathNode node = null)
    {
        if (node == null) node = currentNode;
        transform.position = node.transform.position;
    }

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
