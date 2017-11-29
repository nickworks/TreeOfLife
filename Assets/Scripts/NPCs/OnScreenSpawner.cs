using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class spawns objects when falls into the camera's view frustrum (it's on-screen).
/// </summary>
public class OnScreenSpawner : MonoBehaviour {

    /// <summary>
    /// A cooldown before another object will spawn.
    /// </summary>
    private float cooldownTimer = 0;
    /// <summary>
    /// Storing the main camera object
    /// </summary>
    private Camera cam;
    /// <summary>
    /// The spawner collider, used to determine if it's on the screen
    /// </summary>
    private Collider spawnCollider;
    /// <summary>
    /// Storing the prefab to spawn.
    /// </summary>
    public GameObject prefab;
    /// <summary>
    /// How long a spawner must wait before spawning another object.
    /// </summary>
    public float spawnCooldown = 1;

    /// <summary>
    /// Whether or not the collider is offscreen.
    /// </summary>
    private bool offScreen = true;
    /// <summary>
    /// This initializes our object.
    /// </summary>
	void Start () {
        cam = Camera.main;
        spawnCollider = GetComponent<Collider>();
	}
    /// <summary>
    /// This message runs every tick.
    /// </summary>
	void Update () {

        if(cooldownTimer >= 0) cooldownTimer -= Time.deltaTime;

        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (GeometryUtility.TestPlanesAABB(camPlanes, spawnCollider.bounds))
        {
            if (offScreen && cooldownTimer < 0)
            {
                Spawn();
            }
            offScreen = false;
        }
        else
        {
            offScreen = true;
        }
    }
    /// <summary>
    /// Spawns an object at the spawner location
    /// </summary>
    private void Spawn()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
        cooldownTimer = spawnCooldown;
    }
}
