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
    private GameObject ErrorTextObject;
    [SerializeField]
    private GameObject TapAnyWhereTextObject;

    [SerializeField]
    private GameObject ExitPanel;    

    private void Start()
    {        
        gameManager = GameManagerObject.GetComponent<GameManager>();        
    }

    private void Awake()
    {
        ClickToContinueButtonObject.GetComponent<Button>().interactable = false;

        if (GameManager.BluetoothPlugin != null)
        {
            CheckBluetoothCompatable();
        }
        else
        {
            Invoke("CheckBluetoothCompatable", 1);
            Invoke("PermissionChecks", 1);
        }
    }

    void PermissionChecks()
    {
        GameManager.BluetoothPlugin.PermissionChecks();
    }
    void CheckBluetoothCompatable()
    {
        string btCheck = GameManager.BluetoothPlugin.BtCheck();
        if (btCheck == "0")
        {
            ClickToContinueButtonObject.SetActive(false);
            TapAnyWhereTextObject.SetActive(false);
            ErrorTextObject.SetActive(true);
            ErrorTextObject.GetComponent<Text>().text = "Device Does Not Support Bluetooth";
        }
        else
        {
            ClickToContinueButtonObject.GetComponent<Button>().interactable = true;
            GameManager.BluetoothPlugin.ReceivePair();
        }
    }

    public void CheckStarted(string message)
    {
        ErrorTextObject.SetActive(true);
        ErrorTextObject.GetComponent<Text>().text = message;
    }

    private void OnEnable()
    {
        ErrorTextObject.SetActive(false);
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
        Button btn = ClickToContinueButtonObject.GetComponent<Button>();
        btn.interactable = false;
        string btCheckIsOn = GameManager.BluetoothPlugin.BtCheckIsOn();
        if (btCheckIsOn == "1")
        {
            gameManager.OpenPage("ConnectionPage", true);
            Invoke("ButtonInteraction", 5);
        }
        else
        {
            ErrorTextObject.SetActive(true);
            ErrorTextObject.GetComponent<Text>().text = "Bluetooth Turned On";

            Invoke("NextPage", 2);
        }
    }

    void NextPage()
    {
        Invoke("ButtonInteraction", 5);
        gameManager.OpenPage("ConnectionPage", true);
    }

    void ButtonInteraction()
    {
        ClickToContinueButtonObject.GetComponent<Button>().interactable = true ;
        ErrorTextObject.SetActive(false);
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
