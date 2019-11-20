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
    private GameObject gridSizeDropDownObject;
    private Dropdown gridSizeDropDown;

    [SerializeField]
    private GameObject numOfShotsInputObject;
    private InputField numOfShotsInput;
    

    [SerializeField]
    private GameObject btnContinueObject;
    private Button btnContinue;
    #endregion
    private void Start()
    {
        gameManager = gameManagerObject.GetComponent<GameManager>();
        nameInput = nameInputObject.GetComponent<InputField>();
        gridSizeDropDown = gridSizeDropDownObject.GetComponent<Dropdown>();
        numOfShotsInput = numOfShotsInputObject.GetComponent<InputField>();
        btnContinue = btnContinueObject.GetComponent<Button>();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.OpenPage("ConnectionPage", false);
            }
        }
    }

    private void OnEnable()
    {
        if(!gameManager.Settings.Master)
        {
            //gridSizeDropDown.interactable = false;
            //numOfShotsInput.interactable = false;
            //btnContinue.interactable = false;
        }
    }

    public void Continue()
    {
        if(gameManager.Settings.YourName == "")
        {
            DisplayError("Please Enter A Name");
            return;
        }

        if(gameManager.Settings.NumOfShots < 1 || gameManager.Settings.NumOfShots > 10)
        {
            DisplayError("Please Enter Valid Number Of Shots");
        }
    }

    public void NameChange()
    {
        gameManager.Settings.YourName = nameInput.text;

        GameManager.BluetoothPlugin.SendData(nameInput.text);
    }

    public void DropDownOptionChange()
    {
        gameManager.Settings.SizeOfGrid = int.Parse(gridSizeDropDown.itemText.text);
        gridSizeDropDown.template.gameObject.SetActive(false);
        if(gameManager.Settings.Master)
        {
            GameManager.BluetoothPlugin.SendData(gridSizeDropDown.itemText.text);
        }
    }

    public void ReceiveGridSize(string gridSize)
    {
        gridSizeDropDown.captionText.text = gridSize;
    }

    public void NumOfShotsChange()
    {
        int temp = 0;
        try
        {
            temp = int.Parse(numOfShotsInput.text);
        }
        catch (System.Exception)
        {
            DisplayError("Number Of Shots Must Be A Number");
            numOfShotsInput.text = "1";
            return;
        }
        if(temp > 10)
        {
            DisplayError("Max Number Of Shots Is " + gameManager.Settings.MaxNumOfShots.ToString());
            numOfShotsInput.text = "1";
            return;
        }
        if(temp < 1)
        {
            DisplayError("Number Of Shots Must Be More Than 0");
            numOfShotsInput.text = "1";
            return;
        }

        gameManager.Settings.NumOfShots = temp;
        if(gameManager.Settings.Master)
        {
            GameManager.BluetoothPlugin.SendData(temp.ToString());
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
    private void DisableError()
    {
        errorText.SetActive(false);
        errorText.GetComponentInChildren<Text>().text = "";
    }
}
