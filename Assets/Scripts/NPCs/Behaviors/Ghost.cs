using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

/// <summary>
/// This class controls how the ghost/ghost-like enemy moves and "attacks" the player
/// </summary>
public class Ghost : BehaviorNPC
{

    /// <summary>
    /// Is the player in range to start chasing?
    /// </summary>
    private bool isAggro = false;
    /// <summary>
    /// This is the player's position, where the ghost wants to go
    /// </summary>
    private Vector3 targetPos;
    /// <summary>
    /// This is the speed X the ghost will move
    /// </summary>
    private float xAcc;
    /// <summary>
    /// This is the speed Y the ghost will move
    /// </summary>
    private float yAcc;
    /// <summary>
    /// This is the ghosts current position, for convienence control
    /// </summary>
    private Vector3 currentPos;
    /// <summary>
    /// This is the offset added or subtracted from the targetPos to determine of the ghost needs to update on an axis
    /// </summary>
    private float offset = .05f;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        xAcc = 0;
        yAcc = 0;
    }

    /// <summary>
    /// Called once per frame
    /// </summary>
    void Update()
    {
        currentPos = transform.position;
        if (isAggro)
        {
            if(xAcc < 3) xAcc += Time.deltaTime;
            if(yAcc < 3) yAcc += Time.deltaTime;

            if (currentPos.x > targetPos.x + offset || currentPos.x < targetPos.x - offset)
            {
                if (targetPos.x > currentPos.x)
                {
                    //player to the right of ghost
                    currentPos.x += xAcc * Time.deltaTime;
                }
                else if (targetPos.x < currentPos.x)
                {
                    //player to the left of ghost
                    currentPos.x -= xAcc * Time.deltaTime;
                }
            }

            if(currentPos.y > targetPos.y +offset || currentPos.y < targetPos.y - offset)
            {
                if (targetPos.y > currentPos.y)
                {
                    //player above ghost
                    currentPos.y += yAcc * Time.deltaTime;
                }
                else if (targetPos.y < currentPos.y)
                {
                    //player below ghost
                    currentPos.y -= yAcc * Time.deltaTime;
                }
            }

            transform.position = currentPos;
        }
        else
        {
            xAcc = 0;
            yAcc = 0;
        }
    }

    public override void FindsPlayer(PlayerController player)
    {
        isAggro = true;
    }
    public override void PlayerNearby(PlayerController player)
    {
        targetPos = player.transform.position;
    }
    public override void LosesPlayer()
    {
        isAggro = false;
    }

    public override void IsStopped()
    {
       
    }
}
