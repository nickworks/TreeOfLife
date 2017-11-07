using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class allows a GameObject to align with a path. A path is comprised of one or more PathNode objects.
/// </summary>
public class AlignWithPath : MonoBehaviour {

    /// <summary>
    /// This value represents the percentage of how far this object is from the current
    /// PathNode to the next (right) node. The value is calculated in PathNode.CalcOverallPercent()
    /// and then it can be passed into PathNode.GetCameraData() to get the interpolated
    /// camera data from this node to the next. Negative values are possible and will be
    /// used to interpolate to the left node.
    /// </summary>
    private float percentValueOnPath;
    /// <summary>
    /// This stores a reference to the current PathNode within a path. This script calls the
    /// node's Constrain() method to retrieve constrained position and rotation values.
    /// </summary>
    public PathNode currentNode;

    void Start()
    {
        Teleport();
    }
    /// <summary>
    /// This will teleport this object to a specific PathNode.
    /// </summary>
    /// <param name="node">The PathNode object to teleport to. If null, this object will instead attempt to teleport to currentNode.</param>
    public void Teleport(PathNode node = null)
    {
        if (node == null) node = currentNode;
        if (node != null) transform.position = node.transform.position;
    }

	/// <summary>
    /// Every time this component is updated, it calls ProjectionOntoPath()
    /// </summary>
	void Update () {
        ProjectionOntoPath();
    }
    /// <summary>
    /// This method calls the currentNode's Constrain() method, and then uses the returned position
    /// and rotation data to modify this object's position and rotation. If Constrain() returns
    /// a new PathNode, the new node is stored as currentNode and then the method calls itself
    /// to re-attmempt projection.
    /// </summary>
    /// <param name="recursions">This keeps track of how many times the method has recursed. Recursion should only happen once, but if it happens 3 or more times an Exception is thrown.</param>
    void ProjectionOntoPath(int recursions = 0)
    {
        if (!currentNode) return;
        if(recursions > 2)
        {
            throw new System.Exception("Recursive path switching in AlignWithPath. This shouldn't happen. Submit a bug to the issue tracker.");
        }

        PathNode.ProjectionResults results = currentNode.Constrain(transform);
        if (results.newNode != null)
        {
            currentNode = results.newNode;
            ProjectionOntoPath(recursions + 1);
        }
        else
        {
            if (!float.IsNaN(results.percent)) percentValueOnPath = results.percent;
            results.position.y = transform.position.y;
            transform.rotation = results.rotation;
            transform.position = results.position;
        }
    }
    /// <summary>
    /// This method uses the stored percentValueOnPath to ask the currentNode for interpolated camera data.
    /// </summary>
    /// <returns>The interpolated camera data that dictates how a camera should look at this object.</returns>
    public CameraDataNode.CameraData GetCameraData()
    {
        if (!currentNode) return null;
        return currentNode.GetCameraData(percentValueOnPath);
    }
}
