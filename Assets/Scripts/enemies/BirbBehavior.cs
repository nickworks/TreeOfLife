using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
/// <summary>
/// This class is placed on the Birb Enemy prefab, determines the birb's behavior.
/// </summary>
public class BirbBehavior : MonoBehaviour {

    /// <summary>
    /// The speed at which the Birb impulses towards the player.
    /// </summary>
    public float thrust;
    /// <summary>
    /// A short cooldown that makes the birb wait before divebombing, used to give the player some time to react.
    /// </summary>
    private float waitCooldown = .5f;
    /// <summary>
    /// The time spent impulsing towards the player
    /// </summary>
    private float howLongToApplyForce = .7f;
    /// <summary>
    /// Is true when the birb has been destroyed
    /// </summary>
    public bool isDead = false;
    /// <summary>
    /// Storing the birb's rigidbody in a variable
    /// </summary>
    public Rigidbody2D birb;
    /// <summary>
    /// A small random number that makes the birb's paths slightly varied.
    /// </summary>
    private Vector3 randomOffset;
    /// <summary>
    /// This is used to give the birb's path an arc in a 'U' shape
    /// </summary>
    private Vector3 gravity = new Vector3(0, 10, 0);
    /// <summary>
    /// The birb's current speed.
    /// </summary>
    private Vector3 velocity = new Vector3();
    /// <summary>
    /// A reference to the player.
    /// </summary>
    private PlayerController player;

	void Start () {
        randomOffset = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

    }
	/// <summary>
    /// This message is called every tick.
    /// </summary>
	void Update () {
        waitCooldown -= Time.deltaTime;
        Vector3 target = player.transform.position + randomOffset;
        if (waitCooldown <= 0 && !isDead)
        {
            if (howLongToApplyForce > 0)
            {
                howLongToApplyForce -= Time.deltaTime;
                velocity += (target - transform.position).normalized * thrust * Time.deltaTime;
            }
            else
            {
                velocity += gravity * Time.deltaTime;
            }
            transform.position += velocity * Time.deltaTime;
            if (transform.position.y - target.y > 10)
            {
                Destroy(gameObject);
            }
        }
	}
    /// <summary>
    /// This is called when the player collides with the birb (The birb enemy is just a trigger object).
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Hurt the Player and knock them back
    }
}
