using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This behavior is attached to a volume. When an path-aligned object (e.g. the player) collides with the volume, it will switch tracks.
/// </summary>
public class VolumePathSwitcher : MonoBehaviour {

    /// <summary>
    /// The PathNode to switch to when another objects collides with this object.
    /// </summary>
    public PathNode node;

    /// <summary>
    /// This is fired when an object enters the volume.
    /// </summary>
    /// <param name="collider">The collider that entered the volume.</param>
	void OnTriggerEnter(Collider collider)
    {
        AlignWithPath controller = collider.GetComponent<AlignWithPath>();
        if (controller && node) // if the other object has an AlignWithPath node, switch its current node.
        {
            controller.currentNode = node;
        }
    }
}
