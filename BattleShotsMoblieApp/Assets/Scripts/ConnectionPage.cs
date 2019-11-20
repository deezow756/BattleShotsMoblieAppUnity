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
    private GameObject prevLoadingIcon;
    [SerializeField]
    private GameObject newList;
    [SerializeField]
    private GameObject newLoadingIcon;
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

    private void Start()
    {
        gameManager = GameManagerObject.GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        GetKnownDevices();
        GetNewDevices();
    }

    private void OnDisable()
    {
        gettingKnownDevices = false;
        gettingNewDevices = false;
        prevLoadingIcon.GetComponent<Button>().interactable = true;
        prevLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
        newLoadingIcon.GetComponent<Button>().interactable = true;
        newLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
        LoadingScreen.SetActive(false);
    }

    public void EnableDiscoverable()
    {
        GameManager.BluetoothPlugin.EnableDiscoverable();
    }

    public void GetKnownDevices()
    {
        knownDeviceNames = null;        
        prevLoadingIcon.GetComponent<Button>().interactable = false;
        prevLoadingIcon.GetComponent<Animator>().SetBool("Loading", true);
        string devices = GameManager.BluetoothPlugin.GetPairedDevices(); ;
        knownDeviceNames = devices.Split(',');
        if (knownDeviceNames.Length > 0)
        {
            DestoryKnownDevices();
            gettingKnownDevices = true;
        }
        else
        {
            DestoryKnownDevices();
            prevLoadingIcon.GetComponent<Button>().interactable = true;
            prevLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
        }
    }

    public void DestoryKnownDevices()
    {
        if(prevButtons.Count > 0)
        {
            for (int i = 0; i < prevButtons.Count; i++)
            {
                Destroy(prevButtons[i]);
            }
        }
    }

    public void GetNewDevices()
    {
        newButtons.Clear();
        DestoryNewDevices();
        newLoadingIcon.GetComponent<Button>().interactable = false;
        newLoadingIcon.GetComponent<Animator>().SetBool("Loading", true);        
        GameManager.BluetoothPlugin.DiscoverDevices();        
        Invoke("CancelDiscovery", 20);        
    }

    void CancelDiscovery()
    {
        GameManager.BluetoothPlugin.CancelDiscovery();
        newLoadingIcon.GetComponent<Button>().interactable = true;
        newLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
    }

    public void NewDeviceFound(string device)
    {
        GameObject button = Instantiate(listButtonPrefab, newList.transform);
        button.GetComponent<ButtonScript>().connectionPage = this;
        button.GetComponent<ButtonScript>().ButtonName = device;
        button.transform.name = device;
        button.GetComponentInChildren<Text>().text = device;
        button.GetComponent<Button>().onClick.AddListener(button.GetComponent<ButtonScript>().Clicked);
        newButtons.Add(button);
    }

    public void DestoryNewDevices()
    {
        if (newButtons.Count > 0)
        {
            for (int i = 0; i < newButtons.Count; i++)
            {
                Destroy(newButtons[i]);
            }
        }
    }

    public void CompleteConnection(string name)
    {
        //Loading Screen Stuff
        //LoadingScreen.SetActive(true);
        //GameManager.BluetoothPlugin.ConnectToDevice(name);
        gameManager.OpenPage("SetupPage1", true);
    }

    private void Update()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.OpenPage("StartPage", false);
            }

            if(gettingKnownDevices)
            {
                if (knownDeviceIndex < knownDeviceNames.Length)
                {
                    GameObject button = Instantiate(listButtonPrefab, prevList.transform);
                    button.GetComponent<ButtonScript>().connectionPage = this;
                    button.GetComponent<ButtonScript>().ButtonName = knownDeviceNames[knownDeviceIndex];
                    button.transform.name = knownDeviceNames[knownDeviceIndex];
                    button.GetComponentInChildren<Text>().text = knownDeviceNames[knownDeviceIndex];
                    button.GetComponent<Button>().onClick.AddListener(button.GetComponent<ButtonScript>().Clicked);
                    prevButtons.Add(button);
                    if (knownDeviceIndex == knownDeviceNames.Length - 1)
                    {
                        gettingKnownDevices = false;
                        knownDeviceIndex = 0;
                        prevLoadingIcon.GetComponent<Button>().interactable = true;
                        prevLoadingIcon.GetComponent<Animator>().SetBool("Loading", false);
                    }
                    else
                    {
                        knownDeviceIndex++;
                    }
                }
            }
        }
    }
}
