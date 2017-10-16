using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component gives this GameObject an AABB and allows for collision detection with the Physics2D engine.
/// The pawn also has the ability to ascend slopes.
/// </summary>
public class PawnAABB : MonoBehaviour {

    /// <summary>
    /// This struct contains information about collision detection,
    /// including how far the object is allowed to move and which edges were hit.
    /// </summary>
    public struct CollisionResults
    {
        /// <summary>
        /// After collision detection, this stores how far this object may move.
        /// Before collision detection, this stores how far this object is attempting to move.
        /// </summary>
        public Vector3 distance;
        /// <summary>
        /// After collision detection is calculated, this stores whether or not the object hit its top edge on something.
        /// </summary>
        public bool hitTop;
        /// <summary>
        /// After collision detection is calculated, this stores whether or not the object hit its bottom edge on something.
        /// </summary>
        public bool hitBottom;
        /// <summary>
        /// After collision detection is calculated, this stores whether or not the object hit its left edge on something.
        /// </summary>
        public bool hitLeft;
        /// <summary>
        /// After collision detection is calculated, this stores whether or not the object hit its right edge on something.
        /// </summary>
        public bool hitRight;
        /// <summary>
        /// After collision detection is calculated, this stores whether or not the object is going up a slope.
        /// </summary>
        public bool ascendSlope;
        /// <summary>
        /// After collision detection is calculated, this stores whether or not the object is going down slope.
        /// </summary>
        public bool descendSlope;
        /// <summary>
        /// The angle of the slope on this frame.
        /// </summary>
        public float slopeAngle;
        /// <summary>
        /// The angle of the slope on the last frame.
        /// </summary>
        public float slopeAnglePrevious;

        /// <summary>
        /// This resets the values and prepares this struct for re-use.
        /// </summary>
        /// <param name="distance">The distance this object is attempting to move.</param>
        public void Reset(Vector3 distance)
        {
            this.distance = distance;
            ascendSlope = descendSlope = hitTop = hitBottom = hitLeft = hitRight = false;
            slopeAnglePrevious = slopeAngle;
            slopeAngle = 0;
        }   
    }

    /// <summary>
    /// This field stores a reference to the GameObject's BoxCollider component.
    /// </summary>
    private BoxCollider2D aabb;

    /// <summary>
    /// This contains the results of collision detection.
    /// </summary>
    private CollisionResults results = new CollisionResults();

    #region Inspector Properties
    /// <summary>
    /// This is the maximum angle, in degrees, that this PawnAABB can ascend.
    /// </summary>
    [Range(0, 90)] public float maxSlopeAscend = 80;
    /// <summary>
    /// This is the maximum angle, in degrees, that the PawnAABB can smoothly descend without falling/skipping.
    /// </summary>
    [Range(0, 90)] public float maxSlopeDescend = 70;
    /// <summary>
    /// This is how many rays we want to cast from each edge.
    /// </summary>
    [Range(3, 10)] public int resolution = 3;
    /// <summary>
    /// This stores the skinWidth to use for collision detection. The skinWidth is how
    /// far into the object to cast rays from. Having skinWidth prevents the corners of
    /// the box from getting caught on level geometry.
    /// </summary>
    [Range(0.01f, 0.5f)] public float skinWidth = 0.1f;
    /// <summary>
    /// Whether or not to render into the Scene view details about this component. In this case, we draw things like raycasts.
    /// </summary>
    public bool renderInEditor = true;
    /// <summary>
    /// Which layers this object can collide with.
    /// </summary>
    public LayerMask collidableWith;
    #endregion

