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

        public bool isClimbing = false;

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

            if (isClimbing == true)
            {
                if (playerState != null) playerState = new PlayerStateClimbing();
                print(playerState);
            }
            if (isClimbing != true)
            {
                if (playerState != null) playerState = new PlayerStateRegular();
                print(playerState);
            }

            PlayerState nextState = playerState.Update(this);
            
           

            if (nextState != null)
            {
                playerState.OnExit(this);
                playerState = nextState;
                playerState.OnEnter(this);
            }
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.tag == "StickyWeb")
            {
                if (Input.GetButton("Grab"))
                {
                    isClimbing = true;
                }
                
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.tag == "StickyWeb")
            {
                isClimbing = false;
            }
        }

    }
}