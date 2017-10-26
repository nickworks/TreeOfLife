using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTrigger : MonoBehaviour {

   
    /// <summary>
    /// this method is called when a collision with a 2d collider is detected
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {           
            GetComponentInParent<EnemyDasher>().inRange = true;
            GetComponentInParent<EnemyDasher>().canBeThrown = true;
            GetComponentInParent<EnemyDasher>().isHit = true;

        }
    }
    /// <summary>
    /// this method runs every frame that the collision with the 2d collider is still happening
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {           
            GetComponentInParent<EnemyDasher>().targetPos = collision.transform.position;
        }
    }
    /// <summary>
    /// this method runs when the collision is no longer happening with the 2d collider
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {           
            GetComponentInParent<EnemyDasher>().inRange = false;
        }

    }
}
