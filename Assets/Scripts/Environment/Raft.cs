using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is the main script that controls all raft objects in the game.
/// </summary>
public class Raft : MonoBehaviour
{
    /// <summary>
    /// Controls whether the raft is affected by gravity, this is turned true when colliding with water.
    /// </summary>
    private bool isInWater = false;
    /// <summary>
    /// The direction of gravity applied when not colliding with water.
    /// </summary>
    private Vector3 gravity = Vector3.down;
    private Vector3 velocity;
    /// <summary>
    /// Holds a reference to the player object.
    /// </summary>
    private Player.PlayerController pawn;
    /// <summary>
    /// Holds a reference to the player object's transform
    /// </summary>
    private Transform pawn2;
    /// <summary>
    /// Which layers we want the raft to collide with.
    /// </summary>
    public LayerMask waterMask;

    /// <summary>
    /// gets the boxCollider bounds of the object and stores it in a variable
    /// </summary>
    private BoxCollider boxCollider;
    /// <summary>
    /// stores the X max point for the raft
    /// </summary>
    private Transform xLocalMax;
    /// <summary>
    /// stores the X min point for the raft
    /// </summary>
    private Transform xLocalMin;
    /// <summary>
    /// stores the centerpoint to the raft
    /// </summary>
    private Vector3 centerPoint;
    /// <summary>
    /// stores the distance of half the height of the raft
    /// </summary>
    private float halfH;
    /// <summary>
    /// stores the distance of half the width of the raft
    /// </summary>
    private float halfW;
    /// <summary>
    /// stores the distance of half the depth of the raft
    /// </summary>
    private float halfD;

    /// <summary>
    /// gets the bounds of the object and then calculates the half height for it and stores both in a variable
    /// </summary>
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        halfH = boxCollider.bounds.extents.y;
        halfW = boxCollider.bounds.extents.x;
        halfD = boxCollider.bounds.extents.z;
        xLocalMax = this.transform.GetChild(1);
        xLocalMin = this.transform.GetChild(2);
    }

    /// <summary>
    /// updates gravity every frame if not colliding with water. Updates horizontal velocity every frame if the player "isAttached" to the raft
    /// </summary>
    void LateUpdate()
    {
        CalculateBounds();
        if (isInWater == false)
        {
            
            if (Physics.Raycast(centerPoint, Vector3.down, halfH, waterMask))
            {
                isInWater = true;
            }
            else
            {
                transform.position += gravity * Time.deltaTime;
            }

        }

        // is true when the player is attached to the raft
        if (pawn != null)
        {
            // applies velocity to the raft based upon the player
            transform.position += DetermineHorizontalMovement();
        }

    }

    /// <summary>
    /// Calculates the positions for Raycasts to be sent from the raft.
    /// </summary>
    private void CalculateBounds()
    {
        boxCollider = this.GetComponent<BoxCollider>();
        centerPoint = boxCollider.bounds.center;
        
    }
    /// <summary>
    /// Determines the rafts horizontal movement if it is touching water. The raft casts rays from its left minimum y position and right minimum y position to determine if it can move in that direction initially.
    /// </summary>
    /// <returns>How much velocity to add to the raft.</returns>
    private Vector3 DetermineHorizontalMovement()
    {

        Vector3 raftWorldVelocity = new Vector3(pawn.worldSpace.x, 0, pawn.worldSpace.z);
        velocity = raftWorldVelocity;
        NewMethod();

        return velocity;
    }

    private void NewMethod()
    {
        /*
        casts a ray downwards
        if the ray does not collide with water and the player is moving left
        stop the rafts horizontal movement
        */
        
        
        if (!Physics.Raycast(xLocalMax.position, Vector3.down, halfH, waterMask))
        {
            // TODO
                velocity.x = 0;
                velocity.z = 0;

        }

        /*
        casts a ray downwards
        if the ray does not collide with water and the player is moving right
        stop the rafts horizontal movement
        */
        
        if (!Physics.Raycast(xLocalMin.position, Vector3.down, halfH, waterMask))
        {
            // TODO
                velocity.x = 0;
                velocity.z = 0;
        }
    }

    /// <summary>
    /// This method is called to attach the raft to a player.
    /// </summary>
    /// <param name="player">The PlayerController to attach to.</param>
    public void Attach(Player.PlayerController player)
    {
        pawn = player;
        transform.rotation = player.GetComponent<Transform>().rotation;
    }
    /// <summary>
    /// This method detaches the raft from a PlayerController.
    /// </summary>
    public void Detach()
    {
        pawn = null;
        pawn2 = null;
    }
}
