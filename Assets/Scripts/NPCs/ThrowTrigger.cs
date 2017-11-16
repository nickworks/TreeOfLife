using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTrigger : MonoBehaviour {   
    // FIX ME - needs to check for 0 velocity to reset canBeThrown
    /// <summary>
    /// this method runs when collision with the 2d collider is detected 
    /// </summary>
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {           
            GetComponentInParent<EnemyDasher>().inRange = true;
            GetComponentInParent<EnemyDasher>().canBeThrown = true;
            
        }
    }
    /// <summary>
    /// this method runs every frame that the collision with the 2d collider is still happening
    /// </summary>
    private void OnTriggerStay(Collider collision)
    {
        if (collision.tag == "Player")
        {           
           // GetComponentInParent<EnemyDasher>().targetPos = collision.transform.position;
        }
    }
    /// <summary>
    /// this method runs when the collision is no longer happening with the 2d collider
    /// </summary>
    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {           
            GetComponentInParent<EnemyDasher>().inRange = false;
            GetComponentInParent<EnemyDasher>().canBeThrown = false;

        }

    }
   
}



