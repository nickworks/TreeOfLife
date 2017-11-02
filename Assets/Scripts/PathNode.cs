using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour {

    public PathNode left;
    public PathNode right;
    [Range(0, 10)] public float curveRadius = 0;
    private float clampedCurveRadius = 0;

    public Vector3 curveIn { get; private set; }
    public Vector3 curveOut { get; private set; }
    private float percentCurveIn;
    private float percentCurveOut;

    private Quaternion rotationYaw;
    private float length;

    void Start()
    {
        CacheData();
    }
    void OnValidate()
    {
        CacheData();
    }
    void CacheData(PathNode caller = null)
    {
        length = 0;
        curveIn = transform.position;
        curveOut = transform.position;
        clampedCurveRadius = 0;

        if (left && caller != left) left.CacheData(this); // ripple outwards
        if (right && caller != right) right.CacheData(this); // ripple outwards

        if (right)
        {
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

        if (!right) return results; // hmmmm...

        float p = ProjectOnSegment(results.position, curveOut, right.curveIn); // project onto straight segment

        if(p < 0) // left of straight segment
        {
            if (clampedCurveRadius <= 0 || curveIn == curveOut)
            {
                if (left) results.newNode = left;
            }
            else
            {
                p = ProjectOnSegment(results.position, curveIn, curveOut); // project onto curve
                if (p < 0 || p > 1) // left of curve
                {
                    if(left) results.newNode = left;
                }
                else // on curve
                {
                    results.position = OverwriteY(GetPointOnCurve(p), results.position.y);
                    results.rotation = GetRotationOnCurve(p);
                }
            }

        } else if (p <= 1) { // on straight segment

            results.position = OverwriteY(Vector3.Lerp(curveOut, right.curveIn, p), results.position.y);
            results.rotation = rotationYaw;

        } else { // right of straight segment

            results.newNode = right;

        }

        return results;
    }
    public ProjectionResults ClosestPoint(Transform obj)
    {
        ProjectionResults results = new ProjectionResults();
        results.position = obj.position;

        if (!right) return results;
        Vector3 nodeToThis = obj.position - transform.position;
        nodeToThis.y = 0;

        Vector3 segment = VectorToRight();
        segment.y = 0;

        Vector3 axis = segment.normalized;
        float projectedPoint = Vector3.Dot(axis, nodeToThis);

        if (left && right && projectedPoint < clampedCurveRadius) // projection inside curved section:
        {   
            float t = (projectedPoint + clampedCurveRadius) / (clampedCurveRadius * 2);
            //print(t);
            //results.position = GetPointOnCurve(t);
            results.rotation = GetRotationOnCurve(t);
        }
        else if (projectedPoint <= 0) // projection to the left of this node:
        {
            results.newNode = left;
        }
        else if (projectedPoint >= length) // projection past right node:
        {
            results.newNode = right;
        }
        else // projection on segment:
        {
            results.position = axis * projectedPoint + transform.position;
            results.position.y = obj.position.y; // break y out of the line segment to preserve the object's height
            results.rotation = rotationYaw;
        }
            
        return results;
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
        if (transform.hasChanged) CacheData();

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
