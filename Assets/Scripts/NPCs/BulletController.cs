using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    /// <summary>
    /// How fast the bullet moves.
    /// </summary>
    public float speed = 10f;
    /// <summary>
    /// How long the bullet is alive.
    /// </summary>
    public float lifeSpan = 4f;

	/// <summary>
    /// Called once per frame.
    /// </summary>
	void Update ()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        //the forward vector is set when the shot is fired, bullets maintain a consant velocity unaffected by gravity for predictable platforming

        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0 )
        {
            Destroy(gameObject);
        }
	}

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if(collision.tag == "Player" )
        {
            //TODO: DAMAGE PLAYER
        }
        //TODO: clean up tags/layers
        //Destroy if colliding with anything other than another enemy or a volume, add in anything else not to collide with here.
        if (collision.tag != "Enemy" && collision.tag != "Volume" ) Destroy(gameObject);

    }
}
