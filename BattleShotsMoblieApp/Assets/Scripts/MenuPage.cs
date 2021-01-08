using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public GameObject GameManagerObject;
    GameManager gameManager;

    [SerializeField]
    private GameObject Canvas;

    [SerializeField]
    private GameObject helpPanel;

    private void Start()
    {
        gameManager = GameManagerObject.GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        if(GameManager.BluetoothPlugin.GetIsReceiving() == "0")
        {
            GameManager.BluetoothPlugin.ReceivePair();
        }

        helpPanel.SetActive(false);
        
    }

    public void BtnSingleOnClick()
    {
        gameManager.OpenPage("SinSetUp1");        
    }
    public void BtnMultiOnClick()
    {
        gameManager.OpenPage("ConnectionPage");        
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
    public void BtnHelpOnClick()
    {
        helpPanel.SetActive(true);
    }

    public void BtnHelpOkOnClick()
    {
        helpPanel.SetActive(false);
    }
}
