using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

/// <summary>
/// This class moves platforms and moves the player with them
/// </summary>
public class PlatformController : MonoBehaviour
{
    /// <summary>
    /// Which layers this object can collide with.
    /// </summary>
    public LayerMask playerMask;
    /// <summary>
    /// This is the movement of the platform on an axis
    /// </summary>
    public Vector3 move;
    /// <summary>
    /// This will hold a reference to the Player object
    /// </summary>
    public PlayerController player;
    /// <summary>
    /// List holds all the passenger movements for the player, based on the PassengerMovement struct
    /// </summary>
    List<PassengerMovement> passengerMovement;
    /// <summary>
    /// This is the width the bounding box is reduced by
    /// </summary>
    public const float skinWidth = .015f;
    /// <summary>
    /// Number of horizontal raycasts the platform will do
    /// </summary>
    public int horizontalRayCount = 3;
    /// <summary>
    /// Number of vertical raycasts the platform will do
    /// </summary>
    public int verticalRayCount = 6;
    /// <summary>
    /// This determines the spacing between each horizontal raycast
    /// </summary>
    public float horizontalRaySpacing;
    /// <summary>
    /// This determines the spacing between each vertical raycast
    /// </summary>
    public float verticalRaySpacing;
    /// <summary>
    /// This holds a reference to the platforms collider
    /// </summary>
    BoxCollider2D collider;
    /// <summary>
    /// This determines the origins of the raycast
    /// </summary>
    public RaycastOrigins raycastOrigins;

    /// <summary>
    /// Initilize this class
    /// </summary>
    public void Start()
    {
        //Set the collider
        collider = GetComponent<BoxCollider2D>();
        //Calculate the space that will be between each ray
        CalculateRaySpacing();
        //Set the player reference
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //This update the origins of the raycast each frame. Helpful when the platform moves
        UpdateRaycastOrigins();

        //Setting the movement to use delta time
        Vector3 velocity = move * Time.deltaTime;

        //This determines how to move the player with the platform
        MovePlayer(velocity);

        
        
        //If the player should move before the platform, move them here
        MovePassangers(true);
        //Move the platform
        transform.position += velocity;
        //If the player should move after the platform, move them here
        MovePassangers(false);
    }

    /// <summary>
    /// This method handles moving the player with the platform
    /// </summary>
    /// <param name="beforeMovePlatform">Should move before platform or after?</param>
    void MovePassangers(bool beforeMovePlatform)
    {
        //Check each passenger
        foreach (PassengerMovement passenger in passengerMovement)
        {
            //Check if the player should move now
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                //Access the players AABB move
                PawnAABB3D.CollisionResults results = passenger.transform.GetComponent<PlayerController>().pawn.Move(passenger.velocity);
                //update the players position
                passenger.transform.position += transform.TransformVector(results.distanceLocal);
            }
        }
    }


    /// <summary>
    /// This method determines how to move the player, depending on the players relation to the platform
    /// </summary>
    /// <param name="velocity">Speed the platform is moving times delta time</param>
    void MovePlayer(Vector3 velocity)
    {
        //Hashset of transforms to check and see if the player has been moved once already
        HashSet<Transform> movedPlayers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();
        
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertical moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            //Raycastying up
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, playerMask);
                //Is there a hit?
                if (hit)
                {
                    //Has the player already moved?
                    if (!movedPlayers.Contains(hit.transform))
                    {
                        //Add the player to hashset, determine how much to move the player
                        movedPlayers.Add(hit.transform);
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        float pushX = (directionY == 1) ? velocity.x : 0;

                        //hit.transform.Translate(new Vector3(pushX, pushY));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                        /*PawnAABB.CollisionResults results = player.pawn.Move(new Vector3(pushX, pushY));
                        player.transform.position += results.distance;*/
                    }//end if hash
                }//end if hit
            }//end for loop
        }//end if vertical


        //horizontal moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            //Horizontal raycasting
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, playerMask);
                //Is there a hit
                if (hit)
                {
                    //Has the player moved?
                    if (!movedPlayers.Contains(hit.transform))
                    {
                        movedPlayers.Add(hit.transform);
                        float pushY = 0;
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;

                        //hit.transform.Translate(new Vector3(pushX, pushY));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                        /*PawnAABB.CollisionResults results = player.pawn.Move(new Vector3(pushX, pushY));
                        player.transform.position += results.distance;*/
                    }//end if hash
                }//end if hit
            }//end for loop
        }//end if vertical*/

        // Player on top platform moving horizontally or down
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, playerMask);
                //Is there a hit?
                if (hit)
                {
                    //Has the player moved?
                    if (!movedPlayers.Contains(hit.transform))
                    {
                        movedPlayers.Add(hit.transform);
                        float pushY = velocity.y;
                        float pushX = velocity.x;

                        //hit.transform.Translate(new Vector3(pushX, pushY));
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                        /*PawnAABB.CollisionResults results = player.pawn.Move(new Vector3(pushX, pushY));
                        player.transform.position += results.distance;*/
                    }//end if hash
                }//end if hit
            }//end for loop
        }//end if vertical
    }
    /// <summary>
    /// This is a struct holding logic for moving the player
    /// </summary>
    struct PassengerMovement
    {

        /// <summary>
        /// Players transform
        /// </summary>
        public Transform transform;
        /// <summary>
        /// How much the player should be moved
        /// </summary>
        public Vector3 velocity;
        /// <summary>
        /// Is the player on the platform?
        /// </summary>
        public bool standingOnPlatform;
        /// <summary>
        /// Should the player move before the platform?
        /// </summary>
        public bool moveBeforePlatform;

        /// <summary>
        /// This method stores information for moving the player
        /// </summary>
        /// <param name="t">The players transform</param>
        /// <param name="v">How much to move the player</param>
        /// <param name="onPlat">On platform?</param>
        /// <param name="beforePlat">Move playuer before moving the platform</param>
        public PassengerMovement(Transform t, Vector3 v, bool onPlat, bool beforePlat)
        {
            transform = t;
            velocity = v;
            standingOnPlatform = onPlat;
            moveBeforePlatform = beforePlat;
        }
    }
    /// <summary>
    /// This method calculates the raycast origins each frame
    /// </summary>
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);


        Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.min.y));
        Debug.DrawLine(new Vector3(bounds.max.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.max.y));
        Debug.DrawLine(new Vector3(bounds.max.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.max.y));
        Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.min.y));
    }
    /// <summary>
    /// This method determines the spcaing between the raycasts
    /// </summary>
    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    /// <summary>
    /// This struct stores the bounds of the platform
    /// </summary>
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
