using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This operates in the playe rnames space
/// </summary>
namespace Player
{
    /// <summary>
    /// A public class for when the Player is harrased by flies
    /// </summary>
    public class PlayerStateBugged : PlayerState
    {

        
        /// <summary>
        /// This overrides the playerControllers update method
        /// </summary>
        /// <param name="player"> A reference to the player controller</param>
        /// <returns></returns>
         override public  PlayerState Update(PlayerController player)
        {
            //updates all of the stuff in the base playercontroller
            base.Update(player);        
            //Caculates the players jumpvelocity and sets jumpactive
            bool jumpActive = Jump(player.jumpVelocity);
            //Applies gravity but at a higher level because they are being harrased by bugs
            ApplyGravity(jumpActive ? 2 : 3f);
            //Handles player input
            HandleInput();
            //Does collisions
            DoCollisions();
            //Does collisions
            return this;
        }//End of update override

        /// <summary>
        /// Overrides the OnEnter method
        /// </summary>
        /// <param name="player"> A reference to the player controller</param>
        override public void OnEnter(PlayerController player)
        {
            //Sets the players jump time to a different amount
            player.jumpTime =4;
            //Sets the players jump height to a different amount
            player.jumpHeight = 3;
       
        }//End of onEnter override
        /// <summary>
        /// An empty method (for now) that overrides the players on Exit method
        /// </summary>
        /// <param name="player"> A reference to the playerController script</param>
        override public  void OnExit(PlayerController player)
        {
            
        }//End of onExit override
        /// <summary>
        /// A private method that handles input from the player
        /// </summary>
        private void HandleInput()
        {
            //Gets the players raw horizontal axis
            float axisH = Input.GetAxisRaw("Horizontal");
            //Most of this is similar to player state regular
           if(Input.GetButton("Jump"))
            {               
                Jump(player.jumpVelocity);
            }
           if(axisH == 0)
            {
                DecelerateX(player.walkAcceleration);
            }
            else
            {
                bool movingLeft = (player.velocity.x <= 0);
                bool acceleratingLeft = (axisH <= 0);
                //Except for here where we scale down the players acceleration so they move slower
                float scaleAcceleration = (movingLeft != acceleratingLeft) ? player.turnAroundMultiplier : .55f;
                AccelerateX(axisH * player.walkAcceleration * scaleAcceleration);
            }           
        }
        
    }//End of public class PlayerStateBugged
}//End of namespace Player

