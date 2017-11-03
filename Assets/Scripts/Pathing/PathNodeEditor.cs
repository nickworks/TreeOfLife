using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This class creates custom Editor GUI (scene and inspector) and behavior for PathNode objects.
/// </summary>
[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor
{
    /// <summary>
    /// This method draws the Inspector GUI for PathNode objects.
    /// </summary>
    override public void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        PathNode node = ((PathNode)target);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+ NODE"))
        {
            node.Split();
            Rename(node);
        }
        if (GUILayout.Button("- NODE"))
        {
            PathNode temp = null;
            if (node.right) temp = node.right;
            node.RemoveAndDestroy();
            Rename(temp);
        }
        GUILayout.EndHorizontal();
        
    }
    /// <summary>
    /// This method renames all of the PathNode objects in a path.
    /// </summary>
    /// <param name="node">A node from the desired path.</param>
    void Rename(PathNode node)
    {
        node.GetLeftMostNode().RenameNodes("Node");
    }
    /// <summary>
    /// This method draws additional gizmos / handles in the scene view when a PathNode is selected.
    /// </summary>
    void OnSceneGUI()
    {
        PathNode node = ((PathNode)target);
        Handles.DrawWireCube(node.transform.position, Vector3.one);
        Handles.DrawWireCube(node.curveCenter, Vector3.one * .1f);
        //Handles.DrawWireArc(node.curveCenter, Vector3.up, Vector3.right, 360, node.curveRadius);
        Handles.Label(node.transform.position + new Vector3(1, 0, 1), node.data);

    }
}

