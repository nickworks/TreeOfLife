using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component gives the GameObject a 2D AABB that prevents collisions with 3D colliders. Weird.
/// The pawn also has the ability to ascend slopes.
/// </summary>
public class PawnAABB3D : MonoBehaviour
{

    /// <summary>
    /// This struct contains information about collision detection,
    /// including how far the object is allowed to move and which edges were hit.
    /// </summary>
    public struct CollisionResults
    {
        
        /// <summary>
        /// After collision detection, this stores how far this object may move.
        /// Before collision detection, this stores how far this object is attempting to move.
        /// This value is in LOCAL space. If you are going to add this to transform.position, first convert it to world-space coordinates by using transform.TransformVector().
        /// </summary>
        public Vector3 distanceLocal;

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
            this.distanceLocal = distance;
            ascendSlope = descendSlope = hitTop = hitBottom = hitLeft = hitRight = false;
            slopeAnglePrevious = slopeAngle;
            slopeAngle = 0;
        }
    }

    /// <summary>
    /// This contains the results of collision detection.
    /// </summary>
    private CollisionResults results = new CollisionResults();

    private const float HALF_PI = Mathf.PI / 2;

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
    public float halfWidth = .4f;
    public float halfHeight = .4f;
    #endregion

    #region Collision Detection Data
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
    private bool goingLeft;
    /// <summary>
    /// Whether or not the PawnAABB is moving down. If this value if false, we can assume the PawnAABB is moving up.
    /// </summary>
    private bool goingDown;
    #endregion

    /// <summary>
    /// This message is called by Unity after the game object spawns. It is called once.
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// This method should be called when a PawnAABB is attempting to move along the curved XY "plane".
    /// </summary>
    /// <param name="distance">How far the object is trying to move from its previous position. In many cases, this can be the object's current velocity. This velocity should be 2D (XY values)!</param>
    /// <returns>The results of collision detection. This includes a potentially modified distance value and additional information about which edges were hit.</returns>
	public CollisionResults Move(Vector3 distance)
    {
        goingLeft = distance.x <= 0;
        goingDown = distance.y <= 0;

        results.Reset(distance);

        if (renderInEditor) RenderBounds();

        if (distance.y < 0) DescendSlope();
        DoRaycasts(true); // horizontal
        //if (results.ascendSlope) ExtraRaycastFromToes();
        DoRaycasts(false); // vertical
        FinalRaycast();
        
        return results;
    }
    /// <summary>
    /// This raycast fixes a troubling edge case where the player would get stuck in a slope or pass through an object when colliding with a corner.
    /// It casts a ray diagonally in the direction the player is moving.
    /// </summary>
    private void FinalRaycast()
    {
        if (results.hitBottom || results.hitLeft || results.hitRight || results.hitTop) return;

        Vector3 origin = GetPointInQuad(goingLeft ? 0 : 1, goingDown ? 0 : 1);
        float originalDistance = results.distanceLocal.magnitude + skinWidth;
        Vector3 dir = transform.TransformVector(results.distanceLocal.normalized * originalDistance);

        if(renderInEditor) Debug.DrawRay(origin, dir);

        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, originalDistance, collidableWith))
        {
            if (hit.distance < originalDistance)
            {
                //print("corner fix");
                results.distanceLocal.x = (dir.normalized * hit.distance).x;
                results.distanceLocal.x += goingLeft ? skinWidth : -skinWidth;
                results.hitLeft = goingLeft;
                results.hitRight = !goingLeft;
            }
        }
    }

    /// <summary>
    /// This method performs the raycasting. If collisions are found, it then calls other functions to limit or adjust the players movement.
    /// </summary>
    /// <param name="doHorizontal">Whether or not to cast rays horizontally. If false, the function will cast rays vertically.</param>
    private void DoRaycasts(bool doHorizontal)
    {
        float rayLength = GetRayLength(doHorizontal);

        for (int i = 0; i < resolution; i++)
        {
            Vector3 dir = GetCastDirection(doHorizontal);
            Vector3 origin = GetOrigin(doHorizontal, i);
            Ray ray = new Ray(origin, dir);
            RaycastHit hit;

            if (renderInEditor) Debug.DrawRay(origin, dir * rayLength);

            if (!Physics.Raycast(ray, out hit, rayLength, collidableWith)) continue; // return if there's no collision

            float slopeDegrees = GetSlopeAngleFromNormal(hit.normal);

            if (doHorizontal)
            {
                // slope ascending is currently handled when casting horizontal rays
                // but the vertical rays seem to be causing ascending as well...

                if (i == 0 && Mathf.Abs(slopeDegrees) <= maxSlopeAscend)
                {
                    AscendSlope(slopeDegrees * Mathf.Deg2Rad);
                }
                if ((!results.ascendSlope && !results.descendSlope) || slopeDegrees > maxSlopeAscend) // if we're not ascending OR if the slope is too steep to climb
                {
                    if (hit.distance < rayLength)
                    {
                        rayLength = hit.distance;
                        SetRayLength(rayLength, doHorizontal);
                    }
                }
            }
            else
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
        /*
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
        */
    }
    /// <summary>
    /// This method renders the bounds in the editor. This draws a rectangle of the shrunken AABB.
    /// </summary>
    private void RenderBounds()
    {
        Vector3 corner1 = GetPointInQuad(0, 0);
        Vector3 corner2 = GetPointInQuad(1, 0);
        Vector3 corner3 = GetPointInQuad(1, 1);
        Vector3 corner4 = GetPointInQuad(0, 1);

        Debug.DrawLine(corner1, corner2);
        Debug.DrawLine(corner2, corner3);
        Debug.DrawLine(corner3, corner4);
        Debug.DrawLine(corner4, corner1);
    }
    /// <summary>
    /// This method returns the length to cast a ray.
    /// </summary>
    /// <param name="isHorizontal">Whether or not we want the length of the horizontal rays. If this value is false, the method will return the length of the vertical rays.</param>
    /// <returns>The length of the ray to cast. This should always be a positive number, and it includes skinWidth in its calculation.</returns>
    private float GetRayLength(bool isHorizontal)
    {
        if (isHorizontal)
        return skinWidth + (goingLeft ? -results.distanceLocal.x : results.distanceLocal.x);
        return skinWidth + (goingDown ? -results.distanceLocal.y : results.distanceLocal.y);
    }
    /// <summary>
    /// This method determines the direction to cast the rays, based on the direction the object is trying to move.
    /// </summary>
    /// <param name="isHorizontal">Whether or not we want the horizontal direction. If this is false, the method will return the vertical direction.</param>
    /// <returns>This is a directional Vector3. It will be one of the following: up, down, left, or right.</returns>
    private Vector3 GetCastDirection(bool isHorizontal)
    {
        if (isHorizontal)
        return (goingLeft) ? -transform.right : transform.right;
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
        ? GetPointInQuad(goingLeft ? 0: 1, i / (resolution - 1f))
        : GetPointInQuad(i / (resolution - 1f), goingDown ? 0 : 1);

        return origin;
    }
    /// <summary>
    /// Returns the world position of a point within the player's 2D AABB. Note: the input parameters are not clamped within the method.
    /// </summary>
    /// <param name="px">The horizontal position. The range of values is [0.0 to 1.0].</param>
    /// <param name="py">The vertical position. The range of values is [0.0 to 1.0].</param>
    /// <returns></returns>
    private Vector3 GetPointInQuad(float px, float py)
    {
        // TODO: we should cache the transformed max and min and only do two lerps here (no transformPoint())
        float x = Mathf.Lerp(-halfWidth, halfWidth, px);
        float y = Mathf.Lerp(-halfHeight, halfHeight, py);

        return transform.TransformPoint(new Vector3(x, y, 0));
    }
    /// <summary>
    /// This method reduces the distance the PawnABB is allowed to move. The method should be called only when a collision has been found.
    /// </summary>
    /// <param name="length">The new length of the ray. This should be a positive number, and it should include skinWidth.</param>
    /// <param name="isHorizontal">Whether or not we are setting the horizontal distance. If this is false, we will set the vertical distance.</param>
    private void SetRayLength(float length, bool isHorizontal)
    {
        length -= skinWidth;
        // DON'T clamp length to 0, otherwise we can't move the player out of a collider if they end up slightly within one

        if (isHorizontal)
        {
            results.hitLeft = goingLeft;
            results.hitRight = !goingLeft;

            results.distanceLocal.x = goingLeft ? -length : length;
        }
        else
        {
            results.hitBottom = goingDown;
            results.hitTop = !goingDown;
            results.distanceLocal.y = goingDown ? -length : length;
        }
    }
    /// <summary>
    /// This method should be called if the player is attempting to ascend a sloped surface.
    /// The function will use the slope to determine an all new distance for the player to attempt to move.
    /// </summary>
    /// <param name="slopeDegrees">The slope of the surface, in degrees.</param>
    private void AscendSlope(float slopeRadians)
    {
        float dis = results.distanceLocal.x;// goingLeft ? -results.distanceLocal.x : results.distanceLocal.x;
        float newDistanceY = -dis * Mathf.Sin(slopeRadians);
        if (newDistanceY < results.distanceLocal.y) return; // If moving up the slope would result in LESS height gained, then don't bother ascending the slope.
        
        results.distanceLocal.x = dis * Mathf.Cos(slopeRadians);
        results.distanceLocal.y = newDistanceY;
        results.slopeAngle = slopeRadians * Mathf.Rad2Deg;
        results.ascendSlope = true;
        results.hitBottom = true;
    }
    /// <summary>
    /// This method casts a ray down. If it hits ground, the slope of the ground is calculated.
    /// If the slope is less than maxSlopeDescend, horizontal movement is translated into movement along the surface.
    /// </summary>
    private void DescendSlope()
    {
        Vector3 origin = GetPointInQuad(goingLeft ? 1 : 0, 0);
        if (renderInEditor) Debug.DrawRay(origin, Vector2.down * 10, Color.blue);

        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, float.PositiveInfinity, collidableWith)) // there's ground below us!
        {
            float slopeDegrees = GetSlopeAngleFromNormal(hit.normal); // get the angle of the slope of that ground below us
            if (Mathf.Abs(slopeDegrees) <= maxSlopeDescend) // we only do this trick for slopes below maxSlopeDescend
            {
                bool slopeDescendsLeft = (slopeDegrees <= 0); // whether or not the slope descends to the left (/ true) (\ false)
                if (slopeDescendsLeft == goingLeft) // If the player is moving down the slope... (either left or right)
                {
                    float distanceX = results.distanceLocal.x; // value of horizontal distance
                    float distanceToHit = hit.distance - skinWidth;
                    float slopeRadians = slopeDegrees * Mathf.Deg2Rad;
                    float slope = Mathf.Tan(slopeRadians);
                    float dropFromSlope = slope * distanceX; // if we were moving down this slope, our horizontal "run" would result in how much "rise"?

                    if (distanceToHit <= dropFromSlope) // the ground is closer than the slope's drop in height
                    {
                        float howFarToDrop = distanceX * Mathf.Sin(slopeRadians); // calculate how far to move DOWN
                        results.distanceLocal.x = distanceX * Mathf.Cos(slopeRadians); // calculate how far to move LEFT / RIGHT 
                        results.distanceLocal.y = -(distanceToHit + howFarToDrop);

                        results.hitBottom = true;
                        results.slopeAngle = slopeDegrees;
                        results.descendSlope = true;
                    }
                }
            }
        }
    }
    /// <summary>
    /// Given a surface's normal, get the angle of the surface's slope along the transform's right direction.
    /// </summary>
    /// <param name="normal">The angle of a surface normal.</param>
    /// <returns>The angle of the slope, in degrees.</returns>
    private float GetSlopeAngleFromNormal(Vector3 normal)
    {
        float maxSlope = Vector3.Angle(normal, Vector3.up);
        Vector3 flattenedNormal = new Vector3(normal.x, 0, normal.z).normalized;
        float percent = Vector3.Dot(transform.right, flattenedNormal);
        return maxSlope * percent;
    }
}