    #region Collision Detection Data
    /// <summary>
    /// This stores the amount of horizontal space between the origins of rays being
    /// cast vertically. We use this value when calculating the positions of origins.
    /// </summary>
    private float spaceBetweenVerticalOrigins;
    /// <summary>
    /// This stores the amount of vertical space between the origins of rays being
    /// cast horizontally. We use this value when calculating the positions of origins.
    /// </summary>
    private float spaceBetweenHorizontalOrigins;
    /// <summary>
    /// This stores the bounds of the PawnAABB's AABB box. Because it is shrunken using
    /// skinWidth, this value should be smaller than the CollisionBox2D's bounds.
    /// </summary>
    private Bounds bounds;
    /// <summary>
    /// Returns -1 when moving left and +1 when moving right. Also returns -1 if not moving horizontally.
    /// </summary>
    private int signX { get { return goingLeft ? -1 : 1; } }
    /// <summary>
    /// Returns -1 when moving down and +1 when moving up. Also returns -1 if not moving vertically.
    /// </summary>
    private int signY { get { return goingDown ? -1 : 1; } }
    /// <summary>
    /// Whether or not the PawnAABB is moving to the left. If this value is false, we can assume the PawnAABB is moving to the right.
    /// </summary>
    private bool goingLeft { get { return (results.distance.x <= 0); } }
    /// <summary>
    /// Whether or not the PawnAABB is moving down. If this value if false, we can assume the PawnAABB is moving up.
    /// </summary>
    private bool goingDown { get { return (results.distance.y <= 0); } }
    /// <summary>
    /// For rays being cast horizontally, this returns the x-value of the rays' origins.
    /// </summary>
    private float originX { get { return goingLeft ? bounds.min.x : bounds.max.x; } }
    /// <summary>
    /// For rays being cast vertically, this returns the y-value of the rays' origins.
    /// </summary>
    private float originY { get { return goingDown ? bounds.min.y : bounds.max.y; } }
    #endregion

