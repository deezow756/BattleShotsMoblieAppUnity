using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    public GameObject GameManagerObject;
    GameManager gameManager;

    [SerializeField]
    private Button sinPhoneBtn;
    [SerializeField]
    private Button mulPhoneBtn;

    [SerializeField]
    private GameObject Canvas;

    [SerializeField]
    private GameObject helpPanel;

    [SerializeField]
    private GameObject errorText;

    private void Start()
    {
        gameManager = GameManagerObject.GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        if (GameManager.BluetoothPlugin.BtCheck() != "0")
        {
            GameManager.BluetoothPlugin.RestrictReceive(false);
            if (GameManager.BluetoothPlugin.GetIsReceiving() == "0")
            {
                GameManager.BluetoothPlugin.ReceivePair();
            }
        }

        helpPanel.SetActive(false);        
    }

    public void BtnSingleOnClick()
    {
        gameManager.OpenPage("SinSetUp1");        
    }
    public void BtnMultiOnClick()
    {
        if (GameManager.BluetoothPlugin.BtCheck() == "0")
        {
            DisplayError("Sorry your device does not support bluetooth functionality");
        }
        else
        {
            gameManager.OpenPage("ConnectionPage");
        }
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.OpenPage("StartPage");
            }
        }
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

    public void BtnBackOnClick()
    {
        gameManager.OpenPage("StartPage");
    }
}
