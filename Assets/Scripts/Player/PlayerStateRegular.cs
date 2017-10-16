using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateRegular : PlayerState {

    override public PlayerState Update(PlayerController player)
    {
        base.Update(player);

        bool jumpActive = Jump(player.jumpVelocity);
        ApplyGravity(jumpActive ? 1 : 2f);
        HandleInput();
        DoCollisions();

        return null;
    }
    override public void OnEnter(PlayerController player)
    {
        
    }
    override public void OnExit(PlayerController player)
    {

    }
    /// <summary>
    /// This method uses input to manipulate this object's physics.
    /// </summary>
    private void HandleInput()
    {
        float axisH = Input.GetAxisRaw("Horizontal");

        if (axisH == 0)
        {
            DecelerateX(player.walkAcceleration);
        }
        else
        {
            bool movingLeft = (player.velocity.x <= 0);
            bool acceleratingLeft = (axisH <= 0);
            float scaleAcceleration = (movingLeft != acceleratingLeft) ? 5 : 1; // if the player pushes the opposite direction from how they're moving, the player turns around quicker!

            AccelerateX(axisH * player.walkAcceleration * scaleAcceleration);
        }
    }

}
