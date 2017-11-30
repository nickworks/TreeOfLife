using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

/// <summary>
/// This class controls a proximity trigger volume for enemies. This component MUST be on a gameobject that is a
/// direct child of a gameobject with a sub-class component of BehaviourNPC.
/// </summary>
public class TriggerNPC : MonoBehaviour {

   


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //print("Chase!");
            GetComponentInParent<BehaviorNPC>().FindsPlayer(other.GetComponent<PlayerController>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //print("Chase!");
            GetComponentInParent<BehaviorNPC>().FindsPlayer(other.GetComponent<PlayerController>());
        }
    }//End of private void OnTriggerStay

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //print("Chase!");
            GetComponentInParent<BehaviorNPC>().FindsPlayer(other.GetComponent<PlayerController>());
        }
    }

    

    
}

        }else if (other.tag == "StickyWeb")
        {
            GetComponentInParent<BehaviorNPC>().IsStopped();
        }
    }

    private void OnTriggerStay(Collider other)
    {
            GetComponentInParent<BehaviorNPC>().PlayerNearby(other.GetComponent<PlayerController>());
        }
        else if (other.tag == "StickyWeb")
        {
            GetComponentInParent<BehaviorNPC>().IsStopped();
        }
    }//End of private void OnTriggerStay

    private void OnTriggerExit(Collider other)
    {
            GetComponentInParent<BehaviorNPC>().LosesPlayer();
        }
    }
