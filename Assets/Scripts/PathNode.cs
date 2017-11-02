using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A single node for building paths. This class forms the backbone of a linked list of nodes.
/// </summary>
public class PathNode : MonoBehaviour {

    public static PathNode lastCheckpoint;

    public bool isACheckpoint = false;
    /// <summary>
    /// The prefab to use to spawn more PathNodes. This feels strangely recursive...
    /// </summary>
    public PathNode pathNodePrefab;
    /// <summary>
    /// The PathNode to the left of this one. This is the previous item in the linked List.
    /// </summary>
    [HideInInspector]
    public PathNode left;
    /// <summary>
    /// The PathNode to the right of this one. This is the next item in the linked list.
    /// </summary>
    [HideInInspector]
    public PathNode right;
    /// <summary>
    /// How much to curve this corner (if space allows).
    /// </summary>
    [Range(0, 10)] public float curveRadius = 0;
    private float clampedCurveRadius = 0;

    public Vector3 curveIn { get; private set; }
    public Vector3 curveOut { get; private set; }

    /// <summary>
    /// The yaw-rotation of the segment between this node and the next (right) node.
    /// </summary>
    private Quaternion rotationYaw;
    /// <summary>
    /// The length from this node and the next (right) node.
    /// </summary>
    private float length;

    void Start()
    {
        CacheData(false);
    }
    void OnValidate()
    {
        CacheData(true);
    }
    void CacheData(bool shouldRipple, PathNode caller = null)
    {
        length = 0;
        curveIn = transform.position;
        curveOut = transform.position;
        clampedCurveRadius = 0;

        if (shouldRipple)
        {
            if (left && caller != left) left.CacheData(false, this); // ripple outwards
            if (right && caller != right) right.CacheData(false, this); // ripple outwards
        }

        if (right)
        {
            // if there's a node to the right, use it to calculate and cache values for length & rotationYaw
            Vector3 diff = VectorToRight();
            length = diff.magnitude;
            float angle = Mathf.Atan2(-diff.z, diff.x) * Mathf.Rad2Deg;
            rotationYaw = Quaternion.Euler(0, angle, 0);
        }       

        if (left && right)
        {

            float disToLeft = Vector3.Distance(transform.position, left.transform.position);
            float disToRight = Vector3.Distance(transform.position, right.transform.position);

            clampedCurveRadius = Mathf.Min(curveRadius, 0.5f * disToLeft, 0.5f * disToRight);
            if (clampedCurveRadius < 0) clampedCurveRadius = 0;
            Vector3 p1 = left.transform.position;
            Vector3 p2 = transform.position;
            Vector3 p3 = right.transform.position;
            curveIn = p2 - (p2 - p1).normalized * clampedCurveRadius;
            curveOut = p2 - (p2 - p3).normalized * clampedCurveRadius;
        }
    }