    /// <summary>
    /// This message is called by Unity after the game object spawns. It is called once.
    /// </summary>
    void Start()
    {
        aabb = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// This method should be called when a PawnAABB is attempting to move.
    /// </summary>
    /// <param name="distance">How far the object is trying to move from its previous position. In many cases, this can be the object's current velocity.</param>
    /// <returns>The results of collision detection. This includes a potentially modified distance value and additional information about which edges were hit.</returns>
	public CollisionResults Move(Vector3 distance)
    {

        results.Reset(distance);
        CalculateEdges();

        if (renderInEditor) RenderBounds();

        if (distance.y < 0) DescendSlope();
        DoRaycasts(true); // horizontal
        if (results.ascendSlope) ExtraRaycastFromToes();
        DoRaycasts(false); // vertical
        

        return results;
    }

    /// <summary>
    /// This method uses the skinWidth to create a shrunken copy of the Collider's aabb. Then it calculates and caches the distance to use for spreading out the raycast origins.
    /// </summary>
    private void CalculateEdges()
    {
        bounds = aabb.bounds;
        bounds.Expand(skinWidth * -2);
        spaceBetweenVerticalOrigins = bounds.size.x / (resolution - 1);
        spaceBetweenHorizontalOrigins = bounds.size.y / (resolution - 1);
    }
    /// <summary>
    /// This method performs the raycasting. If collisions are found, it then calls other functions to limit or adjust the players movement.
    /// </summary>
    /// <param name="doHorizontal">Whether or not to cast rays horizontally. If false, the function will cast rays vertically.</param>
    private void DoRaycasts(bool doHorizontal)
    {
        
        float rayLength = GetRayLength(doHorizontal);

        for(int i = 0; i < resolution; i++)
        {
            Vector3 dir = GetCastDirection(doHorizontal);
            Vector3 origin = GetOrigin(doHorizontal, i);

            if (renderInEditor) Debug.DrawRay(origin, dir * rayLength);

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayLength, collidableWith);
            if (hit.collider == null) continue; // if there's no collision, we're done with this raycast

            float slopeAngle = GetSlopeAngleFromNormal(hit.normal);

            if (doHorizontal)
            {
                if (i == 0 && slopeAngle <= maxSlopeAscend)
                {
                    AscendSlope(slopeAngle);
                    // FIXME: somehow the front toe is getting caught in the slope sometimes
                    //results.distance.x += (hit.distance - skinWidth) * Mathf.Sign(results.distance.x); // maybe this will help?
                }
                if((!results.ascendSlope && !results.descendSlope) || slopeAngle > maxSlopeAscend) // if we're not ascending OR if the slope is too steep to climb
                {
                    rayLength = hit.distance;
                    SetRayLength(rayLength, doHorizontal);
                }
            } else
            {
                if (hit.distance < rayLength) // if the collision is the closest we've encountered yet
                {
                    if (results.descendSlope && !goingLeft && i == 0) continue;
                    if (results.descendSlope && goingLeft && i == resolution - 1) continue;

                    rayLength = hit.distance;
                    SetRayLength(rayLength, doHorizontal);
                }
            }
        } // for
    } // DoRaycasts()
    /// <summary>
    /// This sends out a raycast forward, out from the player's "toes". It allows us to check for new slopes while ascending.
    /// </summary>
    private void ExtraRaycastFromToes()
    {
        // check for a yet steeper collision ahead of us
        float rayLength = GetRayLength(true);
        Vector2 origin = new Vector2(goingLeft ? bounds.min.x : bounds.max.x, bounds.min.y + results.distance.y); // get origin (player's toe)
        Vector2 dir = goingLeft ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayLength, collidableWith);
        if (renderInEditor) Debug.DrawRay(origin, dir * rayLength, Color.blue);
        if (hit)
        {
            float slopeAngle = GetSlopeAngleFromNormal(hit.normal);
            if (slopeAngle != results.slopeAngle) // we hit a new slope!
            {
                results.distance.x = (hit.distance - skinWidth) * signX;
                results.slopeAngle = slopeAngle;
            }
        }
    }
    /// <summary>
    /// This method renders the bounds in the editor. This draws a rectangle of the shrunken AABB.
    /// </summary>
    private void RenderBounds()
    {
        Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.min.y));
        Debug.DrawLine(new Vector3(bounds.max.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.max.y));
        Debug.DrawLine(new Vector3(bounds.max.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.max.y));
        Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.min.y));
    }
    /// <summary>
    /// This method returns the length to cast a ray.
    /// </summary>
    /// <param name="isHorizontal">Whether or not we want the length of the horizontal rays. If this value is false, the method will return the length of the vertical rays.</param>
    /// <returns>The length of the ray to cast. This should always be a positive number, and it includes skinWidth in its calculation.</returns>
    private float GetRayLength(bool isHorizontal)
    {
        if (isHorizontal)
        return skinWidth + (goingLeft ? -results.distance.x : results.distance.x);
        return skinWidth + (goingDown ? -results.distance.y : results.distance.y);
    }
    /// <summary>
    /// This method determines the direction to cast the rays, based on the direction the object is trying to move.
    /// </summary>
    /// <param name="isHorizontal">Whether or not we want the horizontal direction. If this is false, the method will return the vertical direction.</param>
    /// <returns>This is a directional Vector3. It will be one of the following: up, down, left, or right.</returns>
    private Vector3 GetCastDirection(bool isHorizontal)
    {
        if (isHorizontal)
        return (goingLeft) ? Vector3.left : Vector3.right;
        return (goingDown) ? Vector3.down : Vector3.up;
    }
    /// <summary>
    /// This method returns an origin of where to cast a ray from.
    /// </summary>
    /// <param name="isHorizontal">Whether or not we want the origins for horizontal rays. If this is false, the function will return an origin for a vertical ray.</param>
    /// <param name="i">Which origin do we want? As this number increases, the function will offset the position and calculate the next origin on the same edge.</param>
    /// <returns>A raycast origin</returns>
    private Vector3 GetOrigin(bool isHorizontal, int i)
    {
        Vector3 origin = (isHorizontal)
        ? new Vector3(originX, bounds.min.y + i * spaceBetweenHorizontalOrigins)
        : new Vector3(bounds.min.x + i * spaceBetweenVerticalOrigins, originY);

        return origin;
    }
    /// <summary>
    /// This method reduces the distance the PawnABB is allowed to move. The method should be called only when a collision has been found.
    /// </summary>
    /// <param name="length">The new length of the ray. This should be a positive number, and it should include skinWidth.</param>
    /// <param name="isHorizontal">Whether or not we are setting the horizontal distance. If this is false, we will set the vertical distance.</param>
    private void SetRayLength(float length, bool isHorizontal)
    {
        length -= skinWidth;
        if (length < 0) length = 0;
        if (isHorizontal)
        {
            results.hitLeft = goingLeft;
            results.hitRight = !goingLeft;
            results.distance.x = goingLeft ? -length : length;
        }
        else
        {
            results.hitBottom = goingDown;
            results.hitTop = !goingDown;
            results.distance.y = goingDown ? -length : length;
        }
    }
    /// <summary>
    /// This method should be called if the player is attempting to ascend a sloped surface.
    /// The function will use the slope to determine an all new distance for the player to attempt to move.
    /// </summary>
    /// <param name="slopeDegrees">The slope of the surface, in degrees.</param>
    private void AscendSlope(float slopeDegrees)
    {
        float slopeRadians = slopeDegrees * Mathf.Deg2Rad;
        float dis = goingLeft ? -results.distance.x : results.distance.x;
        float newDistanceY = dis * Mathf.Sin(slopeRadians);

        if (newDistanceY < results.distance.y) return; // If moving up the slope would result in LESS height gained, then don't bother ascending the slope.
        
        results.distance.x = dis * Mathf.Cos(slopeRadians) * signX;
        results.distance.y = newDistanceY;
        results.slopeAngle = slopeDegrees;
        results.ascendSlope = true;
        results.hitBottom = true;
        
    }
    /// <summary>
    /// This method casts a ray down. If it hits ground, the slope of the ground is calculated.
    /// If the slope is less than maxSlopeDescend, horizontal movement is translated into movement along the surface.
    /// </summary>
    private void DescendSlope()
    {
        Vector2 origin = new Vector2(goingLeft ? bounds.max.x : bounds.min.x, bounds.min.y); // cast from the bottom, trailing corner
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, float.PositiveInfinity, collidableWith); // cast ray down!

        if (renderInEditor) Debug.DrawRay(origin, Vector2.down * 10, Color.blue);

        if (hit) // there's ground below us!
        {            
            float slopeDegrees = GetSlopeAngleFromNormal(hit.normal); // get the angle of the slope of that ground below us
            if(slopeDegrees > 0 && slopeDegrees <= maxSlopeDescend) // we only do this trick for slopes between 0 and maxSlopeDescend
            {
                bool slopeDescendsLeft = (hit.normal.x <= 0); // whether or not the slope descends to the left (/ true) (\ false)
                
                if (slopeDescendsLeft == goingLeft) // If the player is moving down the slope... (either left or right)
                {
                    float slopeRadians = slopeDegrees * Mathf.Deg2Rad; // get radians
                    float distanceX = Mathf.Abs(results.distance.x); // absolute value of horizontal distance
                    float distanceToHit = hit.distance - skinWidth;

                    float slope = Mathf.Tan(slopeRadians);
                    float dropFromSlope = slope * distanceX; // if we were moving down this slope, our horizontal "run" would result in how much "rise"?

                    if (distanceToHit <= dropFromSlope) // the ground is closer than the slope's drop in height
                    {

                        float howFarToDrop = distanceX * Mathf.Sin(slopeRadians); // calculate how far to move DOWN
                        results.distance.x = distanceX * Mathf.Cos(slopeRadians) * signX; // calculate how far to move LEFT / RIGHT 
                        results.distance.y = -(distanceToHit + howFarToDrop);

                        results.hitBottom = true;
                        results.slopeAngle = slopeDegrees;
                        results.descendSlope = true;
                    }
                }
            }
        }

    }
    /// <summary>
    /// Given a surface's normal, get the angle of the surface's slope.
    /// </summary>
    /// <param name="normal">The angle of a surface normal.</param>
    /// <returns>The angle of the slope, in degrees.</returns>
    private float GetSlopeAngleFromNormal(Vector2 normal)
    {
        return Vector2.Angle(normal, Vector3.up);
    }
}
