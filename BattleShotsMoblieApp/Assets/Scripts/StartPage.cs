﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPage : MonoBehaviour
{
    [SerializeField]
    private GameObject GameManagerObject;
    private GameManager gameManager;

    [SerializeField]
    private GameObject ClickToContinueButtonObject;

    [SerializeField]
    private GameObject ExitPanel;

    private void Start()
    {        
        gameManager = GameManagerObject.GetComponent<GameManager>();        
    }

    private void Awake()
    {
        //ClickToContinueButtonObject.GetComponent<Button>().interactable = false;

        if (GameManager.BluetoothPlugin != null)
        {
            CheckBluetoothCompatable();
        }
        else
        {
            Invoke("CheckBluetoothCompatable", 0.2f);
            Invoke("PermissionChecks", 0.2f);
        }
    }

    void PermissionChecks()
    {
        GameManager.BluetoothPlugin.PermissionChecks();
    }
    void CheckBluetoothCompatable()
    {
        try
        {
            string btCheck = GameManager.BluetoothPlugin.BtCheck();
            if (btCheck != "0")            
            {
                //ClickToContinueButtonObject.GetComponent<Button>().interactable = true;
                string btCheckIsOn = GameManager.BluetoothPlugin.BtCheckIsOn();
                if (btCheckIsOn == "1")
                {
                    GameManager.BluetoothPlugin.ReceivePair();
                }
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ExitPanel.activeSelf == false)
                {
                    ExitPanel.SetActive(true);
                }
            }
        }
    }
    public void StartGame()
    {
        try
        {
            Button btn = ClickToContinueButtonObject.GetComponent<Button>();
            btn.interactable = false;
            string btCheckIsOn = GameManager.BluetoothPlugin.BtCheckIsOn();
            if (btCheckIsOn == "1")
            {
                string IsReceiving = GameManager.BluetoothPlugin.GetIsReceiving();
                if (IsReceiving == "0")
                {
                    GameManager.BluetoothPlugin.ReceivePair();
                }
                btn.interactable = true;
                gameManager.OpenPage("MenuPage", true);
                Invoke("ButtonInteraction", 3);
            }
            else
            {
                Invoke("SecondCheck", 1);
            }
        }
        catch(Exception ex)
        {
        }
    }

    void SecondCheck()
    {
        try
        {
            Button btn = ClickToContinueButtonObject.GetComponent<Button>();
            btn.interactable = false;
            string btCheckIsOn = GameManager.BluetoothPlugin.BtCheckIsOn();
            if (btCheckIsOn == "1")
            {
                string IsReceiving = GameManager.BluetoothPlugin.GetIsReceiving();
                if (IsReceiving == "0")
                {
                    GameManager.BluetoothPlugin.ReceivePair();
                }
                btn.interactable = true;
                gameManager.OpenPage("ConnectionPage", true);
                Invoke("ButtonInteraction", 3);
            }
            else
            {
                Invoke("StartGame", 1);
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void CancelExit()
    {
        ExitPanel.SetActive(false);
    }
}
