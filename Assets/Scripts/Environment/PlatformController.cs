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
    /// This is the movement of the platform on an axis
    /// </summary>
    [HideInInspector]
    public Vector3 move;

    //MoveAlongPath logic
    /// <summary>
    /// The speed this object should move, measured in meters/second.
    /// </summary>
    public float speed = 1;
    /// <summary>
    /// The points that form the path. These points are all relative to this object's starting world position.
    /// </summary>
    public List<Vector3> points = new List<Vector3>();
    /// <summary>
    /// The current index to use for the nextPoint.
    /// </summary>
    private int index = 1;
    /// <summary>
    /// How much time has elapsed since we've been Lerping between the prevPoint and nextPoint.
    /// </summary>
    private float timeElapsed = 0;
    /// <summary>
    /// The total amount of time (in seconds) to use for the current Lerp. This value will change to reflect the distances between pairs of points.
    /// </summary>
    private float timeTotal = 1;
    /// <summary>
    /// The world position of where this object initially spawned.
    /// </summary>
    private Vector3 startingPosition;
    /// <summary>
    /// The position of the nextPoint in the path. This is reflected as a relative distance from startingPosition.
    /// </summary>
    private Vector3 nextPoint;
    /// <summary>
    /// The position of the prevPoint in the path. This is reflected as a relative distance from startingPosition.
    /// </summary>
    private Vector3 prevPoint;

    /// <summary>
    /// Initilize this class
    /// </summary>
    public void Start()
    {

        startingPosition = transform.localPosition;
        SetPoints();
    }
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {

        MoveAlongPath();

        //Setting the movement to use delta time
        Vector3 velocity = move * Time.deltaTime;

        //Move the platform
        transform.position += transform.TransformVector(velocity);
    }

    /// <summary>
    /// This method controls moving the platform between its previous point and it's next point
    /// </summary>
    void MoveAlongPath()
    {
        timeElapsed += Time.deltaTime;
        if (points.Count == 0) return;

        float percent = timeElapsed / timeTotal;
        if (percent > 1)
        {
            timeElapsed -= timeTotal; // calc leftover timeElapsed
            index++; // increase index
            SetPoints(); // set the points
            timeTotal = (nextPoint - prevPoint).magnitude / speed; // using the distance and desired speed, set the timeTotal
            percent = timeElapsed / timeTotal; // calculate the new percentage based on the leftover timeElapsed
        }
        move = Vector3.Lerp(prevPoint, nextPoint, percent);

    }

    /// <summary>
    /// This method sets the two points that we should interpolate between.
    /// </summary>
    private void SetPoints()
    {
        if (index < 0 || index >= points.Count) index = 0;
        int prevIndex = index - 1;
        if (prevIndex < 0) prevIndex = points.Count - 1;
        prevPoint = points[prevIndex];
        nextPoint = points[index];
    }

}
