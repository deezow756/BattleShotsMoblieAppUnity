using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera Cam;
    private Animator camAnimator;
    [SerializeField]
    private GameObject startPage;
    [SerializeField]
    private GameObject connectionPage;
    [SerializeField]
    private GameObject setUpPage1;
    [SerializeField]
    private GameObject setUpPage2;
    [SerializeField]
    private GameObject gamePage;

    private GameObject currentPage;
    private GameObject prevPage;

    private GameSettings settings;
    public GameSettings Settings
    {
        get { return settings; }
        set { settings = value; }
    }

    static BluetoothWrapper bluetoothPlugin;
    public static BluetoothWrapper BluetoothPlugin { get => bluetoothPlugin; set { bluetoothPlugin = value; } }

    void Start()
    {        
        camAnimator = Cam.GetComponent<Animator>();
        BluetoothPlugin = BluetoothWrapper.pluginWithGameObjectName(this.transform.name);
        Settings = new GameSettings();
        startPage.SetActive(true);
        connectionPage.SetActive(false);
        setUpPage1.SetActive(false);
        setUpPage2.SetActive(false);
        gamePage.SetActive(false);
        currentPage = startPage;
    }

    public void OpenPage(string _pageName, bool _transition)
    {
        prevPage = currentPage;
        //if(_transition)
        //{
        //    camAnimator.Play("Next");
        //}
        //else
        //{
        //    camAnimator.Play("Prev");
        //}
        switch (_pageName)
        {
            case "StartPage":
                currentPage = startPage;
                break;
            case "ConnectionPage":
                currentPage = connectionPage;
                break;
            case "SetupPage1":
                currentPage = setUpPage1;
                break;
            case "SetupPage2":
                currentPage = setUpPage2;
                break;
            case "GamePage":
                currentPage = gamePage;
                break;
            default:
                break;
        }
        SetPages();
    }

    private void SetPages()
    {
        currentPage.SetActive(true);
        prevPage.SetActive(false);
    }
    
    #region Callbacks
    public void NewDeviceFound(string device)
    {
        connectionPage.GetComponent<ConnectionPage>().NewDeviceFound(device);
    }

    public void CheckStarted(string message)
    {
        startPage.GetComponent<StartPage>().CheckStarted(message);
    }

    public void ConnectedToDeviceCallBack(string msg)
    {
        if (msg == "1")
        {
            settings.Master = true;
            OpenPage("SetupPage1", true);
        }
        else
        {
            connectionPage.GetComponent<ConnectionPage>().LoadingScreen.SetActive(false);
        }
    }

    public void ReceivedPairCallBack(string device)
    {
        Settings.Master = false;
        settings.ConnectedDeviceName = device;
        OpenPage("SetupPage1", true);
    }

    public void ReceivedDataCallBack(string msg)
    {
        if(setUpPage1.activeSelf)
        {
            SetupPage1 insSetupPage1 = setUpPage1.GetComponent<SetupPage1>();
            string[] split = msg.Split(',');
            if(split.Length > 1)
            {
                if(split[0].Equals("name"))
                {
                    Settings.EnemyName = split[1];
                }
                else if(split[0].Equals("grid"))
                {
                    Settings.SizeOfGrid = int.Parse(split[1]);
                }
                else if (split[0].Equals("shots"))
                {
                    Settings.NumOfShots = int.Parse(split[1]);
                }
            }
            else
            {

            }
        }
        else if(setUpPage2.activeSelf)
        {

        }
        else if(gamePage.activeSelf)
        {

        }

    }

    public void DataSentCallBack(string status)
    {

    }

    #endregion
}
