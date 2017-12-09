
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
        /// <summary>
        /// A reference to the PawnAABB component on this object.
        /// </summary>
        public float swingStrength = 5;
        /// </summary>
        /// The strength of the swing.
        /// <summary>
        public Rigidbody rigidBody;
        public Transform ropeTarget = null;
        public PawnAABB3D pawn { get; private set; }

        /// <summary>
        /// This int stores what state the player is currently in.
        /// </summary>
        public int currentState = 1;

        /// <summary>
        /// These constants represent what state the player is in, used only by currentState (above).
        /// </summary>
        public const int STATE_REGULAR = 1;
        public const int STATE_CLIMBING = 2;
        public const int STATE_SWINGING = 3;
        public const int STATE_BUGGED = 4;

        /// <summary>
        ///  //A timer for the bugged status effect
        /// </summary>
        public float bugTimer = 3;
        /// <summary>
        ///A boolean to control when the timer is started
        /// </summary>
        public bool startTimer = false;

        public Vector3 worldSpace;

        //public SpawnTriggerMover sTM = new SpawnTriggerMover();
        SpawnPointMover sTM;

        /// <summary>
        /// Reference to the SpawnPoint object in the scene
        /// </summary>
        GameObject spawnRef;

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
            spawnRef = GameObject.Find("SpawnPoint");
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
        void Update()
        {
            //If start timer is true
            if (startTimer)
            {
                //we subtract the bug timer from time.deltatime
                bugTimer -= Time.deltaTime;
            }
            
            if (playerState == null)
            {
                playerState = new PlayerStateRegular();
                currentState = STATE_REGULAR;
            }

            PlayerState nextState = playerState.Update(this);
            if (nextState != null)
            {
                playerState.OnExit(this);
                playerState = nextState;
                playerState.OnEnter(this);
            }

            //If the player hits the respawn button
            if (Input.GetButton("RightTrigger") && Input.GetButtonDown("Respawn"))
            {
                FindObjectOfType<SceneDictionary>().RestartLevel();
            }

            if (Input.GetButton("RightTrigger") && Input.GetButtonDown("LeftTrigger"))
            {
                gameObject.SetActive(false);
                velocity = Vector3.zero;
                transform.localPosition = spawnRef.transform.localPosition;
                GetComponent<AlignWithPath>().currentNode = spawnRef.GetComponent<SpawnLocation>().spawnNode;
                gameObject.SetActive(true);
            }
            if(Input.GetButton("RightTrigger") && Input.GetButtonDown("Circle"))
            {
                ImageEffect.active = !ImageEffect.active;
            }

            if (currentState == STATE_CLIMBING && Input.GetButtonDown("Jump"))
            {
                playerState = new PlayerStateRegular();
                currentState = STATE_REGULAR;
            }

            //If bug timer is less than or equal to zero
            if (bugTimer <= 0)
            {
                //we set the bug timer equal to 3
                bugTimer = 3;
                //we set startTimer to false so it doesn't keep subtracting
                startTimer = false;
                //we set player state to PlayerStateRegular
                playerState = new PlayerStateRegular();
                //We set current state to STATE_REGULAR
                currentState = STATE_REGULAR;
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
                        currentState = STATE_CLIMBING;
                        break;
                    case "Rope":
                        playerState = new PlayerStateClimbing("Rope"); // ID 2 = Rope
                        currentState = STATE_CLIMBING;
                        break;
                   
                }
            }//End of Grab if statement
            //These are switch statements for just hitting other colliders
            switch (other.tag)
            {
                //For case bugging collider
                case "BuggingCollider":
                    //WE set the bug timer to 3
                    bugTimer = 3;
                    //We start the timer 
                    startTimer = true;
                    //we set playerState to PlayerStateBugged
                    playerState = new PlayerStateBugged();
                    //We set currentState to state bugged
                    currentState = STATE_BUGGED;
                    //WE break out of the case
                    break;
            }
        }

        /// <summary>
        /// This message is called by the physics engine when the player enters a trigger volume.
        /// </summary>
        /// <param name="other">The trigger volume of the other object.</param>
        void OnTriggerEnter(Collider other)
        {
            // allows the player to attach itself to the raft and passes in a reference to the player
            switch (other.gameObject.tag)
            {
                case "Raft":
                    other.transform.parent.gameObject.GetComponent<Raft>().Attach(this);
                    break;
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
                case "Raft":
                    other.transform.parent.gameObject.GetComponent<Raft>().Detach();
                    break;
                case "StickyWeb":
                case "Rope":
                case "BuggingCollider":
                    playerState = new PlayerStateRegular();
                    currentState = STATE_REGULAR;
                    break;
                
                    
            }
        }
    }

}
