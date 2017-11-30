using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is the main script that controls all raft objects in the game.
/// </summary>
[RequireComponent(typeof(AlignWithPath))]
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
    /// the velocity that is added to the raft every frame to determine movement.
    /// </summary>
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
    /// stores the position of the raft every frame while its in water, if it goes off of water, it holds the last position of the raft while it was in the water.
    /// </summary>
    private Vector3 lastPointInWater;
    /// <summary>
    /// stores the position of the raft when it moves off of the water.
    /// </summary>
    private Vector3 offWaterPoint;
    /// <summary>
    /// is true when the raft is on the water and false when it moves off of the water.
    /// </summary>
    private bool isCompletelyOnWater;

    /// <summary>
    /// gets the bounds of the object and then calculates the half height for it and stores both in a variable
    /// </summary>
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        halfH = boxCollider.bounds.extents.y;
        xLocalMax = this.transform.GetChild(1);
        xLocalMin = this.transform.GetChild(2);
    }

    /// <summary>
    /// Updates gravity every frame if not colliding with water.
    /// Updates horizontal velocity every frame if the player "isAttached" to the raft.
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
    /// Determines the rafts horizontal movement if it is touching water.
    /// The raft casts rays from its minimum x position and maximum x position to determine if it can move in that direction initially.
    /// </summary>
    /// <returns>How much velocity to add to the raft.</returns>
    private Vector3 DetermineHorizontalMovement()
    {
        // gets and stores the velocity of the player for that frame
        Vector3 raftWorldVelocity = new Vector3(pawn.worldSpace.x, 0, pawn.worldSpace.z);
        velocity = raftWorldVelocity;

        // determines to either check edges or check movement based on last frame's raycasts.
        if(isCompletelyOnWater)
        {
            CheckEdges();
        } else
        {
            CheckMovement();
        }
        return velocity;
    }

    private void CheckMovement()
    {
        // stores the Vector3 distance between the last point it was on water and the point of where it is off the water at.
        Vector3 offDistance = offWaterPoint - lastPointInWater;
        // grabs the last position on water to create an imaginary point in space with the following IF statement.
        Vector3 imaginaryWaterPoint = lastPointInWater;
        // moves the imaginary point further down the line to sandwich the lastPositionInWater point 
        if (Mathf.Sign(offDistance.x) >= 0)
        {
            imaginaryWaterPoint.x += 1;
        }
        else
        {
            imaginaryWaterPoint.x -= 1;
        }
        if (Mathf.Sign(offDistance.z) >= 0)
        {
            imaginaryWaterPoint.z += 1;
        }
        else
        {
            imaginaryWaterPoint.z -= 1;
        }

        // stores where the raft wants to move this frame to stay aligned to player
        Vector3 nextFramePosition = centerPoint + velocity;
        // calculates and stores the distance that the raft needs to move to get back on water.
        float returnDistance = CalcDistance(offDistance);
        // stores the distance between the off water point and the imaginary point.
        float imaginaryDistance = CalcDistance(offWaterPoint - imaginaryWaterPoint);
        // stores the distance between the off water point and the future position
        float currentDistance = CalcDistance(offWaterPoint - nextFramePosition);
        // stores the distance between the imaginary point and the future position
        float currentDistanceFromImaginary = CalcDistance(imaginaryWaterPoint - nextFramePosition);
        // checks to see if the raft is moving towards the water
        if (currentDistanceFromImaginary > imaginaryDistance)
        {
            // checks to see if the raft will be completely back on water next frame.
            if (returnDistance / imaginaryDistance <= currentDistance / imaginaryDistance)
            {
                isCompletelyOnWater = true;
            }
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }
    }

    /// <summary>
    /// calculates the distance between two points in space and returns that distance as a float.
    /// </summary>
    /// <param name="distance"> the Vector3 distance between two points </param>
    /// <returns></returns>
    private float CalcDistance(Vector3 distance)
    {
        float xDistance = distance.x;
        float zDistance = distance.z;
        float returnDistance = Mathf.Sqrt((xDistance * xDistance) + (zDistance * zDistance));

        return returnDistance;
    }

    /// <summary>
    /// checks to see if both edges of the raft are on water.
    /// </summary>
    private void CheckEdges()
    {
        /*
        casts a ray downwards
        if the ray does not collide with water and the player is moving left
        stop the rafts horizontal movement
        */
        if (!Physics.Raycast(xLocalMax.position, Vector3.down, halfH, waterMask) || !Physics.Raycast(xLocalMin.position, Vector3.down, halfH, waterMask))
        {
            // TODO
            offWaterPoint = centerPoint;
            velocity.x = 0;
            velocity.z = 0;
            isCompletelyOnWater = false;

        } else
        {
            lastPointInWater = centerPoint;
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
