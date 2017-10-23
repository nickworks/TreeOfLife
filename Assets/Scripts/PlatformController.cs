using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{

    /// <summary>
    /// Which layers this object can collide with.
    /// </summary>
    public LayerMask playerMask;

    public Vector3 move;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastOrigins();

        Vector3 velocity = move * Time.deltaTime;

        MovePlayer(velocity);
        transform.Translate(velocity);
    }

    void MovePlayer(Vector3 velocity)
    {
        HashSet<Transform> movedPlayers = new HashSet<Transform>();

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

                        hit.transform.Translate(new Vector3(pushX, pushY));
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

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }//end if hash
                }//end if hit
            }//end for loop
        }//end if vertical

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

                        hit.transform.Translate(new Vector3(pushX, pushY));
                    }//end if hash
                }//end if hit
            }//end for loop
        }//end if vertical
    }
}
