using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAABB : MonoBehaviour {

    public struct CollisionResults
    {
        /// <summary>
        /// After collision detection, this stores how far this object may move.
        /// Before collision detection, this stores how far this object is attempting to move.
        /// </summary>
        public Vector3 distance;

        public bool hitTop { get; private set; }
        public bool hitBottom { get; private set; }
        public bool hitLeft { get; private set; }
        public bool hitRight { get; private set; }

        public bool onSlope;

        private float dx;
        private float dy;

        private Bounds bounds;
        private float skinWidth;

        public bool goingLeft { get { return (distance.x <= 0); } }
        public bool goingDown { get { return (distance.y <= 0); } }
        public float originX { get { return goingLeft ? this.bounds.min.x : this.bounds.max.x; } }
        public float originY { get { return goingDown ? this.bounds.min.y : this.bounds.max.y; } }

        public CollisionResults(Vector3 distance, Bounds bounds, float skinWidth, int resolution)
        {
            hitTop = false;
            hitBottom = false;
            hitLeft = false;
            hitRight = false;
            onSlope = false;

            bounds.Expand(-skinWidth * 2);
            
            this.distance = distance;
            this.bounds = bounds;
            this.skinWidth = skinWidth;

            dx = bounds.size.x / (resolution - 1);
            dy = bounds.size.y / (resolution - 1);
        }
        public void RenderInEditor()
        {
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.min.y));
            Debug.DrawLine(new Vector3(bounds.max.x, bounds.min.y, 0), new Vector3(bounds.max.x, bounds.max.y));
            Debug.DrawLine(new Vector3(bounds.max.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.max.y));
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, 0), new Vector3(bounds.min.x, bounds.min.y));
        }
        public float GetRayLength(bool isHorizontal)
        {
            if (isHorizontal)
            return skinWidth + (goingLeft ? -distance.x : distance.x);
            return skinWidth + (goingDown ? -distance.y : distance.y);
        }
        public Vector3 GetCastDirection(bool isHorizontal)
        {
            if(isHorizontal)
            return (goingLeft) ? Vector3.left : Vector3.right;
            return (goingDown) ? Vector3.down : Vector3.up;
        }
        public Vector3 GetOrigin(bool isHorizontal, int i)
        {
            Vector3 origin = (isHorizontal)
            ? new Vector3(originX, bounds.min.y + i * dy)
            : new Vector3(bounds.min.x + i * dx, originY);

            return origin;
        }
        public void SetRayLength(float length, bool isHorizontal)
        {
            if (isHorizontal)
            {
                
                if(goingLeft)
                    hitLeft = true;
                else
                    hitRight = true;

                distance.x = goingLeft ? -length : length;

            } else {

                if (goingDown)
                    hitBottom = true;
                else
                    hitTop = true;

                distance.y = goingDown ? -length : length;
            }
        }
    }

    BoxCollider2D aabb;
    public float maxSlopeAngle = 45;
    [Range(3, 10)] public int resolution = 3;
    [Range(0.01f, 0.5f)] public float skinWidth = 0.1f;
    public bool renderInEditor = true;
    public LayerMask collidableWith;

    void Start()
    {
        aabb = GetComponent<BoxCollider2D>();
    }

	public CollisionResults Move(Vector3 distance)
    {

        CollisionResults results = new CollisionResults(distance, aabb.bounds, skinWidth, resolution);

        if (renderInEditor) results.RenderInEditor();

        DoRaycasts(ref results, true); // horizontal
        DoRaycasts(ref results, false); // vertical

        return results;
    }
    private void DoRaycasts(ref CollisionResults results, bool doHorizontal)
    {
        Vector3 dir = results.GetCastDirection(doHorizontal);
        float rayLength = results.GetRayLength(doHorizontal);

        for(int i = 0; i < resolution; i++)
        {
            Vector3 origin = results.GetOrigin(doHorizontal, i);

            if (renderInEditor) Debug.DrawRay(origin, dir * rayLength);

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, rayLength, collidableWith);
            if (hit.collider == null) continue; // if there's no collision, we're done with this raycast

            float hitAngle = Vector2.Angle(hit.normal, Vector3.up);
            if (doHorizontal && i == 0)
            {
                if(hitAngle < maxSlopeAngle)
                {
                    hitAngle *= Mathf.Deg2Rad;
                    float dis = results.goingLeft ? -results.distance.x : results.distance.x;
                    float newDistanceY = dis * Mathf.Sin(hitAngle);
                    if (newDistanceY >= results.distance.y) // prevent a slope from messing up other vertical movmeent
                    {
                        results.distance.x = dis * Mathf.Cos(hitAngle) * (results.goingLeft ? -1 : 1);
                        results.distance.y = newDistanceY;
                        results.onSlope = true;
                    }
                }
            }

            if (hit.distance < rayLength) // if the collision is the closest we've encountered yet
            {
                if (doHorizontal && results.onSlope && hitAngle < maxSlopeAngle) continue;
                rayLength = hit.distance;
                results.SetRayLength(rayLength - skinWidth, doHorizontal);
            } // if
        } // for

    } // DoRaycasts()
}
