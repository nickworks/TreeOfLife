using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is the main script that controls all raft objects in the game.
/// </summary>
public class Raft : MonoBehaviour
{
    /// <summary>
    /// controls whether the raft is affected by gravity, this is turned true when colliding with water.
    /// </summary>
    private bool isInWater = false;
    /// <summary>
    /// applies downward gravity when not colliding with water
    /// </summary>
    private Vector3 gravity = Vector3.down;
    /// <summary>
    /// holds a reference to the player object
    /// </summary>
    private Player.PlayerController pawn;
    /// <summary>
    /// Which layers we want the raft to collide with.
    /// </summary>
    public LayerMask waterMask;

    /// <summary>
    /// updates gravity every frame if not colliding with water
    /// updates horizontal velocity every frame if the player "isAttached" to the raft
    /// </summary>
    void LateUpdate()
    {
        // is true when colliding with water, else false
        if (isInWater == false)
        {
            // applies gravity to the raft position
            transform.position += gravity * Time.deltaTime;
        }

        // is true when the player is attached to the raft
        if (pawn != null)
        {
            // applies velocity to the raft
            transform.position += DetermineHorizontalMovement() * Time.deltaTime;
        }

    }

    /// <summary>
    /// determines the rafts horizontal moevement if it is touching water
    /// the raft casts rays from its left minimum y position and right minimum y position
    /// The raycasts make it so that a raft cannot move off of water once colliding
    /// they can however move back onto water if they hit the edge and one raycast is no longer colliding
    /// </summary>
    /// <returns>How much velocity to add to the raft.</returns>
    private Vector3 DetermineHorizontalMovement()
    {
        // obtains the players X velocity and stores it in the playerX variable
        float playerX = pawn.velocity.x;
        // stores player's X in a vector3 and stores it in a vector3 that will be used to move the raft
        Vector3 velocity = new Vector3(playerX, 0, 0);
        // gets the boxCollider2D bounds of the object and stores it in a variable
        Bounds bounds = GetComponent<BoxCollider2D>().bounds;
        // stores the left minimum position of the object in a Vector2
        Vector2 leftMin = new Vector2(bounds.min.x, bounds.min.y);
        // stores the right minimum position of the object in a Vector2
        Vector2 rightMin = new Vector2(bounds.max.x, bounds.min.y);

        /// <summary>
        /// casts a ray downwards
        /// if the ray does not collide with water and the player is moving left
        /// stop the rafts horizontal movement
        /// </summary>
        if (!Physics2D.Raycast(leftMin, Vector2.down, 1, waterMask))
        {
            if (playerX < 0)
            {
                velocity.x = 0;
            }
        }

        /// <summary>
        /// casts a ray downwards
        /// if the ray does not collide with water and the player is moving right
        /// stop the rafts horizontal movement
        /// </summary>
        if (!Physics2D.Raycast(rightMin, Vector2.down, 1, waterMask))
        {
            if (playerX > 0)
            {
                velocity.x = 0;
            }
        }

        return velocity;
    }

    /// <summary>
    /// detects collision with objects that have the "Water" tag
    /// when collision occurs, downward gravity is stopped on the raft
    /// </summary>
    /// <param name="other"></param> the object the raft IS colliding with
    void OnTriggerEnter2D(Collider2D other)
    {
        // sets isInWater to true if the raft is colliding with an object with "Water" Layer
        int mask = (1 << other.gameObject.layer);

        if ((mask & waterMask.value) > 0)
        {
            isInWater = true;
        }
        
    }
    /// <summary>
    /// This method is called to attach the raft to a player.
    /// </summary>
    /// <param name="player">The PlayerController to attach to.</param>
    public void Attach(Player.PlayerController player)
    {
        pawn = player;
    }
    /// <summary>
    /// This method detaches the raft from a PlayerController.
    /// </summary>
    public void Detach()
    {
        pawn = null;
    }
}
