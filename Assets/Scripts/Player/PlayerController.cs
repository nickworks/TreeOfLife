using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    /// <summary>
    /// This component turns a GameObject into a controllable avatar.
    /// </summary>
    [RequireComponent(typeof(PawnAABB))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerState playerState;

        

        /// <summary>
        /// The amount of time, in seconds, that it should take the player to reach the peak of their jump arc.
        /// </summary>
        public float jumpTime = 0.75f;
        /// <summary>
        /// The height, in meters, of the player's jump arc.
        /// </summary>
        public float jumpHeight = 3;
        /// <summary>s
        /// The horizontal acceleration to use when the player moves left or right.
        /// </summary>
        public float walkAcceleration = 10;
        /// <summary>
        /// How much to scale the acceleration when the player's horizontal input is opposite of their velocity. Higher numbers make the player stop and turn around more quickly.
        /// </summary>
        public float turnAroundMultiplier = 10;
        /// <summary>
        /// The acceleration to use for gravity. This will be calculated from the jumpTime and jumpHeight fields.
        /// </summary>
        public float gravity;
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


        //public SpawnTriggerMover sTM = new SpawnTriggerMover();
        SpawnPointMover sTM;

        /// <summary>
        /// A reference to the PawnAABB component on this object.
        /// </summary>
        public PawnAABB pawn { get; private set; }
        #region Setup
        /// <summary>
        /// This initializes this component.
        /// </summary>
        void Start()
        {
            //sTM =  GameObject.Find("SpawnTrigger").GetComponent<SpawnTriggerMover>();
            sTM = GameObject.Find("SpawnPoint").GetComponent<SpawnPointMover>();
            pawn = GetComponent<PawnAABB>();
            velocity = new Vector3();
            DeriveJumpValues();
            if (playerState == null) playerState = new PlayerStateRegular();
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
            gravity = (jumpHeight * 2) / (jumpTime * jumpTime);
            jumpVelocity = gravity * jumpTime;
        }
        #endregion
        /// <summary>
        /// This method is called each frame. 
        /// </summary>
        void Update()
        {


            
            PlayerState nextState = playerState.Update(this);
            
           

            if (nextState != null)
            {
                playerState.OnExit(this);
                playerState = nextState;
                playerState.OnEnter(this);
            }

            //If the player hits the respawn button
            if (Input.GetButton("Respawn"))
            {
                //We set their current transform to the STM's transform
              transform.position = sTM.transform.position;
                
            }
            
        }

        /// <summary>
        /// A on trigger stay 2D method to handle collision events
        /// </summary>
        /// <param name="other">A reference to the collider information passed into this</param>
        private void OnTriggerStay2D(Collider2D other)
        {
            //If the other object is a stickyweb
            if (other.gameObject.tag == "StickyWeb")
            {
                //And the player grabs the players state is set to climbing
                if (Input.GetButton("Grab"))
                {
                    //We set the playerstate to playerStateClimbing
                    playerState = new PlayerStateClimbing();

                }else if(Input.GetButtonUp("Jump")){// If the jump button is released
                    //Player state is set to regular
                  playerState = new PlayerStateRegular();
                }
                
            }
        }
        /// <summary>
        /// An on trigger exit 2D method to handle collision exit events
        /// </summary>
        /// <param name="other"> A reference to the collider information passed into thiss</param>
        private void OnTriggerExit2D(Collider2D other)
        {
            //If the other game object is a stickyweb
            if (other.gameObject.tag == "StickyWeb" )
            {
                //The player state is set to regular
                playerState = new PlayerStateRegular();
            }
        }

    }
}