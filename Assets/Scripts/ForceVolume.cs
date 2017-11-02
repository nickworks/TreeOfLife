using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A trigger script for collision volumes that applies forces to objects that enter the volume. Requires some kind of 2D collider! For the volume to work, the collider component on the same object needs to be a trigger!
/// </summary>
public class ForceVolume : MonoBehaviour {

    #region variables

    /// <summary>
    /// Controls whether this volume will apply force to pawns or not.
    /// </summary>
    public bool turnedOn = true;
    /// <summary>
    /// How strong the force is in this particular volume 
    /// </summary>
    public float forceMult = 2f;
    /// <summary>
    /// Toggles whether or not to a ray in the scene view in the direction of this volume's force. 
    /// </summary>
    public bool drawAngleOfForce = false;
    /// <summary>
    /// The angle of the forces applied by this volume. 0 is directly upwards.
    /// </summary>
    public float angleOfForce = 0f;
    /// <summary>
    /// What directional vector the force of this volume goes.  Should be normalized.
    /// </summary>
    private Vector3 forceVector = Vector3.up;
    /// <summary>
    /// Should this volume overwrite the other object's gravity, or simply add to it's velocity?
    /// </summary>
    public bool OverwritesGravity = false;
    /// <summary>
    /// Controls whether the volume toggles between active and inactive on a timer
    /// </summary>
    public bool isTimed = false;
    /// <summary>
    /// The number (in seconds) between deactivating and reactivating (the inactive time)
    /// </summary>
    public float offTime = 1f;
    /// <summary>
    /// The number (in seconds) between activating and deactivating (the active time)
    /// </summary>
    public float activeTime = 1f;
    /// <summary>
    /// The timer controling toggling (used if isTimed = true)
    /// </summary>
    private float timer = 0f;
    /// <summary>
    /// When toggled off, should this object also disable the mesh on this object?
    /// </summary>
    public bool toggleMesh = true;

    #endregion
    #region EditorAdjustments
    /// <summary>
    /// Called every time a value in the inspector is changed.  Updates force direction information.
    /// </summary>
    public void OnValidate()
    {
        //Create a rotation based on the angle specified in the editor, and then apply it to a vector.
        forceVector = Quaternion.AngleAxis(-angleOfForce, Vector3.forward) * Vector3.up;
    }

    /// <summary>
    /// Called when rendering in scene view.  Draws a line in the direction of force.
    /// </summary>
    private void OnDrawGizmos()
    {
        if( drawAngleOfForce ) { Debug.DrawRay(transform.position, forceVector * 5, Color.red); }
    }

    #endregion

    /// <summary>
    /// Every tick, if the "is timed" option is checked, the game cycles the timer and adjusts the volume accordingly
    /// </summary>
    private void Update() {
        if( isTimed ) {

            timer -= Time.deltaTime;

            if(timer < 0f ) {
                turnedOn = !turnedOn;//toggle the active state
                timer = turnedOn ? activeTime : offTime;//apply the time to the timer
                //toggle visibility of the volume if the option to do so is enabled
                if( toggleMesh) GetComponent<MeshRenderer>().enabled = turnedOn;
            }
        }
    }

    #region Trigger Events

    /// <summary>
    /// What to do when an object enters this trigger volume.  Overwrites gravity if that option is checked.
    /// </summary>
    /// <param name="collision">The object that collided with this volume.</param>
    private void OnTriggerEnter2D( Collider2D collision )
    {
        if( turnedOn )
        {
            if( OverwritesGravity )
            {
                //only apply this to the player pawns
                if( collision.tag == "Player" )
                {
                    collision.GetComponent<Player.PlayerController>().SetGravity(forceVector, forceMult);
                }
            }
        }                
    }
    /// <summary>
    /// What to do when an object exits this trigger volume.  Resets gravity forces that may have been changed to defaults.
    /// </summary>
    /// <param name="collision">The object exiting the volume</param>
    private void OnTriggerExit2D( Collider2D collision )
    {
        if( turnedOn )
        {
            if( OverwritesGravity )
            {
                if( collision.tag == "Player" )
                {
                    collision.GetComponent<Player.PlayerController>().SetGravity();
                }
            }
        }        
    }
    /// <summary>
    /// What to do every tick an object is in this volume.  Applies forces if overwrites gravity is false.
    /// </summary>
    /// <param name="collision">The object that collided with this volume.</param>
    private void OnTriggerStay2D( Collider2D collision )
    {
        if( turnedOn )
        {
            if( !OverwritesGravity )
            {
                if( collision.tag == "Player" )
                {
                    collision.GetComponent<Player.PlayerController>().ApplyForce(forceMult, forceVector);
                }
            }
        }        
    }

    #endregion

}

/// <summary>
/// A custom editor that allows for timer options to be hidden if the volume is not timed.
/// </summary>
[CustomEditor(typeof(ForceVolume))]
public class ForceVolumeEditor : Editor
{
    override public void OnInspectorGUI()
    {

        var myScript = target as ForceVolume;

        new SerializedObject(myScript);

        myScript.turnedOn = EditorGUILayout.Toggle(new GUIContent("Turned On", "Toggles whether or not this volume currently active."), myScript.turnedOn);
        EditorGUILayout.PrefixLabel(new GUIContent("Force Multiplier:", "How powerful the force volume is. Gravitational forces require much less power."));
        myScript.forceMult = EditorGUILayout.Slider(myScript.forceMult, 0, 100);
        myScript.drawAngleOfForce = EditorGUILayout.Toggle(new GUIContent("Draw Angle", "Toggles whether or not to draw a vector in the scene view that represents the angle of force."), myScript.drawAngleOfForce);
        EditorGUILayout.PrefixLabel(new GUIContent("Angle of Force: ", "What direction should the force vector point?  0 is straight UP. Toggle 'Draw Angle' To see the angle represented in the Scene View."));
        myScript.angleOfForce = EditorGUILayout.Slider(myScript.angleOfForce, -180, 180);
        myScript.OverwritesGravity = EditorGUILayout.Toggle(new GUIContent("Overwrites Gravity", "When true, forces overwrite gravity. When false, they are applied separately, and normal gravity is still applied to the pawn."), myScript.OverwritesGravity);

        myScript.isTimed = EditorGUILayout.Toggle(new GUIContent("Use Timer", "Allows the volume to toggle itself on for a set period of time after a set delay."), myScript.isTimed);

        using( var group = new EditorGUILayout.FadeGroupScope(System.Convert.ToSingle(myScript.isTimed)) )
        {
            if( group.visible == true )
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PrefixLabel(new GUIContent("Time Off:", "How long the volume is inactive before it is toggled back on."));
                myScript.offTime = EditorGUILayout.Slider(myScript.offTime, 0, 100);
                EditorGUILayout.PrefixLabel(new GUIContent("Time On:", "How long the volume is active before it is toggled back off."));
                myScript.activeTime = EditorGUILayout.Slider(myScript.activeTime, 0, 100);
                myScript.toggleMesh = EditorGUILayout.Toggle(new GUIContent("Toggle Visibility", "If true, the mesh on this object will be turned off while the volume is in the 'inactive' part of it's timer cycle."), myScript.toggleMesh);
                EditorGUI.indentLevel--;
            }
        }

        //OnValidate doesn't automatically trigger w/ custom inspectors :(
        if( GUI.changed ) { myScript.OnValidate(); }
    }
}