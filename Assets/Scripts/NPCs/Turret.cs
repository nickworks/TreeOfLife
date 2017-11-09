using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    #region variables
    /// <summary>
    /// A reference to the object this turret is going to shoot
    /// </summary>
    [Tooltip("The projectile object this turret is going to shoot.")]
    public GameObject projectile;
    /// <summary>
    /// How long (in seconds) to delay between shots.  Will be calculated
    /// </summary>
    private float fireDelay;
    /// <summary>
    /// Rate of fire, in Rounds per Minute, this turret should have.  Used to calculate fireDelay.
    /// </summary>
    [Tooltip("The rate of fire of this Turret, as Rounds Per Minute.  Higher numbers mean faster rate of fire.")]
    public float RPM = 10;
    /// <summary>
    /// The timer that controls when the cannon is eligable to fire again.
    /// </summary>
    private float fireTimer;
    /// <summary>
    /// How quickly the turret's barrel can rotate around the body. Percentage decimals used (0-1).
    /// </summary>
    [Tooltip("How Quickly the turret's Barrel can rotate around it's body.")]
    [Range(0, 1)]public float turretSpeed = .5F;
    /// <summary>
    /// The point in space, represented by an objects anchor point, that will emit the projectile
    /// </summary>
    [SerializeField]
    [Tooltip("An Object whose anchor point will be used as the origin for projectile emission.  Make sure the emitter is pointed in the 'forward' vector of the barrel!")]
    private Transform shootFrom;
    /// <summary>
    /// The Game Object that represents the base of the barrel of the turret
    /// </summary>
    [SerializeField]
    [Tooltip("The base of the barrel of the turret.  Make sure the barrel is pointed in the 'forward' vector of the base!")]
    private Transform rotateAround;
    #endregion
    #region intialization
    /// <summary>
    /// Called when this script is intialized.
    /// </summary>
    void Start ()
    {
        CalculateFireDelay();
        GetComponent<Collider>().isTrigger = true;//make sure the collider is a trigger so it can detect players
	}
    /// <summary>
    /// Called when values are changed in the editor. Updates fire delay and caps certain values
    /// </summary>
    private void OnValidate()
    {
        CalculateFireDelay();//Make sure to recalculate fire rate if RPM gets changed.
    }
    /// <summary>
    /// Calculates the time in between each shot
    /// </summary>
    private void CalculateFireDelay()
    {
        fireDelay = 60f / RPM;//use of RPM allows for higher numbers in editor to equate to faster shooting.
    }
    #endregion
    #region everyFrame
    /// <summary>
    /// Called once per frame.  Updates fire timer.
    /// </summary>
    void Update ()
    {
        fireTimer -= Time.deltaTime;
	}
    private void OnTriggerStay( Collider collision )
    {
        if(collision.tag == "Player" )//only target players
        {
            /*THE 2D IMPLIMENTATION JUST IN CASE
            //figure out where the barrel should be pointing
            Vector3 direction = collision.transform.position - rotateAround.position;
            float angle = Vector3.SignedAngle(Vector3.up, direction, Vector3.forward);
            //calculate a slerp towards the intended rotation for smoothness
            Quaternion slerpChange = Quaternion.Slerp(rotateAround.rotation, Quaternion.Euler(0, 0, angle), turretSpeed);

            rotateAround.rotation = slerpChange;//apply the slerped rotation to the barrel
            */

            //Aim the Barrel
            Vector3 relativePos = collision.transform.position - rotateAround.transform.position;
            Quaternion rot = Quaternion.LookRotation(relativePos, Vector3.up);
            Quaternion slerpRot = Quaternion.Slerp(rotateAround.rotation, rot, turretSpeed);
            rotateAround.rotation = slerpRot;//slerp for smoothness

            //shoot things
            if( fireTimer <= 0 )
            {
                fireTimer = fireDelay;//reset fire timer
                GameObject b = Instantiate(projectile, shootFrom.position, Quaternion.identity);//make a bullet
                b.transform.forward = rotateAround.forward;//bullet's forward momentum is in the direction the barrel points
            }
        }
    }
#endregion

}
