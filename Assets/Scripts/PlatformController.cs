using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;


//FIXME: Comment all variables and classes
public class PlatformController : MonoBehaviour
{

    /// <summary>
    /// Which layers this object can collide with.
    /// </summary>
    public LayerMask playerMask;
    //FIXME: Comment all variables and classes
    //FIXME: Make a vector 2 so nobody tinkers with the z axis but this will probably get reworked cause of the whole pathing thing
    public Vector3 move;
    //FIXME: Comment all variables and classes
    public PlayerController player;
    //FIXME: Comment all variables and classes
    List<PassengerMovement> passengerMovement;
    //FIXME: Comment all variables and classes
    public const float skinWidth = .015f;
    //FIXME: Comment all variables and classes
    public int horizontalRayCount = 3;
    //FIXME: Comment all variables and classes
    public int verticalRayCount = 6;
    //FIXME: Comment all variables and classes
    public float horizontalRaySpacing;
    //FIXME: Comment all variables and classes
    public float verticalRaySpacing;
    //FIXME: Comment all variables and classes
    BoxCollider2D collider;
    //FIXME: Comment all variables and classes
    public RaycastOrigins raycastOrigins;

    //FIXME: Comment what you are doing here
    public void Start()
    {
        //FIXME: Comment what you are doing here
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //FIXME: Comment what you are doing here
        UpdateRaycastOrigins();

        //FIXME: Comment what you are doing here
        Vector3 velocity = move * Time.deltaTime;
        //FIXME: Comment what you are doing here
        MovePlayer(velocity);
        //FIXME: Comment what you are doing here
        //transform.Translate(velocity);
        MovePassangers(true);
        //FIXME: Comment what you are doing here
        transform.position += velocity;
        //FIXME: Comment what you are doing here
        //transform.position = move;
        MovePassangers(false);
    }

    //FIXME: Comment what you are doing here
    void MovePassangers(bool beforeMovePlatform)
    {
        //FIXME: What is this for each loop doing
        foreach (PassengerMovement passenger in passengerMovement)
        {
            //FIXME: What is this if statement doing
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                //FIXME: Comment what you are doing here
                PawnAABB.CollisionResults results = passenger.transform.GetComponent<PlayerController>().pawn.Move(passenger.velocity);
                //FIXME: Comment what you are doing here
                passenger.transform.position += results.distance;
            }
        }
    }


    //FIXME: Comment what you are doing here
    void MovePlayer(Vector3 velocity)
    {
        //FIXME: Comment what you are doing here
        HashSet<Transform> movedPlayers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();
        //FIXME: Comment what you are doing here
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //FIXME: What is this if statement doing
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            //FIXME: What is this for loop doing
            for (int i = 0; i < verticalRayCount; i++)
            {
                //FIXME: Comment what you are doing here
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                //FIXME: Comment what you are doing here
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                //FIXME: Comment what you are doing here
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, playerMask);
                //FIXME: What is this if statement doing
                if (hit)
                {
                    //FIXME: What is this if statement doing
                    if (!movedPlayers.Contains(hit.transform))
                    {
                        //FIXME: Comment what you are doing here
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


        //FIXME: What is this if statement doing
        //horizontal moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            //FIXME: What is this for loop doing
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, playerMask);
                //FIXME: What is this if statement doing
                if (hit)
                {
                    //FIXME: What is this if statement doing
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


        //FIXME: What is this if statement doing
        // Player on top platform moving horizontally or down
        if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
        {
            float rayLength = skinWidth * 2;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, playerMask);
                //FIXME: What is this if statement doing
                if (hit)
                {
                    //FIXME: What is this if statement doing
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
    //FIXME: Comment what you are doing here
    struct PassengerMovement
    {

        //FIXME: What are these variables for
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        //FIXME: What are you doing here
        public PassengerMovement(Transform t, Vector3 v, bool onPlat, bool beforePlat)
        {
            transform = t;
            velocity = v;
            standingOnPlatform = onPlat;
            moveBeforePlatform = beforePlat;
        }
    }
    //FIXME: Comment what you are doing here
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
    //FIXME: Comment what you are doing here
    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    //FIXME: Comment what you are doing here
    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
