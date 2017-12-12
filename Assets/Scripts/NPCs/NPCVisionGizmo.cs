using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// option list for drawing gizmos
/// </summary>
public enum NPCVisionGizmoDrawOptions
{
    None,
    All,
    OnlyThisOne
}

/// <summary>
/// Apply this to an NPC (on the same object as a vision/detection collider) to draw their activation/vision radius in the scene view.  Requires a 3d collider, doesn't work with capsules (no capsule drawing gizmo atm)
/// </summary>
public class NPCVisionGizmo : MonoBehaviour {

    /// <summary>
    /// Holds the selected draw option for Gizmo Drawing.
    /// </summary>
    public static NPCVisionGizmoDrawOptions drawGizmos;

    /// <summary>
    /// Global gizmo drawing call.  Will draw this even when not selected.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (drawGizmos == NPCVisionGizmoDrawOptions.All )
        {
            Gizmos.color = Color.red;//red for bad guys
            //figure out which kind of collider is on this object and draw a representation of that collider
            if( GetComponent<BoxCollider>() )
            {
                Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().bounds.size);
            }else if( GetComponent<SphereCollider>() )
            {
                Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
            }
        }
    }

    /// <summary>
    /// Gizmo Drawing Call. Will only be drawn when selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if( drawGizmos == NPCVisionGizmoDrawOptions.OnlyThisOne )
        {
            Gizmos.color = Color.red;//red for bad guys
            //figure out which kind of collider is on this object and draw a representation of that collider
            if( GetComponent<BoxCollider>() )
            {
                Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().bounds.size);
            } else if( GetComponent<SphereCollider>() )
            {
                Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
            }
        }
    }
}

/// <summary>
/// Custom editor for the NPC Vision Gizmo script that allows user to select gizmo draw options from a drop down.
/// </summary>
[CustomEditor(typeof(NPCVisionGizmo))]
public class NPCVisionGizmoEditor : Editor
{
    /// <summary>
    /// Called when the inspector GUI is updated
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();//call the default GUI (shouldn't be anything but does a good job of setting up the window content)
        
        //we need to create a custom inspector like this because a static variable can't be serialized.  This way we manually manipulate it in the inspector and change the static variable directly.
        NPCVisionGizmo.drawGizmos = (NPCVisionGizmoDrawOptions)EditorGUILayout.EnumPopup(new GUIContent("Gizmo Draw Options","Should gizmos be drawn on all objects with this script, only this object, or not at all?  Controls everything with this script, does not discriminate between enemy types!"), NPCVisionGizmo.drawGizmos);
    }

}