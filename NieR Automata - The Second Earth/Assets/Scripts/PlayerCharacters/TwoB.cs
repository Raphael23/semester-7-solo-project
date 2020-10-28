﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwoB : PlayerCharacterBase
{
    public Contaminated Contaminated;

    protected override void Awake()
    {
        base.Awake();

        // When the left stick is moved (mapped to MoveCharacter1), it will call performed.
        // performed will give a context (mv) which has a readable value of the stick's direction.
        // move is then set to the stick's direction.
        controls.TwobControls.Move.performed += MovePerformed;
        // When the stick is idle, canceled will be called setting move to Vector2.zero.
        controls.TwobControls.Move.canceled += MoveCanceled;

        controls.TwobControls.Interact.performed += InteractPerformed;
    }

    new protected void Update()
    {
        base.Update();

        if (Contaminated.Draining)
        {
            Contaminated.DrainSelfControl(Contaminated.DrainRate);
        }
    }

    protected override void InteractPerformed(InputAction.CallbackContext obj)
    {
        if (Contaminated.SelfControl > 0)
        {
            base.InteractPerformed(obj);
        }
    }

    protected override void Move()
    {
        if (Contaminated.SelfControl > 0)
        {  
            // Goes from 0.0 to 1.0 depending on current SelfControl and takes 20% of the value. 
            float finalTwitchMultiplier = 0.6f * Contaminated.SelfControlCurve.Evaluate((float)Contaminated.SelfControl / 100);

            // Adds the twitch value % randomly to the initial movement input.
            Vector2 twitchMovementDirection = AddRandomMovement(oldMovementInput, finalTwitchMultiplier);

            twitchMovementDirection.Normalize();
            // Determines the movement direction and speed in the world space. 
            Vector3 worldDirection = new Vector3(twitchMovementDirection.x * speed, 0, twitchMovementDirection.y * speed);
            //Look at direction
            transform.rotation = Quaternion.LookRotation(worldDirection);
            // Move
            transform.Translate(worldDirection * Time.deltaTime, Space.World);
            Debug.Log("Curve value:" + Contaminated.SelfControlCurve.Evaluate((float)Contaminated.SelfControl / 100));
            Debug.Log("Multiplier:" + finalTwitchMultiplier);
            Debug.Log("old:" + oldMovementInput);
            Debug.Log("new:" + twitchMovementDirection);
            Debug.Log("world:" + worldDirection);
        }
      
    }

    private Vector2 AddRandomMovement(Vector2 direction, float multiplier)
    {
        //TODO add variable that saves the last random result for both x and y. Base the new random value on the previous one if input direction is the same. 
        direction = direction * UnityEngine.Random.Range(1 - multiplier, 1 + multiplier);
        Vector2 randomDirection = new Vector2(direction.x * UnityEngine.Random.Range(1 - multiplier, 1 + multiplier), direction.y * UnityEngine.Random.Range(1 - multiplier, 1 + multiplier));
        return randomDirection;
    }
    public override void UpdateCharacterDialogueState(DialogueState state)
    {
        base.UpdateCharacterDialogueState(state);
        if (state != DialogueState.Disabled)
        {
            controls.TwobControls.Move.performed -= MovePerformed;
        }
        else if (state == DialogueState.Disabled)
        {
            controls.TwobControls.Move.performed += MovePerformed;
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        controls.TwobControls.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        controls.TwobControls.Disable();
    }

}
