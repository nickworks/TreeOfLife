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
    /// stores the left minimum value for the raft
    /// </summary>
    private Vector3 leftMin;
    /// <summary>
    /// stores the right minimum value for the raft
    /// </summary>
    private Vector3 rightMin;
    /// <summary>
    /// stores the centerpoint to the raft
    /// </summary>
    private Vector3 centerMin;
    /// <summary>
    /// stores the distance of half the height of the raft
    /// </summary>
    private float halfH;

    /// <summary>
    /// gets the bounds of the object and then calculates the half height for it and stores both in a variable
    /// </summary>
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        halfH = Mathf.Abs(boxCollider.bounds.center.y - boxCollider.bounds.min.y);
    }

    /// <summary>
    /// updates gravity every frame if not colliding with water. Updates horizontal velocity every frame if the player "isAttached" to the raft
    /// </summary>
    void LateUpdate()
    {
        CalculateBounds();
        // is true when colliding with water, else false
        if (isInWater == false)
        {
            if (Physics.Raycast(centerMin, Vector3.down, halfH, waterMask))
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
        leftMin = new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);
        rightMin = new Vector3(boxCollider.bounds.max.x, boxCollider.bounds.center.y, boxCollider.bounds.center.z);
        centerMin = boxCollider.bounds.center;
    }
    /// <summary>
    /// Determines the rafts horizontal movement if it is touching water. The raft casts rays from its left minimum y position and right minimum y position to determine if it can move in that direction initially.
    /// </summary>
    /// <returns>How much velocity to add to the raft.</returns>
    private Vector3 DetermineHorizontalMovement()
    {

        Vector3 raftWorldVelocity = new Vector3(pawn.worldSpace.x, 0, pawn.worldSpace.z);
        float playerX = pawn.velocity.x;

        Vector3 velocity = raftWorldVelocity;

        /*
        casts a ray downwards
        if the ray does not collide with water and the player is moving left
        stop the rafts horizontal movement
        */
        if (!Physics.Raycast(leftMin, Vector3.down, 1, waterMask))
        {
            Debug.DrawRay(leftMin, Vector3.down, Color.green, 5);
            if (pawn.velocity.x > 0)
            {
                velocity.x = 0;
                velocity.z = 0;
            }
        }

        /*
        casts a ray downwards
        if the ray does not collide with water and the player is moving right
        stop the rafts horizontal movement
        */
        if( !Physics.Raycast(rightMin, Vector3.down, 1, waterMask))
        {
            Debug.DrawRay(rightMin, Vector3.down, Color.green, 5);
            if (pawn.velocity.x < 0)
            {
                velocity.x = 0;
                velocity.z = 0;
            }
        }

        return velocity;
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
