using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSetUp1Page : MonoBehaviour
{
    [SerializeField]
    private GameObject gameManagerObject;
    private GameManager gameManager;

    [SerializeField]
    private GameObject errorText;

    [SerializeField]
    private GameObject p1NameInputObject;
    private InputField p1NameInput;

    [SerializeField]
    private GameObject p2NameInputObject;
    private InputField p2NameInput;

    [SerializeField]
    private Button gridSizeDropDown;
    [SerializeField]
    private Text gridDropDownText;
    [SerializeField]
    private GameObject numOfShotsInputObject;
    private InputField numOfShotsInput;
    [SerializeField]
    private GameObject btnContinueObject;
    private Button btnContinue;

    [SerializeField]
    private GameObject LeaveGamePanel;

    [SerializeField]
    private GameObject helpPanel;

    private void Start()
    {
        gameManager = gameManagerObject.GetComponent<GameManager>();
        p1NameInput = p1NameInputObject.GetComponent<InputField>();
        p2NameInput = p2NameInputObject.GetComponent<InputField>();
        numOfShotsInput = numOfShotsInputObject.GetComponent<InputField>();
        btnContinue = btnContinueObject.GetComponent<Button>();
    }
    private void OnEnable()
    {
        GameManager.BluetoothPlugin.RestrictReceive(true);
        LeaveGamePanel.SetActive(false);
        helpPanel.SetActive(false);
        p1NameInput.text = "";
        p2NameInput.text = "";
        numOfShotsInput.text = "";
    }
    public void Continue()
    {
        if (gameManager.Settings.Player1 == "")
        {
            DisplayError("Please Enter A Name For Player 1");
            return;
        }

        if (gameManager.Settings.Player2 == "")
        {
            DisplayError("Please Enter A Name For Player 2");
            return;
        }

        if (gameManager.Settings.YourGrid == null)
        {
            DisplayError("Please Select A Grid Size");
            return;
        }

        if (gameManager.Settings.NumOfShots < 1 || gameManager.Settings.NumOfShots > 10)
        {
            DisplayError("Please Enter Valid Number Of Shots");
            return;
        }

        gameManager.OpenPage("SinSetUp2");
    }

    public void P1NameChange()
    {
        gameManager.Settings.Player1 = p1NameInput.text;
    }
    public void P2NameChange()
    {
        gameManager.Settings.Player2 = p2NameInput.text;
    }

    public void DropDownOptionChange(int value)
    {
        gameManager.Settings.SizeOfGrid = value;
    }

    public void NumOfShotsChange()
    {
        try
        {
            int temp = 0;
            try
            {
                if (!(numOfShotsInput.text == ""))
                {
                    temp = int.Parse(numOfShotsInput.text);
                }
            }

            catch (System.Exception)
            {
                DisplayError("Number Of Shots Must Be A Number");
                numOfShotsInput.text = "";
                return;
            }
            if (temp > 10)
            {
                DisplayError("Max Number Of Shots Is " + gameManager.Settings.MaxNumOfShots.ToString());
                numOfShotsInput.text = "";
                return;
            }
            if (temp < 1)
            {
                numOfShotsInput.text = "";
                return;
            }

            gameManager.Settings.NumOfShots = temp;
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Updating Number Of Shots";
        }
    }


    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveGamePanel.SetActive(true);
            }
        }
    }
    public void LeaveYes()
    {
        gameManager.Settings = new GameSettings();
        gameManager.OpenPage("MenuPage", false);
    }

    public void LeaveNo()
    {
        LeaveGamePanel.SetActive(false);
    }

    private void DisplayError(string _message)
    {
        errorText.SetActive(true);
        errorText.GetComponentInChildren<Text>().text = _message;
        Invoke("DisableError", 4);
    }

    void DisableError()
    {
        errorText.GetComponentInChildren<Text>().text = "";
        errorText.SetActive(false);
    }

    public void BtnHelpOnClick()
    {
        helpPanel.SetActive(true);
    }

    public void BtnHelpOkOnClick()
    {
        helpPanel.SetActive(false);
    }
}
