using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// This class creates custom Editor GUI (scene and inspector) and behavior for PathNode objects.
/// </summary>
[CustomEditor(typeof(PathNode))]
[CanEditMultipleObjects]
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
            PathNode temp = node.right ? node.right : null;
            Undo.DestroyObjectImmediate(node.gameObject);
            Rename(temp);   
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("Debugging:");
        PathNode.showSharpTurns = EditorGUILayout.Toggle("Show Sharp Turns", PathNode.showSharpTurns);
        if (GUI.changed)
        {
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews(); // force a redraw...
        }
    }
    /// <summary>
    /// This method renames all of the PathNode objects in a path.
    /// </summary>
    /// <param name="node">A node from the desired path.</param>
    void Rename(PathNode node)
    {
        if (!node) return;
        node.GetLeftMostNode().RenameNodes("Node");
    }
}