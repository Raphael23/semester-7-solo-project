﻿using Assets.Scripts.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : InteractableObject
{
    public delegate void ButtonPressed(ButtonInteractable button);
    public static event ButtonPressed OnButtonPressed;

    public delegate void ButtonUnPressed(ButtonInteractable button);
    public static event ButtonUnPressed OnButtonUnPressed;

    [SerializeField]
    private GameObject door;

    [SerializeField]
    private float pressDuration;

    private Transform closedRotation;
    private float openRotation = 90f;

    bool isDoorMoving = false;

    List<ButtonInteractable> pressedButtons = new List<ButtonInteractable>();

    public void Start()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        closedRotation = door.transform;
    }

    public override void Interact()
    {
        if (pressedButtons.Contains(this)) { return; }
        OnButtonPressed(this);
        StartCoroutine(CountDownPress());
    }

    public void TryOpen(ButtonInteractable button)
    {
        pressedButtons.Add(button);       

        if (pressedButtons.Count == 2) 
        {
            StartCoroutine(Open(closedRotation, openRotation, 1.0f));
        }
    }

    public void RemoveButton(ButtonInteractable button)
    {
        pressedButtons.Remove(button);
    }

    private IEnumerator CountDownPress()
    {
        float counter = pressDuration;

        GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        while (counter > 0f)
        {
            counter -= Time.deltaTime;
            yield return null;
        }

        GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        OnButtonUnPressed(this);
    }

    IEnumerator Open(Transform fromPosition, float toPosition, float duration)
    {
        //Make sure there is only one instance of this function running
        if (isDoorMoving)
        {
            yield break; ///exit if this is still running
        }
        isDoorMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        Transform startPos = fromPosition;
        door.transform.rotation = Quaternion.Euler(0, toPosition, 0);
        while (counter < duration)
        {
            counter += Time.deltaTime;
            
            yield return null;
        }

        isDoorMoving = false;
        StartCoroutine(Close(openRotation, closedRotation, 1f));
    }

    IEnumerator Close(float fromPosition, Transform toPosition, float duration)
    {
        //Make sure there is only one instance of this function running
        if (isDoorMoving)
        {
            yield break; ///exit if this is still running
        }

        GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        
        isDoorMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        //Vector3 startPos = fromPosition;
        while (counter < duration)
        {
            counter += Time.deltaTime;
           // door.transform.position = Vector3.Lerp(startPos, toPosition, counter / duration);
            yield return null;
        }
        door.transform.rotation = Quaternion.Euler(0, 0, 0);
        isDoorMoving = false;
    }

    private void OnEnable()
    {
        OnButtonPressed += TryOpen;
        OnButtonUnPressed += RemoveButton;
    }
}
