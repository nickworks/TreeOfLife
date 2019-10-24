using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A single node for building paths. This class forms the backbone of a linked list of nodes.
/// </summary>
[ExecuteInEditMode]
public class PathNode : MonoBehaviour
{

    /// <summary>
    /// This holds the data that is returned from Constrain()
    /// </summary>
    public struct ProjectionResults
    {
        /// <summary>
        /// The overall percentage of the projection results. 0 is this node; 1 is right node; -1 is left node.
        /// </summary>
        public float percent;
        /// <summary>
        /// If this isn't null, then the constrained object should switch its currentNode to newNode.
        /// </summary>
        public PathNode newNode;
        /// <summary>
        /// The constrained position of the object.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// The constrained rotation of the object.
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="t">Use this transform to set the default position and rotation values.</param>
        public ProjectionResults(Transform t)
        {
            percent = float.NaN;
            newNode = null;
            position = t.position;
            rotation = t.rotation;
        }
    }

    /// <summary>
    /// A reference to a CameraDataNode component, if one exists on this GameObject.
    /// </summary>
    [HideInInspector]
    public CameraDataNode cameraDataNode;

    /// <summary>
    /// How much to curve this corner (if space allows).
    /// </summary>
    [Range(0, 50)] public float curveRadius = 0;
    /// <summary>
    /// This renders sharp turns as RED.
    /// </summary>
    static public bool showSharpTurns = true;
    /// <summary>
    /// The PathNode to the left of this one. This is the previous item in the linked List.
    /// </summary>
    //[HideInInspector]
    public PathNode left;
    /// <summary>
    /// The PathNode to the right of this one. This is the next item in the linked list.
    /// </summary>
    //[HideInInspector]
    public PathNode right;
    /// <summary>
    /// The left anchor point of the curved section. If there is no curved section, then this is equal to transform.position.
    /// </summary>
    public Vector3 curveIn { get; private set; }
    /// <summary>
    /// The right anchor point of the curved section. If there is no curved section, then this is equal to transform.position.
    /// </summary>
    public Vector3 curveOut { get; private set; }
    /// <summary>
    /// The yaw-rotation of the segment between this node and the next (right) node.
    /// </summary>
    private Quaternion rotationYaw;
    /// <summary>
    /// The length from this node and the next (right) node.
    /// </summary>
    private float length;
    /// <summary>
    /// The clamped radius. This is what's used to create the curved section of the curve.
    /// </summary>
    private float clampedCurveRadius;
    /// <summary>
    /// The center point of the curved section.
    /// </summary>
    public Vector3 curveCenter { get; private set; }
    /// <summary>
    /// The angle from curveCenter to the left edge of the curve.
    /// </summary>
    public float angleCurveIn { get; private set; }
    /// <summary>
    /// The angle from curveCenter to the right edge of the curve.
    /// </summary>
    public float angleCurveOut { get; private set; }
    /// <summary>
    /// How far (in percent) the curveOut is between this node and the neighboring left node. For performance, this is calculated once and cached.
    /// </summary>
    private float percentCurveIn;
    /// <summary>
    /// How far (in percent) the curveOut is between this node and the neighboring right node. For performance, this is calculated once and cached.
    /// </summary>
    private float percentCurveOut;
    /// <summary>
    /// When this object starts, we call CacheData()
    /// </summary>
    void Start()
    {
        CacheData(false);
    }
    /// <summary>
    /// When values in the inspector are updated, we call CacheData().
    /// </summary>
    void OnValidate()
    {
        CacheData(true);
    }
    /// <summary>
    /// This method calculates and caches rotation, distance, curveLeft, and curveRight.
    /// </summary>
    /// <param name="shouldRipple">Whether or not to tell neighbors to also recalc and cache. Rippling does not recurse, and so it only affects left and right.</param>
    void CacheData(bool shouldRipple)
    {
        cameraDataNode = GetComponent<CameraDataNode>();
        angleCurveIn = angleCurveOut = percentCurveIn = percentCurveOut = length = 0;
        curveCenter = curveIn = curveOut = transform.position;

        if (shouldRipple)
        {
            if (left) left.CacheData(false); // ripple outwards
            if (right) right.CacheData(false); // ripple outwards
        }

        if (right)
        {
            // if there's a node to the right, use it to calculate and cache values for length & rotationYaw
            Vector3 diff = right.transform.position - transform.position;
            length = diff.magnitude;
            float angle = Mathf.Atan2(-diff.z, diff.x) * Mathf.Rad2Deg;
            rotationYaw = Quaternion.Euler(0, angle, 0);
        }

        if (left && right)
        {
            clampedCurveRadius = curveRadius;

            Vector3 p1 = left.transform.position;
            Vector3 p2 = transform.position;
            Vector3 p3 = right.transform.position;

            Vector3 leftDiff = OverwriteY(p1 - p2); // get the FLAT vector to left
            Vector3 rightDiff = OverwriteY(p3 - p2); // get the FLAT vector to right

            Vector3 leftAxis = leftDiff.normalized; // direction to left
            Vector3 rightAxis = rightDiff.normalized; // direction to right

            // the YAW to angle1 and angle2
            float angleToP3 = Mathf.Atan2(rightDiff.z, rightDiff.x); // YAW from this to right
            float angleToP1 = Mathf.Atan2(leftDiff.z, leftDiff.x); // YAW from this to left
            angleToP3 = AngleWrapFromTo(angleToP3, angleToP1); // wrap the values into the same 180-degree arc

            if (Mathf.Abs(angleToP1 - angleToP3) >= Mathf.PI) // if in a straight line... don't try to curve.
            {
                clampedCurveRadius = 0;
                return;
            }

            float angleBetween = Mathf.Abs(angleToP3 - angleToP1) / 2; // save a step, and half the difference
            float angleToCenter = (angleToP3 + angleToP1) / 2; // angle from p2 to center... use the average of our previous two angles

            float maxAdjacent = Mathf.Min(leftDiff.magnitude, rightDiff.magnitude) * 0.5f;
            float cos = Mathf.Cos(angleBetween);
            float maxDistance = maxAdjacent / cos;
            float disToCenter = curveRadius / Mathf.Sin(angleBetween);

            if (maxDistance < disToCenter)
            {
                clampedCurveRadius *= maxDistance / disToCenter;
                disToCenter = maxDistance;
            }

            Vector3 axisToCenter = new Vector3(Mathf.Cos(angleToCenter), 0, Mathf.Sin(angleToCenter));
            curveCenter = transform.position + axisToCenter * disToCenter;

            float disToInOut = cos * disToCenter; // use trig to find FLAT distance to curveIn and curveOut

            percentCurveIn = disToInOut / leftDiff.magnitude; // percent of the way from p2 to p1
            percentCurveOut = disToInOut / rightDiff.magnitude; // percent of the way from p2 to p3

            curveIn = Vector3.Lerp(p2, p1, percentCurveIn);
            curveOut = Vector3.Lerp(p2, p3, percentCurveOut);

            angleCurveIn = CurveAngleTo(curveIn);
            angleCurveOut = CurveAngleTo(curveOut);
            angleCurveOut = AngleWrapFromTo(angleCurveOut, angleCurveIn);

        }
    }
    /// <summary>
    /// Gets the angle from the center of the curve to another point.
    /// </summary>
    /// <param name="pos">The position</param>
    /// <returns>The angle in radians</returns>
    private float CurveAngleTo(Vector3 pos)
    {
        return Mathf.Atan2(pos.z - curveCenter.z, pos.x - curveCenter.x);
    }
    /// <summary>
    /// Call this method to calculate a constained position and rotation for a given Transform
    /// </summary>
    /// <param name="obj">The Transform to constrain. This method will NOT modify the Transform.</param>
    /// <returns>This contains a final position, rotation, and (if necessary) new PathNode.</returns>
    public ProjectionResults Constrain(Transform obj)
    {
        ProjectionResults results = new ProjectionResults(obj);

        if (right)
        {
            float p = ProjectOnSegment(results.position, curveOut, right.curveIn);
            if (p > 1) results.newNode = right;
            else if (p >= 0) // on straight segment:
            {
                //results.cameraData = CameraRig.CameraData.Lerp(cameraData, right.cameraData, p);
                results.percent = CalcOverallPercent(p, false);
                results.position = OverwriteY(Vector3.Lerp(curveOut, right.curveIn, p), results.position.y);
                results.rotation = rotationYaw;
            }
            else if (left) // on the curve:
            {
                if (angleCurveIn == angleCurveOut) results.newNode = left;
                else
                {
                    p = ProjectOnCurve(results.position);
                    if (p < 0) results.newNode = left;
                    else if (p <= 1)
                    {
                        results.percent = CalcOverallPercent(p, true);
                        results.position = OverwriteY(GetPointOnCurve(p), results.position.y);
                        results.rotation = GetRotationOnCurve(p);
                    }
                    else
                    {
                        results.newNode = right; // this only happens when curveIn and curveOut are the same point
                    }
                    return results;
                }
            }
            return results;
        }
        if (left) // the player must be right of the right-most node...
        {
            // in this situation, the player is "off the rail"
            float p = ProjectOnSegment(results.position, left.curveOut, curveIn);
            if (p < 1) results.newNode = left; // back on the rail
        }

        return results;
    }
    /// <summary>
    /// Splits this PathNode into two PathNode objects. This affects the linked list of the path.
    /// </summary>
    /// <returns>The newly inserted PathNode.</returns>
    public PathNode Split()
    {
        // spawn a new node:

        //PathNode newNode = (PathNode)PrefabUtility.InstantiatePrefab(PrefabUtility.GetPrefabParent(this));
        PathNode newNode = (PathNode)PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(this));


