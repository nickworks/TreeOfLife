using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Path : MonoBehaviour {

    public struct Segment
    {
        Vector3 p1;
        Vector3 p2;
        Vector3 diff;
        Vector3 dir;
        Quaternion orientation;
        public Segment(Vector3 p1, Vector3 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            diff = p2 - p1;
            diff.y = 0;
            dir = diff.normalized;
            float angle = Mathf.Atan2(-diff.z, diff.x) * Mathf.Rad2Deg;
            orientation = Quaternion.Euler(0, angle, 0);
        }
        public SegmentResult ClosestPointTo(Vector3 p)
        {
            Vector3 vectorToPt = p - p1;
            vectorToPt.y = 0;

            float projectedDistance = Vector3.Dot(dir, (p - p1));

            if (projectedDistance <= 0) return new SegmentResult(SegmentResultType.LeftOfSegment, p1, orientation);
            if (projectedDistance * projectedDistance >= (p2 - p1).sqrMagnitude) return new SegmentResult(SegmentResultType.RightOfSegment, p2, orientation);

            return new SegmentResult(SegmentResultType.OnSegment, p1 + dir * projectedDistance, orientation);
        }
    }

    public enum SegmentResultType {
        OnSegment,
        LeftOfSegment,
        RightOfSegment,
        Fail
    }
    public struct SegmentResult
    {
        public SegmentResultType type;
        public Vector3 position;
        public Quaternion rotation;
        public SegmentResult(SegmentResultType type, Vector3 position, Quaternion rotation)
        {
            this.type = type;
            this.position = position;
            this.rotation = rotation;
        }
    }

    public List<Vector3> points = new List<Vector3>();
    private Segment[] segments;

    void OnValidate()
    {
        segments = new Segment[points.Count - 1];
        for(int i = 0; i < points.Count - 1; i++)
        {
            segments[i] = new Segment(GetWorldPoint(i), GetWorldPoint(i + 1));
        }
    }
    public SegmentResult GetSolution(Vector3 pt, int index = -1)
    {
        if (index < 0) index = FindSegmentIndexClosestTo(pt);

        if (index < 0 || index >= segments.Length) return new SegmentResult(SegmentResultType.Fail, pt, Quaternion.identity);
        SegmentResult results = segments[index].ClosestPointTo(pt);
        // FIXME: this ping pongs back and forth around a >90-degree corner...
        return results;
        switch (results.type)
        {
            case SegmentResultType.LeftOfSegment:
                return GetSolution(pt, index - 1);
            case SegmentResultType.RightOfSegment:
                return GetSolution(pt, index + 1);
            default:
                return results;
        }
    }
    public Segment FindSegmentClosestTo(Vector3 pt)
    {
        int index = FindSegmentIndexClosestTo(pt);
        if (index < 0 || index >= segments.Length) return new Segment(); // this shouldn't happen...
        return segments[index];
    }
    int FindSegmentIndexClosestTo(Vector3 pt)
    {
        float bestDis = float.MaxValue;
        int bestIndex = -1;
        for(int i = 0; i < segments.Length; i++)
        {
            SegmentResult result = segments[i].ClosestPointTo(pt);
            float dis = (pt - result.position).sqrMagnitude;
            if(dis < bestDis)
            {
                bestDis = dis;
                bestIndex = i;
            }
        }
        return bestIndex;
    }
    Vector3 GetWorldPoint(int i)
    {
        if (i < 0) i = 0;
        if (i >= points.Count) i = points.Count - 1;
        return transform.TransformPoint(points[i]);
    }
    Vector3 ClosestPointOfSegment(Vector3 pt, int i)
    {
        Vector3 p1 = GetWorldPoint(i) ;
        Vector3 p2 = GetWorldPoint(i + 1);

        Vector3 vectorToPt = pt - p1;
        Vector3 segment = p2 - p1;

        vectorToPt.y = 0;
        segment.y = 0;

        Vector3 axis = segment.normalized;
        float projectedPoint = Vector3.Dot(axis, vectorToPt);

        // TODO: SWITCH TO NEXT SEGMENT!!
        if (projectedPoint <= 0) {
            if (i == 0) return p1;
            return ClosestPointOfSegment(pt, i - 1);
        }
        if (projectedPoint * projectedPoint >= (p2 - p1).sqrMagnitude)
        {
            if(i == points.Count - 1) return p2;
            return ClosestPointOfSegment(pt, i + 1);
        }
        
        return p1 + axis * projectedPoint;
    }
    void OnDrawGizmos()
    {
        DrawPathInEditor();
        Gizmos.DrawIcon(transform.position, "icon-path.png", true);
    }
    void DrawPathInEditor()
    {
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? transform.rotation : Quaternion.identity;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = transform.TransformPoint(points[i]);
            if (i > 0)
            {
                Vector3 pp = transform.TransformPoint(points[i - 1]);
                Handles.DrawLine(pp, p);
            }
        }
    }
}
[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    void OnSceneGUI()
    {
        Path path = (Path)target;
        Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? path.transform.rotation : Quaternion.identity;
        for (int i = 0; i < path.points.Count; i++)
        {
            Vector3 p = path.transform.TransformPoint(path.points[i]);          

            EditorGUI.BeginChangeCheck();
            p = Handles.DoPositionHandle(p, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(path, "Moved a Path Node");
                EditorUtility.SetDirty(path);
                path.points[i] = path.transform.InverseTransformPoint(p);
            }
        }
    }
    override public void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        if(GUILayout.Button("Add point"))
        {

        }
    }
}