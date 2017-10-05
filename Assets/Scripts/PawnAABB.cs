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
        /// After collision detection is calculated, this stores whether or not the object is on a slope.
        /// </summary>
        public bool onSlope;
        /// <summary>
        /// This resets the values and prepares this struct for re-use.
        /// </summary>
        /// <param name="distance">The distance this object is attempting to move.</param>
        public void Reset(Vector3 distance)
        {
            this.distance = distance;
            onSlope = hitTop = hitBottom = hitLeft = hitRight = false;
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
    public float maxSlopeAngle = 45;
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
    public bool renderInEditor = true;
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
    /// This method initializes this component. It is called once.
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

        bounds = aabb.bounds;
        bounds.Expand(skinWidth * -2);
        spaceBetweenVerticalOrigins = bounds.size.x / (resolution - 1);
        spaceBetweenHorizontalOrigins = bounds.size.y / (resolution - 1);

        if (renderInEditor) RenderBounds();

        DoRaycasts(true); // horizontal
        DoRaycasts(false); // vertical

        return results;
    }
    /// <summary>
    /// This method performs the raycasting. If collisions are found, it then calls other functions to limit or adjust the players movement.
    /// </summary>
    /// <param name="doHorizontal">Whether or not to cast rays horizontally. If false, the function will cast rays vertically.</param>
    private void DoRaycasts(bool doHorizontal)
    {
        Vector3 dir = GetCastDirection(doHorizontal);
        float rayLength = GetRayLength(doHorizontal);

        for(int i = 0; i < resolution; i++)
        {
            Vector3 origin = GetOrigin(doHorizontal, i);
            // FIXME: when on a slope (or even a slat surface),
            // the distance.y becomes a negative number.
            // So the origins switch to the top of the AABB, but the direction does not.
            // So rays are consequently cast into the object.

            if (renderInEditor) Debug.DrawRay(origin, dir * rayLength);

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayLength, collidableWith);
            if (hit.collider == null) continue; // if there's no collision, we're done with this raycast

            float hitAngle = Vector2.Angle(hit.normal, Vector3.up);
            if (doHorizontal && i == 0)
            {
                if (hitAngle < maxSlopeAngle) AscendSlope(hitAngle);
            }

            if (hit.distance < rayLength) // if the collision is the closest we've encountered yet
            {
                if (doHorizontal && results.onSlope && hitAngle < maxSlopeAngle) continue;
                rayLength = hit.distance;
                SetRayLength(rayLength, doHorizontal);
            } // if
        } // for
    } // DoRaycasts()
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
    /// <returns></returns>
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
    /// <param name="slopeAngle">The slope of the surface, in degrees.</param>
    private void AscendSlope(float slopeAngle)
    {
        slopeAngle *= Mathf.Deg2Rad;
        float dis = goingLeft ? -results.distance.x : results.distance.x;
        float newDistanceY = dis * Mathf.Sin(slopeAngle);
        if (newDistanceY >= results.distance.y) // prevent a slope from messing up other vertical movmeent
        {
            results.distance.x = dis * Mathf.Cos(slopeAngle) * (goingLeft ? -1 : 1);
            results.distance.y = newDistanceY;
            results.onSlope = true;
        }
    }


}
