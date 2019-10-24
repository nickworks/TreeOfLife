
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// This component turns a GameObject into a controllable avatar.
    /// </summary>
    [RequireComponent(typeof(PawnAABB3D))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {

        #region variables

        static public PlayerController main;
        private PlayerState playerState;
        /// <summary>
        /// The amount of time, in seconds, that it should take the player to reach the peak of their jump arc.
        /// </summary>
        [Tooltip("The amount of time, in seconds, that it should take the player to reach the peak of their jump arc.")]
        public float jumpTime = 0.75f;
        /// <summary>
        /// The height, in meters, of the player's jump arc.
        /// </summary>
        [Tooltip("The height, in meters, of the player's jump arc.")]
        public float jumpHeight = 3;
        /// <summary>s
        /// The horizontal acceleration to use when the player moves left or right.
        /// </summary>
        [Tooltip("The horizontal acceleration to use when the player moves left or right.")]
        public float walkAcceleration = 10;
        /// <summary>
        /// How much to scale the acceleration when the player's horizontal input is opposite of their velocity. Higher numbers make the player stop and turn around more quickly.
        /// </summary>
        [Tooltip("How much to scale the acceleration when the player's horizontal input is opposite of their velocity. Higher numbers make the player stop and turn around more quickly.")]
        public float turnAroundMultiplier = 10;
        /// <summary>
        /// The standard acceleration to use for gravity. This will be calculated from the jumpTime and jumpHeight fields.  The base value for gravity.
        /// </summary>
        public float gravityStandard { get; private set; }
        /// <summary>
        /// The acceleration to use for gravity.  Defaults to gravityStandard, but can be manipulated by force volumes.
        /// </summary>
        public float gravityTemporary { get; private set; }
        /// <summary>
        /// The direction vector in which Gravity will be applied.
        /// </summary>
        public Vector3 gravityDir { get; private set; }
        /// <summary>
        /// The takeoff speed to use as vertical velocity for the player's jump. This will be calculated from jumpTime and jumpHeight fields.
        /// </summary>
        public float jumpVelocity { get; private set; }
        /// <summary>
        /// The velocity of the player. This is used each frame for Euler physics integration.
        /// </summary>
        public Vector3 velocity = new Vector3();
        /// <summary>
        /// The maximum speed of the player, in meters-per-second.
        /// </summary>
        public float maxSpeed = 10;
        /// </summary>
        /// The strength of the swing.
        /// <summary>
        public Rigidbody rigidBody;
        public Transform ropeTarget = null;
        public PawnAABB3D pawn { get; private set; }


        #endregion
        #region Setup

        /// <summary>
        /// This initializes this component.
        /// </summary>
        void Start()
        {
            pawn = GetComponent<PawnAABB3D>();
            velocity = new Vector3();
            DeriveJumpValues();
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            main = this;
            
        }
        /// <summary>
        /// This is called automatically when the values change in the inspector.
        /// </summary>
        void OnValidate()
        {
            DeriveJumpValues();
        }
        /// <summary>
        /// This method calculates the gravity and jumpVelocity to use for jumping.
        /// </summary>
        void DeriveJumpValues()
        {
            gravityStandard = (jumpHeight * 2) / (jumpTime * jumpTime);
            jumpVelocity = gravityStandard * jumpTime;
            SetGravity();
        }
        #endregion

        /// <summary>
        /// This method is called each frame. 
        /// </summary>
        void Update() {
            DoStateMachine();
        }

        private void DoStateMachine() {
            if (playerState == null) {
                playerState = new PlayerStateRegular();
            }
            PlayerState nextState = playerState.Update(this);
            if (nextState != null) {
                playerState.OnExit(this);
                playerState = nextState;
                playerState.OnEnter(this);
            }
        }

        /// <summary>
        /// Used to set or reset gravity.  An empty set of parameters will reset gravity values to defaults.
        /// </summary>
        /// <param name ="gravityDirection">Which direction should gravity point?  Will be normalized.</param>
        /// <param name ="gravityForce">The power of the gravitational force</param>
        public void SetGravity(Vector3? gravityDirection = null, float? gravityForce = null)
        {
            //If gravity direction isn't specified, set it to the default "down"
            if (gravityDirection == null)
            {
                gravityDir = Vector3.down;
            }
            else//Otherwise we set gravity to the new direction, and normalize that value
            {
                Vector3 tempDirection = (Vector3)gravityDirection;//converts the Vector3? to a vector3
                gravityDir = tempDirection.normalized;
            }
            //If gravity force isn't specified, set it to the default value created at startup
            if (gravityForce == null)
            {
                gravityTemporary = gravityStandard;
            }
            else//Otherwise we set the force of gravity to the new value
            {
                gravityTemporary = (float)gravityForce;//Convert the float? to a normal float
            }
        }

        /// <summary>
        /// This method applies outside forces (from a force volume) to the object.
        /// </summary>
        /// <param name="forceForce">The power of the force to be applied to this pawn.</param>
        /// <param name="forceDir">The directional vector of the force.  Will be normalized.</param>
        public void ApplyForce(float forceForce, Vector3 forceDir)
        {
            velocity += forceForce * forceDir.normalized * Time.deltaTime;
        }

        /// <summary>
        /// This message is called by the physics engine while the player is in a trigger volume.
        /// </summary>
        /// <param name="other">The trigger volume of the other object.</param>
        private void OnTriggerStay(Collider other)
        {
            if (Input.GetButton("Grab"))
            {
                switch (other.tag)
                {
                    case "StickyWeb":
                        playerState = new PlayerStateClimbing("StickyWeb"); // ID 1 = StickyWeb
                        break;
                    case "Rope":
                        playerState = new PlayerStateClimbing("Rope"); // ID 2 = Rope
                        break;
                   
                }
            }
        }

        /// <summary>
        /// detects the end of collision with objects
        /// </summary>
        /// <param name="other"></param> the object the raft WAS colliding with
        void OnTriggerExit(Collider other)
        {
            // resets the raft's variables for next use
            switch (other.gameObject.tag)
            {
                case "StickyWeb":
                case "Rope":
                case "BuggingCollider":
                    playerState = new PlayerStateRegular();
                    break;    
            }
        }
    }
}
