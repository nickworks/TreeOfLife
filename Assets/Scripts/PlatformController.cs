using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlatformController : MonoBehaviour
{

    /// <summary>
    /// Which layers this object can collide with.
    /// </summary>
    public LayerMask playerMask;

    public Vector3 move;

    public PlayerController player;

    List<PassengerMovement> passengerMovement;

    public const float skinWidth = .015f;
    public int horizontalRayCount = 3;
    public int verticalRayCount = 6;

    public float horizontalRaySpacing;
    public float verticalRaySpacing;

    BoxCollider2D collider;
    public RaycastOrigins raycastOrigins;

    // Use this for initialization
    public void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.deltaTime;

        MovePlayer(velocity);

        //transform.Translate(velocity);
        MovePassangers(true);
        transform.position += velocity;
        //transform.position = move;
        MovePassangers(false);
    }

    void MovePassangers(bool beforeMovePlatform)
    {
        foreach(PassengerMovement passenger in passengerMovement)
        {
            if(passenger.moveBeforePlatform == beforeMovePlatform)
            {
                PawnAABB.CollisionResults results = passenger.transform.GetComponent<PlayerController>().pawn.Move(passenger.velocity);
                passenger.transform.position += results.distance;
            }
        }
    }

    void MovePlayer(Vector3 velocity)
    {
        HashSet<Transform> movedPlayers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        // Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, playerMask);

                if (hit)
                {
                    if (!movedPlayers.Contains(hit.transform))
                    {
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

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, playerMask);

                if (hit)
                {
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

                if (hit)
                {
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

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform t, Vector3 v, bool onPlat, bool beforePlat)
        {
            transform = t;
            velocity = v;
            standingOnPlatform = onPlat;
            moveBeforePlatform = beforePlat;
        }
    }

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

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

}
