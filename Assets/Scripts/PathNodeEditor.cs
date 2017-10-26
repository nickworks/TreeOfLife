using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor
{

    [DrawGizmo((GizmoType)255)] // all gizmo types (0011 1111)
    static void DrawGizmo(PathNode node, GizmoType gizmoType)
    //void OnSceneGUI()
    {
        //PathNode node = (PathNode)target;
        //Handles.DrawWireCube(node.transform.position, Vector3.one * .1f);
        //Gizmos.DrawSphere(node.transform.position, .1f);
        if (node.left) Handles.DrawLine(node.transform.position, node.left.transform.position);
        if (node.right) Handles.DrawLine(node.transform.position, node.right.transform.position);
        if (Handles.Button(node.transform.position, Quaternion.identity, .1f, .2f, Handles.SphereHandleCap))
        {
            Selection.activeGameObject = node.gameObject;
            Debug.Log("???");
        }
    }
}

