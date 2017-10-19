using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpawn : MonoBehaviour
{
    /// <summary>
    /// The prefab of a segment of rope.
    /// </summary>
    public GameObject ropePiece;
    /// <summary>
    /// Reference to the player Rigidbody2D when it is connected.
    /// </summary>
    private Rigidbody2D playerBody;
    private PlayerController playerScript;
    /// <summary>
    /// A reference list to all of the rope segments.
    /// </summary>
    private List<GameObject> linksList = new List<GameObject>();
    /// <summary>
    /// The set number of segments to create for the rope.
    /// </summary>
    public int numberOfSegments = 12;
    /// <summary>
    /// The set max length of the full rope.
    /// </summary>
    public float totalLength = 4;
    /// <summary>
    /// The strength of the swing.
    /// </summary>
    public float swingStrength = 5;

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player" && Input.GetButtonDown("Fire1"))
        {
            playerBody = collider.gameObject.GetComponent<Rigidbody2D>();
            playerScript = collider.gameObject.GetComponent<PlayerController>();
            if (playerBody.bodyType == RigidbodyType2D.Kinematic)
                LinkRope();
        }
    }

    private void Update()
    {
        if (playerScript != null)
        {
            Transform pt = playerBody.transform;
            float axisH = Input.GetAxisRaw("Horizontal");
            Vector2 dir = pt.position - transform.position;
            dir = new Vector2(-dir.y, dir.x);
            dir.Normalize();
            playerBody.AddForce(dir * axisH * swingStrength);
            if (Input.GetButtonUp("Fire1"))
                UnlinkRope();
        }
    }

    /// <summary>
    /// This creates the chain/rope and connects it from the player to the hook.
    /// </summary>
	void LinkRope ()
    {
        float desiredLinkLength = totalLength / numberOfSegments;
        Rigidbody2D lastLink = null;
        // The position of the hook.
        Vector2 pos = transform.position;
        // The position of the player.
        Vector2 playerPos = playerScript.transform.position;
        float linkDir = Mathf.Atan2(pos.y - playerPos.y, pos.x - playerPos.x);
        float realDistance = Vector2.Distance(pos, playerPos);
        float realLinkLength = realDistance / numberOfSegments;
        playerScript.TransferPhysics(RigidbodyType2D.Dynamic);
        for (int i = 0; i < numberOfSegments; i++)
        {
            GameObject piece = Instantiate(ropePiece);
            linksList.Add(piece);
            Vector2 myPos = playerPos;
            myPos.x += Mathf.Cos(linkDir) * realLinkLength * i;
            myPos.y += Mathf.Sin(linkDir) * realLinkLength * i;
            piece.transform.position = myPos;
            DistanceJoint2D joint = piece.GetComponent<DistanceJoint2D>();
            joint.distance = desiredLinkLength;
            if (lastLink == null)
            {
                joint.connectedBody = playerBody;
            }
            else
            {
                joint.connectedBody = lastLink;
            }
            if (i >= numberOfSegments - 1)
                piece.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            lastLink = piece.GetComponent<Rigidbody2D>();
        }
	}
	
    /// <summary>
    /// Should be called when the player lets go of the rope.
    /// </summary>
	public void UnlinkRope ()
    {
        for (int i = linksList.Count-1; i >= 0; i--)
        {
            GameObject obj = linksList[i];
            linksList.RemoveAt(i);
            Destroy(obj);
        }
        playerScript.TransferPhysics(RigidbodyType2D.Kinematic);
        playerScript = null;
        playerBody = null;
	}
}
