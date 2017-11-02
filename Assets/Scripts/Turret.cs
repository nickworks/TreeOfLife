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
    /// How quickly the turret's barrel can rotate around the body.
    /// </summary>
    [Tooltip("How Quickly the turret's Barrel can rotate around it's body.  Numbers from 0-1.")]
    public float turretSpeed = .5F;
    /// <summary>
    /// The point in space, represented by an objects anchor point, that will emit the projectile
    /// </summary>
    [SerializeField]
    [Tooltip("An Object whose anchor point will be used as the origin for projectile emission.")]
    private Transform shootFrom;
    /// <summary>
    /// The Game Object that represents the base of the barrel of the turret
    /// </summary>
    [SerializeField]
    [Tooltip("The base of the barrel of the turret.")]
    private Transform barrel;

    #endregion
    #region intialization

    /// <summary>
    /// Called when this script is intialized.
    /// </summary>
    void Start ()
    {
        CalculateFireDelay();
	}
    /// <summary>
    /// Called when values are changed in the editor.   updates fire delay and caps certain values
    /// </summary>
    private void OnValidate()
    {
        CalculateFireDelay();//Make sure to recalculate fire rate if RPM gets changed.
        if( turretSpeed < 0 ) turretSpeed = 0;
        if( turretSpeed > 1 ) turretSpeed = 1;
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
    /// Called once per frame.
    /// </summary>
    void Update ()
    {
        fireTimer -= Time.deltaTime;
	}
    /// <summary>
    /// Called every frame that an object w/ a RigidBody is in this object's collision hull(Requires collision hull to be registered as a trigger volume).
    /// </summary>
    /// <param name="collision">The object that collided with this trigger volume.</param>
    private void OnTriggerStay2D( Collider2D collision )
    {
        if(collision.tag == "Player" )//only target players
        {
            //figure out where the barrel should be pointing
            Vector3 direction = collision.transform.position - barrel.position;
            float angle = Vector3.SignedAngle(Vector3.up, direction, Vector3.forward);
            //calculate a slerp towards the intended rotation for smoothness
            Quaternion slerpChange = Quaternion.Slerp(barrel.rotation, Quaternion.Euler(0, 0, angle), turretSpeed);

            barrel.rotation = slerpChange;//apply the slerped rotation to the barrel

            //shoot things
            if( fireTimer <= 0 )
            {
                fireTimer = fireDelay;//reset fire timer
                GameObject b = Instantiate(projectile, shootFrom.position, Quaternion.identity);//make a bullet
                b.transform.forward = barrel.up;//bullet's forward momentum is in the direction the barrel points
            }
        }
    }

#endregion

}