    public struct ProjectionResults
    {
        public PathNode newNode;
        public Vector3 position;
        public Quaternion rotation;
        public ProjectionResults(Transform t)
        {
            newNode = null;
            position = t.position;
            rotation = t.rotation;
        }
    }
    public ProjectionResults Constrain(Transform obj)
    {
        ProjectionResults results = new ProjectionResults(obj);

        if (!right && !left) return results; // hmmmm...

        float p = 0;

        if (right)
        {
            // project onto straight segment:
            p = ProjectOnSegment(results.position, curveOut, right.curveIn); 

            if (p < 0) // left of straight segment
            {
                if (clampedCurveRadius <= 0 || curveIn == curveOut)
                {
                    // if there's no curved segment, switch to the left node:
                    if (left) results.newNode = left;
                }
                else
                {
                    // project onto curve
                    p = ProjectOnSegment(results.position, curveIn, curveOut);
                    if (p < 0 || p > 1) // if left (or right???) of curve:
                    {
                        if (left) results.newNode = left;
                    }
                    else // if on curve:
                    {
                        // FIXME: bug causes the player to automatically move in a curve...
                        results.position = OverwriteY(GetPointOnCurve(p), results.position.y);
                        results.rotation = GetRotationOnCurve(p);
                    }
                }
            }
            else if (p <= 1)
            { // on straight segment:
                results.position = OverwriteY(Vector3.Lerp(curveOut, right.curveIn, p), results.position.y);
                results.rotation = rotationYaw;
            }
            else // right of straight segment:
            {
                // switch to right node
                results.newNode = right;
            }
        } else // there is a node to the left, but no node to the right...
        {
            // in this situation, the player is "off the rail"
            p = ProjectOnSegment(results.position, left.curveOut, curveIn);
            if (p < 1) results.newNode = left; // back on the rail
        }

        return results;
    }
    public PathNode Split()
    {
        bool spawnToTheLeft = (this.right && !this.left);
        bool trueSplit = (this.right && this.left);

        // determine position of new node:
        Vector3 spawnPos = transform.position;
        if (trueSplit)
        {
            float length = (right.transform.position - left.transform.position).magnitude/3;

            spawnPos = Vector3.Lerp(transform.position, right.transform.position, .33f);
            transform.position = Vector3.Lerp(transform.position, left.transform.position, .33f);

        } else if(left)
        {
            spawnPos += (transform.position - left.transform.position);
        } else if(right)
        {
            spawnPos += (transform.position - right.transform.position);
        } else
        {
            spawnPos += Vector3.right * 10;
        }
        
        // spawn a new node:
        PathNode newNode = Instantiate(pathNodePrefab, spawnPos, Quaternion.identity);

        // insert the node into our linked list of nodes:

        if (spawnToTheLeft) // if this is the left-most node
        {
            // insert the node onto the left
            this.left = newNode;
            newNode.right = this;
            this.left.transform.position += Vector3.left;
        }
        else
        {
            // insert the node onto the right
            newNode.left = this;
            newNode.right = this.right;
            if (this.right) this.right.left = newNode;
            this.right = newNode;
        }
        
        return newNode; // return the new node
    }
    public void RemoveAndDestroy()
    {
        if (left) left.right = right;
        if (right) right.left = left;
        DestroyImmediate(gameObject);
    }
    public void RenameNodes(string name)
    {
        PathNode currentNode = GetLeftMostNode();
        for(int i = 0; currentNode; i++) {
            currentNode.name = name + i;
            currentNode = currentNode.right;
        }
    }
    public PathNode GetLeftMostNode()
    {
        PathNode leftMostNode = this;
        while (leftMostNode.left) leftMostNode = leftMostNode.left;
        return leftMostNode;
    }
    Vector3 OverwriteY(Vector3 v, float y = 0)
    {
        return new Vector3(v.x, y, v.z);
    }
    float ProjectOnSegment(Vector3 target, Vector3 start, Vector3 end)
    {
        Vector3 diff = OverwriteY(target - start);
        Vector3 axis = OverwriteY(end - start);

        float length = axis.magnitude;
        float projection = Vector3.Dot(axis.normalized, diff);

        return projection / length; // return a percentage
    }
    float ProjectOnCurveSection(Vector3 target)
    {
        Vector3 toTarget = target - curveIn;
        toTarget.y = 0;

        Vector3 segment = curveOut - curveIn;
        segment.y = 0;

        Vector3 axis = segment.normalized;
        float projection = Vector3.Dot(axis, toTarget);

        return projection;
    }
    Vector3 VectorToRight()
    {
        if (!right) return Vector3.zero;
        return right.transform.position - transform.position;
    }

    void OnDrawGizmos()
    {
        DrawLines();
        Gizmos.DrawIcon(transform.position, "icon-path.png", true);
    }
    void DrawLines()
    {
        if (transform.hasChanged) CacheData(false);

        if (left) Gizmos.DrawLine(curveIn, left.curveOut);
        if (right) Gizmos.DrawLine(curveOut, right.curveIn);

        if (left && right)
        {
            Gizmos.DrawWireCube(curveIn, Vector3.one * .1f);
            Gizmos.DrawWireCube(curveOut, Vector3.one * .1f);

            int segments = 10;
            Vector3 pt1 = curveIn;
            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;

                Vector3 pt2 = GetPointOnCurve(t);
                Gizmos.DrawLine(pt1, pt2);
                pt1 = pt2;
            }
            Gizmos.DrawLine(pt1, curveOut);
        }
    }
    Vector3 GetPointOnCurve(float t)
    {
        return Vector3.Lerp(Vector3.Lerp(curveIn, transform.position, t), Vector3.Lerp(transform.position, curveOut, t), t);
    }
    Quaternion GetRotationOnCurve(float t)
    {
        if (!left) return rotationYaw;
        return Quaternion.Slerp(left.rotationYaw, rotationYaw, t);
    }
}