        if (!newNode) return null; // FAILED TO SPAWN!

        // register the upcoming changes in the undo list: (this should also mark the objects as "dirty")
        string undoName = "Insert a new path node";
        Undo.RegisterCreatedObjectUndo(newNode.gameObject, undoName);
        RegisterLinkedListUndo(undoName);

        // determin the positions of the affected nodes:

        bool spawnToTheLeft = (this.right && !this.left);
        bool trueSplit = (this.right && this.left);

        Vector3 spawnPos = transform.position; // new position for the new node
        Vector3 newPosThis = transform.position; // new position for this node
        if (trueSplit)
        {
            spawnPos = Vector3.Lerp(transform.position, right.transform.position, .33f);
            newPosThis = Vector3.Lerp(transform.position, left.transform.position, .33f);
        }
        else if (left)      spawnPos += (transform.position - left.transform.position);
        else if (right)     spawnPos += (transform.position - right.transform.position);
        else                spawnPos += Vector3.right * 10;

        // position the new node (and this node, if necessary),
        // AND insert the node into our linked list of nodes:

        newNode.transform.position = spawnPos;
        newNode.transform.parent = transform.parent;

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
            this.transform.position = newPosThis;
        }
        return newNode; // return the new node
    }
    /// <summary>
    /// This method prepares the entire path for changes, by caching the current state in the undo history.
    /// </summary>
    /// <param name="undoName">A name of the impending changes. This shows up in the undo history.</param>
    private void RegisterLinkedListUndo(string undoName)
    {
        PathNode node = GetLeftMostNode();
        List<Object> nodes = new List<Object>();
        do
        {
            nodes.Add(node);
            node = node.right;
        } while (node);
        // this seems really redundant...
        // I want to call RegisterCompleteOnjectUndo() since it remembers object references after deletion,
        // but it doesn't seem to result in marking any changes as dirty.
        // so RecordObjects() is called to mark changed objects as dirty.
        Undo.RegisterCompleteObjectUndo(nodes.ToArray(), undoName);
        Undo.RecordObjects(nodes.ToArray(), undoName);
    }
    /// <summary>
    /// Remove this PathNode from the linked list and destroy the GameObject.
    /// </summary>
    public void RemoveFromLinkedList()
    {
        if (left) left.right = right;
        if (right) right.left = left;
    }
    /// <summary>
    /// Renames all of the PathNode objects in this linked list.
    /// </summary>
    /// <param name="name">The base name</param>
    public void RenameNodes(string name)
    {
        PathNode currentNode = GetLeftMostNode();
        for (int i = 0; currentNode; i++)
        {
            currentNode.name = name + i;
            currentNode = currentNode.right;
        }
    }
    /// <summary>
    /// Find the left-most node in the linked list.
    /// </summary>
    /// <returns>The left-most node in the linked list.</returns>
    public PathNode GetLeftMostNode()
    {
        PathNode leftMostNode = this;
        while (leftMostNode.left) leftMostNode = leftMostNode.left;
        return leftMostNode;
    }
    /// <summary>
    /// Replaces a vector's Y component.
    /// </summary>
    /// <param name="v">The vector to modify.</param>
    /// <param name="y">The new value to use for Y.</param>
    /// <returns>The modified vector.</returns>
    Vector3 OverwriteY(Vector3 v, float y = 0)
    {
        return new Vector3(v.x, y, v.z);
    }
    /// <summary>
    /// This method projects a target position onto a line segment.
    /// </summary>
    /// <param name="target">The position to project.</param>
    /// <param name="start">The start of the segment that forms the projection axis.</param>
    /// <param name="end">The end of the segment that forms the projection axis.</param>
    /// <returns>A percent value of the projection results. 0 would mean that the projected position aligns with start. 1 would mean that the projected position aligns with end.</returns>
    float ProjectOnSegment(Vector3 target, Vector3 start, Vector3 end)
    {
        Vector3 diff = OverwriteY(target - start);
        Vector3 axis = OverwriteY(end - start);

        float length = axis.magnitude;
        if (length == 0) return float.NaN; // segment doesn't have a length; end and start are the same
        float projection = Vector3.Dot(axis.normalized, diff);

        return projection / length; // return a percentage
    }
    /// <summary>
    /// This method "projects" a position onto curve. It really just gets the angle to the position and uses the angle to calculate its position.
    /// </summary>
    /// <param name="target">The position to project.</param>
    /// <returns>A percent value of the projection results. 0 would mean that the position aligns with the left edge of the curve. 1 would mean the position aligns with the right edge of the curve.</returns>
    float ProjectOnCurve(Vector3 target)
    {
        float angle = AngleWrapFromTo(CurveAngleTo(target), angleCurveIn);
        return (angle - angleCurveIn) / (angleCurveOut - angleCurveIn);
    }
    /// <summary>
    /// This method draws the node icon (and lines) in the editor.
    /// </summary>
    void OnDrawGizmos()
    {
        DrawLines();

        string icon = "icon-path-no-lr.png";

        if (left && right) icon = "icon-path.png";
        else if (left) icon = "icon-path-no-r.png";
        else if (right) icon = "icon-path-no-l.png";

        Gizmos.DrawIcon(transform.position, icon, true);
        
    }
    /// <summary>
    /// What gizmos get drawn in the editor when this object is selected.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
    /// <summary>
    /// This method draws the path line in the editor.
    /// </summary>
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

            if (showSharpTurns)
            {
                float p1 = 2 * Mathf.Abs(angleCurveIn - angleCurveOut) / Mathf.PI;
                float p2 = 1 - (clampedCurveRadius / 10);
                Gizmos.color = Color.Lerp(Color.green, Color.red, p1 * p2);
            } else
            {
                Gizmos.color = Color.white;
            }

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;

                Vector3 pt2 = GetPointOnCurve(t);
                Gizmos.DrawLine(pt1, pt2);
                pt1 = pt2;
            }
            Gizmos.DrawLine(pt1, curveOut);
            Gizmos.color = Color.white;
        }
    }
    /// <summary>
    /// Calculates a position on the curved section of the path.
    /// </summary>
    /// <param name="t">The percentage (0 is left edge of curve; 1 is right edge of curve).</param>
    /// <returns>The calculated position.</returns>
    Vector3 GetPointOnCurve(float t)
    {
        float currentAngle = Mathf.Lerp(angleCurveIn, angleCurveOut, t);
        float x = clampedCurveRadius * Mathf.Cos(currentAngle) + curveCenter.x;
        float y = Mathf.Lerp(
            Mathf.Lerp(curveIn.y, transform.position.y, t),
            Mathf.Lerp(transform.position.y, curveOut.y, t),
            t);
        float z = clampedCurveRadius * Mathf.Sin(currentAngle) + curveCenter.z;

        return new Vector3(x, y, z);
    }
    /// <summary>
    /// Calculates a desired rotation on a curved section of the path.
    /// </summary>
    /// <param name="t">The percentage (0 is left edge of curve; 1 is right edge of curve).</param>
    /// <returns>The calculated rotation.</returns>
    public Quaternion GetRotationOnCurve(float t)
    {
        if (!left) return rotationYaw;
        return Quaternion.Slerp(left.rotationYaw, rotationYaw, t);
    }
    /// <summary>
    /// This utility function converts one angle to be within 180 degrees of second angle.
    /// </summary>
    /// <param name="a">The angle to adjust.</param>
    /// <param name="b">The stationary reference angle.</param>
    /// <returns>The adjusted angle.</returns>
    float AngleWrapFromTo(float a, float b)
    {
        while (a - b > Mathf.PI) a -= Mathf.PI * 2;
        while (a - b < -Mathf.PI) a += Mathf.PI * 2;
        return a;
    }
    /// <summary>
    /// Using the percent value from projection, this method calculates an overall percentage of
    /// how far the projection is from this node to the next (right) node. A negative value is possible,
    /// indicating that the projection is on the curve to the left of this node.
    /// </summary>
    /// <param name="p">The projection percentage calculated by ProjectOnSegment() or ProjectOnCurve()</param>
    /// <param name="isOnCurve">Whether or not the input percentage value was calculated by ProjectOnCurve(). If false, it is assumed that the input percentage value was calculated by ProjectOnSegment().</param>
    /// <returns>The overall percentage of how far the projection is from this node (0) to the right node (1) or the left node (-1). The output is not clamped in any way.</returns>
    float CalcOverallPercent(float p, bool isOnCurve)
    {
        if (!right) return 1;

        if (isOnCurve)
        {
            if (p < .5f) return -Mathf.Lerp(0, percentCurveIn, (0.5f - p) / 0.5f); // left side of curve
            return Mathf.Lerp(0, percentCurveOut, (p - 0.5f) / 0.5f); // right side of curve
        }
        return Mathf.Lerp(percentCurveOut, 1 - right.percentCurveIn, p);
    }
    /// <summary>
    /// Using an overall percentage (see CalcOverallPercent()), this method interpolates the camera data of this node and a neighboring node.
    /// </summary>
    /// <param name="p">The overall percentage for the interpolation. If negative, the method will interpolate to the left node. Otherwise, it interpolates to the right node.</param>
    /// <returns>The interpolated camera data.</returns>
    public CameraDataNode.CameraData GetCameraData(float p)
    {
        if (!cameraDataNode) return null;

        CameraDataNode.CameraData data1 = cameraDataNode.cameraData;

        if (p == 0) return data1;
        if (p < 0 && (!left || !left.cameraDataNode)) return data1;
        if (p > 0 && (!right || !right.cameraDataNode)) return data1;

        CameraDataNode.CameraData data2 = (p < 0) ? left.cameraDataNode.cameraData : right.cameraDataNode.cameraData;

        if (p < 0) p *= -1; // make percent positive
        return CameraDataNode.CameraData.Lerp(data1, data2, p); // lerp
    }
    /// <summary>
    /// This is called when this object is about to be destroyed. It will cleanly remove this node from the linked list, and it registers undo history for the deletion / removal.
    /// </summary>
    void OnDestroy()
    {
        RegisterLinkedListUndo("Remove node from path");
        RemoveFromLinkedList();
    }
}