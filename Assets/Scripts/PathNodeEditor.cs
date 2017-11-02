using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor
{
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
        /*
        if(GUILayout.Button("Promote to Parent"))
        {
            PathNode topNode = node;

            while (true)
            {

                PathNode parentNode = topNode.transform.parent.GetComponent<PathNode>();
                if (!parentNode) break;
                topNode = parentNode;
            }

            node.transform.parent = topNode.transform.parent;

            for (PathNode n = node.left; n != null; n = n.left)
            {
                n.transform.parent = node.transform;
            }
            for (PathNode n = node.right; n != null; n = n.right)
            {
                n.transform.parent = node.transform;
            }

        }
        */
        
    }
    void Rename(PathNode node)
    {
        node.GetLeftMostNode().RenameNodes("Node");
    }
}

