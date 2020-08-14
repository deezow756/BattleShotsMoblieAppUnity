using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupPage1 : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject gameManagerObject;
    private GameManager gameManager;

    [SerializeField]
    private GameObject errorText;

    [SerializeField]
    private GameObject nameInputObject;
    private InputField nameInput;

    [SerializeField]
    private GameObject EditorSection;
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
    private GameObject ReadOnlySection;
    [SerializeField]
    private Text txtGridSize;
    [SerializeField]
    private Text txtNumOfShots;

    [SerializeField]
    private GameObject LeaveGamePanel;

    [SerializeField]
    public GameObject reconnectPanel;
    [SerializeField]
    public GameObject reconnectScreen;
    [SerializeField]
    private GameObject reconnectBtnCancel;

    [SerializeField]
    private GameObject helpPanel;

    #endregion
    private void Start()
    {
        gameManager = gameManagerObject.GetComponent<GameManager>();
        nameInput = nameInputObject.GetComponent<InputField>();
        numOfShotsInput = numOfShotsInputObject.GetComponent<InputField>();
        btnContinue = btnContinueObject.GetComponent<Button>();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                LeaveGamePanel.SetActive(true);
            }

            if (gameManager.Settings.Master)
            {
                if (!EditorSection.activeSelf)
                {
                    EditorSection.SetActive(true);
                }
            }
            else
            {
                if (!ReadOnlySection.activeSelf)
                {
                    ReadOnlySection.SetActive(true);
                }
            }
        }
    }

    public void LeaveYes()
    {
        gameManager.Settings.ResetSettings();
        GameManager.BluetoothPlugin.Reset();
        GameManager.BluetoothPlugin.Disconnect();
        gameManager.OpenPage("ConnectionPage", false);
    }

    public void LeaveNo()
    {
        LeaveGamePanel.SetActive(false);
    }

    private void OnEnable()
    {
        LeaveGamePanel.SetActive(false);
        EditorSection.SetActive(false);
        ReadOnlySection.SetActive(false);
        helpPanel.SetActive(false);
        reconnectPanel.SetActive(false);
    }

    public void Continue()
    {
        try
        {
            if (gameManager.Settings.YourName == "")
            {
                DisplayError("Please Enter A Name");
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

            GameManager.BluetoothPlugin.SendData("continue");
            Invoke("NextPage", 0.5f);
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Continue";
        }
    }

    public void NextPage()
    {
        try
        {
            gameManager.OpenPage("SetupPage2", true);
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Next Page";
        }
    }

    public void NameChange()
    {
        try
        {
            gameManager.Settings.YourName = nameInput.text;

            GameManager.BluetoothPlugin.SendData("name," + nameInput.text);
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Updating Name";
        }
    }

    public void DropDownOptionChange(int value)
    {
        try
        {
            gameManager.Settings.SizeOfGrid = value;
            if (gameManager.Settings.Master)
            {
                GameManager.BluetoothPlugin.SendData("grid," + value.ToString());
            }
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Updating GridSize";
        }
    }

    public void ReceiveGridSize(string gridSize)
    {
        txtGridSize.text = gridSize;
    }

    public void ReceiveNumOfShots(string _numOfShots)
    {
        txtNumOfShots.text = _numOfShots;
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
                DisplayError("Number Of Shots Must Be More Than 0");
                numOfShotsInput.text = "";
                return;
            }

            gameManager.Settings.NumOfShots = temp;
            if (gameManager.Settings.Master)
            {
                GameManager.BluetoothPlugin.SendData("shots," + temp.ToString());
            }
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Updating Number Of Shots";
        }
    }

    private void DisplayError(string _message)
    {
        errorText.SetActive(true);
        errorText.GetComponentInChildren<Text>().text = _message;
        Animator animator = errorText.GetComponent<Animator>();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Invoke("DisableError", stateInfo.length);
    }

    public void BtnHelpOnClick()
    {
        helpPanel.SetActive(true);
    }

    public void BtnHelpOkOnClick()
    {
        helpPanel.SetActive(false);
    }

    public void BtnRetry()
    {
        if(gameManager.StartReconnectionStatus)
        {
            GameManager.BluetoothPlugin.ReconnectSend();
            reconnectScreen.SetActive(true);
            reconnectBtnCancel.SetActive(false);
            reconnectPanel.SetActive(false);
        }
        else
        {
            GameManager.BluetoothPlugin.ReconnectReceive();
            reconnectScreen.SetActive(true);
            reconnectBtnCancel.SetActive(true);
            reconnectPanel.SetActive(false);
        }
    }

    public void BtnExit()
    {
        gameManager.CancelReconnectedCallBack("1");
    }

    public void BtnCancel()
    {
        GameManager.BluetoothPlugin.ReconnectCancel();
        reconnectScreen.SetActive(false);
        reconnectBtnCancel.SetActive(false);
        reconnectPanel.SetActive(true);        
    }
}
