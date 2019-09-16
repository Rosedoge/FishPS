﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class TwistedUlt : Ability
{
    [SerializeField] Image ultButton;
    [SerializeField] GameObject ultCamObj;
    [SerializeField] bool isUlt;
    MouseLook playerLook;
    MouseLook playerCameraLook;
    private void Start()
    {
       
    }
    public override bool IsUltimate
    {
        get => isUlt;
        set => throw new System.NotImplementedException();
    }

    bool ready;
    public override bool UltimateReady
    {
        get => ready;
        set => ready = value;
    }
    public override bool Toggled
    {
        get => throw new System.NotImplementedException();
        set => throw new System.NotImplementedException();
    }

    public override float AdditionRate
    {
        get => 2f;
        set => throw new System.NotImplementedException();
    }

    float curCharge = 0;
    public override float UltCharge
    {
        get => 50f;
        set => throw new System.NotImplementedException();
    }
    public override float CurUltCharge
    {
        get => curCharge;
        set => throw new System.NotImplementedException();
    }

    float myCd = 15f;
    public override float Cooldown
    {
        get => myCd;
        set => myCd = value;
    }

    public override void Activate() // Generates CS0506.
    {
        Cooldown = 15f;
        curCharge = 0;
        UltimateReady = false;
        CardsHolder.SetActive(true);// = true;
        isActiveUlt = true;
        transform.GetChild(0).gameObject.SetActive(false);
        ultCamObj.SetActive(true);
        GetComponent<FirstPersonController>().m_MouseLook.enabled = false;
    }


    public override void Deactivate() // Generates CS0506.
    {
        Debug.Log("Enabled");
        GetComponent<FirstPersonController>().m_MouseLook.enabled = true;
    }

    bool isActiveUlt = false;
    [SerializeField] GameObject CardsHolder;
    private void Update()
    {


        if (curCharge < UltCharge)
        {
            curCharge += 5f;
        }
        else
        {
            UltimateReady = true;
        }
        ultButton.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - (curCharge / UltCharge);
        // Debug.Log("Charge is " + curCharge);
        if (isActiveUlt)
        {
            CardsHolder.transform.RotateAround(transform.position, transform.up, Time.deltaTime * 270f);
        }

    }
    public override void AddCharge() // Generates CS0506.
    {
        curCharge += AdditionRate;
    }
}
