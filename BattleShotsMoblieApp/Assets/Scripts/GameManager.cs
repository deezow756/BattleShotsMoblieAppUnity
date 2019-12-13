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

    public string ListToString(List<string> list)
    {
        string s = "";
        for (int i = 0; i < list.Count; i++)
        {
            if(i == list.Count - 1)
            {
                s += list[i];
            }
            else
            {
                s += list[i] + ",";
            }
        }
        return s;
    }
    public string ListToString(string[] list)
    {
        string s = "";
        for (int i = 0; i < list.Length; i++)
        {
            if (i == list.Length - 1)
            {
                s += list[i];
            }
            else
            {
                s += list[i] + ",";
            }
        }
        return s;
    }

    #region Callbacks
    public void NewDeviceFound(string device)
    {
        connectionPage.GetComponent<ConnectionPage>().NewDeviceFound(device);
    }

    public void ConnectedToDeviceCallBack(string msg)
    {
        if (msg == "1")
        {
            settings.Master = true;
            connectionPage.GetComponent<ConnectionPage>().LoadingScreen.SetActive(false);
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
        BluetoothPlugin.WriteAddDevice(device);
        OpenPage("SetupPage1", true);
    }

    public void DisconnectedCallBack(string status)
    {
        settings.ResetSettings();
        OpenPage("ConnectionPage", false);
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
                    insSetupPage1.ReceiveGridSize(split[1]);
                }
                else if (split[0].Equals("shots"))
                {
                    Settings.NumOfShots = int.Parse(split[1]);
                    insSetupPage1.ReceiveNumOfShots(split[1]);
                }
            }
            else
            {
                if (msg.Equals("continue"))
                {
                    insSetupPage1.NextPage();
                }
            }
        }
        else if(setUpPage2.activeSelf)
        {
            SetupPage2 insSetupPage2 = setUpPage2.GetComponent<SetupPage2>();
            if(msg.Equals("enemyisready"))
            {
                insSetupPage2.EnemyIsReady();
            }
            else if(msg.Equals("doublecheck"))
            {
                insSetupPage2.DoubleCheckReceived();
            }
            else if(msg.Equals("notready"))
            {
                insSetupPage2.SetEnemyStatus(false);
            }
            else if(msg.Equals("ready"))
            {
                insSetupPage2.SetEnemyStatus(true);
            }
        }
        else if(gamePage.activeSelf)
        {
            GamePage insGamePage = gamePage.GetComponent<GamePage>();
            string[] split = msg.Split('.');
            if (split.Length > 1)
            {
                if(split[0].Equals("coordenates"))
                {
                    insGamePage.ReceiveTry(split[1]);
                }
            }
            else
            {
                if (msg.Equals("first"))
                {
                    insGamePage.ReceiveCoinFlip(true);
                }
                else if (msg.Equals("second"))
                {
                    insGamePage.ReceiveCoinFlip(false);
                }
                else if (msg.Equals("hit"))
                {
                    insGamePage.ReceiveHitOrMiss(true);
                }
                else if (msg.Equals("miss"))
                {
                    insGamePage.ReceiveHitOrMiss(false);
                }
                else if (msg.Equals("win"))
                {
                    insGamePage.ReceiveLost();
                }
                else if (msg.Equals("ready"))
                {
                    insGamePage.ReceiveReady();
                }
                else if (msg.Equals("tryagain"))
                {
                    insGamePage.ReceiveTryAgain();
                }
                else if (msg.Equals("exit"))
                {
                    insGamePage.ReceiveExit();
                }
            }
        }

    }

    public void DataSentCallBack(string status)
    {

    }

    #endregion
}
