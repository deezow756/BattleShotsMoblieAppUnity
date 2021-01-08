using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPage : MonoBehaviour
{
    public GameObject GameManagerObject;
    GameManager gameManager;

    [SerializeField]
    private GameObject Canvas;
    [SerializeField]
    private GameObject prevList;
    [SerializeField]
    private GameObject newList;
    [SerializeField]
    private GameObject newLoadingIcon;
    [SerializeField]
    private Button btnNewLoading;
    [SerializeField]
    private GameObject listButtonPrefab;

    [SerializeField]
    private GameObject errorText;

    private List<GameObject> prevButtons = new List<GameObject>();
    private string[] knownDeviceNames;
    private int knownDeviceIndex;
    private bool gettingKnownDevices;

    private List<GameObject> newButtons = new List<GameObject>();
    private int newDeviceIndex;
    private bool gettingNewDevices;
    [SerializeField]
    private GameObject loadingScreen;

    public GameObject LoadingScreen { get => loadingScreen; }

    [SerializeField]
    private GameObject helpPanel;

    [SerializeField]
    private Text txtDeviceName;

    private void Start()
    {
        gameManager = GameManagerObject.GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        DestoryKnownDevices();
        DestoryNewDevices();
        gettingKnownDevices = false;
        gettingNewDevices = false;
        btnNewLoading.interactable = true;
        newLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
        LoadingScreen.SetActive(false);
        helpPanel.SetActive(false);
        GetKnownDevices();
        GetNewDevices();
        txtDeviceName.text = GameManager.BluetoothPlugin.GetDeviceName();
        string IsReceiving = GameManager.BluetoothPlugin.GetIsReceiving();
        if (IsReceiving == "0")
        {
            GameManager.BluetoothPlugin.ReceivePair();
        }
    }

    private void OnDisable()
    {
        gettingKnownDevices = false;
        gettingNewDevices = false;
        btnNewLoading.interactable = true;
        newLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
        LoadingScreen.SetActive(false);
    }

    public void EnableDiscoverable()
    {
        GameManager.BluetoothPlugin.EnableDiscoverable();
    }

    public void GetKnownDevices()
    {
        try
        {
            knownDeviceNames = null;
            string devices = GameManager.BluetoothPlugin.GetPairedDevices();
            knownDeviceNames = devices.Split(',');
            if (knownDeviceNames.Length > 0)
            {
                gettingKnownDevices = true;
            }
        }
        catch(Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Getting Known Devices";
        }
    }

    public void DestoryKnownDevices()
    {
        try
        {
            if (prevButtons.Count > 0)
            {
                for (int i = 0; i < prevButtons.Count; i++)
                {
                    Destroy(prevButtons[i]);
                }
            }
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Destoring Known Devices";
        }
    }

    public void GetNewDevices()
    {
        try
        {            
            DestoryNewDevices();
            newButtons.Clear();
            btnNewLoading.interactable = false;
            newLoadingIcon.GetComponent<Animator>().SetBool("Loading", true);
            GameManager.BluetoothPlugin.DiscoverDevices();
            Invoke("CancelDiscovery", 10);
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Getting New Devices";
        }
    }

    void CancelDiscovery()
    {
        try
        {
            GameManager.BluetoothPlugin.CancelDiscovery();
            btnNewLoading.interactable = true;
            newLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Canceling Discovery";
        }
    }

    public void NewDeviceFound(string device)
    {
        try
        {
            GameObject button = Instantiate(listButtonPrefab, newList.transform);
            button.GetComponent<ButtonScript>().connectionPage = this;
            button.GetComponent<ButtonScript>().ButtonName = device;
            button.transform.name = device;
            button.GetComponentInChildren<Text>().text = device.Split('.')[0];
            button.GetComponent<Button>().onClick.AddListener(button.GetComponent<ButtonScript>().Clicked);
            newButtons.Add(button);
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error New Device Found";
        }
    }

    public void DestoryNewDevices()
    {
        try
        {
            if (newButtons.Count > 0)
            {
                for (int i = 0; i < newButtons.Count; i++)
                {
                    Destroy(newButtons[i]);
                }
            }
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Destoring New Devices";
        }
    }

    public void CompleteConnection(string name)
    {
        try
        {
            LoadingScreen.SetActive(true);
            GameManager.BluetoothPlugin.ConnectToDevice(name);
            //gameManager.OpenPage("SetupPage1", true);
        }
        catch (Exception ex)
        {
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = "Error Starting Connection To " + name;
        }
    }

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.OpenPage("MenuPage", false);
            }

            if(gettingKnownDevices)
            {
                if (knownDeviceIndex < knownDeviceNames.Length)
                {
                    if (knownDeviceNames[knownDeviceIndex] != "")
                    {
                        GameObject button = Instantiate(listButtonPrefab, prevList.transform);
                        button.GetComponent<ButtonScript>().connectionPage = this;
                        button.GetComponent<ButtonScript>().ButtonName = knownDeviceNames[knownDeviceIndex];
                        button.transform.name = knownDeviceNames[knownDeviceIndex];
                        button.GetComponentInChildren<Text>().text = knownDeviceNames[knownDeviceIndex].Split('.')[0];
                        button.GetComponent<Button>().onClick.AddListener(button.GetComponent<ButtonScript>().Clicked);
                        prevButtons.Add(button);
                    }

                    if (knownDeviceIndex == knownDeviceNames.Length - 1)
                    {
                        gettingKnownDevices = false;
                        knownDeviceIndex = 0;
                    }
                    else
                    {
                        knownDeviceIndex++;
                    }
                }
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
