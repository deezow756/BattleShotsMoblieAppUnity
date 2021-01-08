using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePage : MonoBehaviour
{
    private const int NULL_STATUS = 0;
    private const int CROSS_STATUS = 1;
    private const int SHOT_STATUS = 2;
    private const int SHOT_CROSS_STATUS = 3;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject enemyGrid;
    [SerializeField]
    private GameObject[,] enemyGridInstance;
    [SerializeField]
    private GameObject yourGrid;
    [SerializeField]
    private GameObject[,] yourGridInstance;
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private Text txtShotsleft;
    [SerializeField]
    private Text status;

    [SerializeField]
    private Text errorText;

    [SerializeField]
    private GameObject winPanel;

    [SerializeField]
    private GameObject lostPanel;

    private GameObject curGridBtn;

    [SerializeField]
    private GameObject LeaveGamePanel;

    [SerializeField]
    private GameObject helpPanel;

    [SerializeField]
    GameObject hitAnimation;

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
        GameManager.BluetoothPlugin.Disconnect();
        gameManager.Settings = new GameSettings();
        GameManager.BluetoothPlugin.Reset();
        gameManager.OpenPage("ConnectionPage", false);
    }

    public void LeaveNo()
    {
        LeaveGamePanel.SetActive(false);
    }

    private void OnEnable()
    {
        LeaveGamePanel.SetActive(false);
        winPanel.SetActive(false);
        lostPanel.SetActive(false);
        try
        {
            CreateGrid EnemyGrid = new CreateGrid(this, true, enemyGrid, gameManager.Settings.EnemyGrid, gameManager.Settings.SizeOfGrid, buttonPrefab);
            enemyGridInstance = EnemyGrid.GetGridInstance();
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Creating Enemy Grid";
        }
        try
        {
            CreateGrid YourGrid = new CreateGrid(this, false, yourGrid, gameManager.Settings.YourGrid, gameManager.Settings.SizeOfGrid, buttonPrefab);
            yourGridInstance = YourGrid.GetGridInstance();
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Creating Your Grid";
        }

        txtShotsleft.text = gameManager.Settings.NumOfShots.ToString();
        gameManager.Settings.NumEnemyShots = gameManager.Settings.NumOfShots;

        CoinFlip();
    }

    private void OnDisable()
    {
        winPanel.SetActive(false);
        lostPanel.SetActive(false);
        status.text = "";
        txtShotsleft.text = "";
        DestoryGrids();
    }

    private void DestoryGrids()
    {
        for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
        {
            for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
            {
                gameManager.Settings.EnemyGrid[i, j] = 0;

            }
        }

        GameObject[] yourGridChildren = yourGrid.GetComponentsInChildren<GameObject>();
        GameObject[] enemyGridChildren = enemyGrid.GetComponentsInChildren<GameObject>();

        for (int i = 0; i < yourGridChildren.Length; i++)
        {
            Destroy(yourGridChildren[i]);
            Destroy(enemyGridChildren[i]);
        }
    }

    public void EnemyGridButtonOnClick(GameObject btn)
    {
        try
        {
            if (gameManager.Settings.AllReadySelected.Contains(btn.name))
            {
                GameManager.BluetoothPlugin.Toast("Youve Already Tried This One");
            }
            else
            {
                SetGridInteraction(false);
                gameManager.Settings.AllReadySelected.Add(btn.name);
                curGridBtn = btn;
                GameManager.BluetoothPlugin.SendData("coordenates" + "." + btn.name);
            }
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Enemy Grid Onclick";
        }
    }

    public void ReceiveTry(string coordenates)
    {
        try
        {
            if (gameManager.Settings.YourShotCoodinates.Contains(coordenates))
            {
                //GameManager.BluetoothPlugin.Toast(gameManager.Settings.EnemyName + " Hit");
                hitAnimation.SetActive(true);
                GameManager.BluetoothPlugin.SendData("hit");
                string[] split = coordenates.Split(',');
                yourGridInstance[int.Parse(split[0]), int.Parse(split[1])].GetComponent<GridButtonScript>().SetShotWithCross();
                txtShotsleft.text = (int.Parse(txtShotsleft.text) - 1).ToString();
            }
            else
            {
                //GameManager.BluetoothPlugin.Toast(gameManager.Settings.EnemyName + " Missed");
                GameManager.BluetoothPlugin.SendData("miss");
                string[] split = coordenates.Split(',');
                yourGridInstance[int.Parse(split[0]), int.Parse(split[1])].GetComponent<GridButtonScript>().SetCross();
                //txtShotsleft.text = (int.Parse(txtShotsleft.text) - 1).ToString();
            }
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Recieve Try";
        }
    }

    public void ReceiveHitOrMiss(bool value)
    {
        try
        {
            if (value)
            {
                //GameManager.BluetoothPlugin.Toast("Hit!");
                // animation for hit
                curGridBtn.GetComponent<GridButtonScript>().SetShotWithCross();
                gameManager.Settings.NumEnemyShots -= 1;
                if (gameManager.Settings.NumEnemyShots == 0)
                {
                    GameManager.BluetoothPlugin.SendData("win");
                    winPanel.SetActive(true);
                    return;
                }
            }
            else
            {
                //GameManager.BluetoothPlugin.Toast("Miss");
                // animation for miss
                curGridBtn.GetComponent<GridButtonScript>().SetCross();
            }
            status.text = gameManager.Settings.EnemyName + "'s Turn";
            GameManager.BluetoothPlugin.SendData("ready");
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Receive Hit Or Miss";
        }
    }

    public void ReceiveReady()
    {
        status.text = "Your Turn";
        SetGridInteraction(true);
    }

    public void ReceiveLost()
    {
        lostPanel.SetActive(true);
    }

    public void CoinFlip()
    {
        try
        {
            //placeholder
            int ran = UnityEngine.Random.Range(0, 1);

            if (ran == 1)
            {
                SetGridInteraction(true);
                gameManager.Settings.YourTurn = true;
                status.text = "Your Turn";
                GameManager.BluetoothPlugin.SendData("second");
            }
            else
            {
                SetGridInteraction(false);
                gameManager.Settings.YourTurn = false;
                status.text = gameManager.Settings.EnemyName + "'s Turn";
                GameManager.BluetoothPlugin.SendData("first");
            }
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error CoinFlip";
        }
    }

    public void ReceiveCoinFlip(bool value)
    {
        try
        {
            if (value)
            {
                SetGridInteraction(true);
                gameManager.Settings.YourTurn = true;
                status.text = "Your Turn";
            }
            else
            {
                SetGridInteraction(false);
                gameManager.Settings.YourTurn = false;
                status.text = gameManager.Settings.EnemyName + "'s Turn";
            }
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Receive Coin Flip";
        }
    }

    public void SetGridInteraction(bool value)
    {
        try
        {
            for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
            {
                for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
                {
                    enemyGridInstance[i, j].GetComponent<Button>().interactable = value;
                }
            }
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Changing Grid Interaction";
        }
    }

    public void PlayAgainOnClick()
    {
        try
        {
            GameManager.BluetoothPlugin.SendData("tryagain");

            gameManager.Settings.AllReadySelected.Clear();
            gameManager.Settings.YourShotCoodinates.Clear();
            gameManager.Settings.SizeOfGrid = gameManager.Settings.SizeOfGrid;
            gameManager.Settings.YourTurn = false;

            if (lostPanel.activeSelf)
            {
                lostPanel.SetActive(false);
            }
            else if (winPanel.activeSelf)
            {
                winPanel.SetActive(false);
            }

            gameManager.OpenPage("SetupPage1", false);
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Play Again Onclick";
        }
    }

    public void NewGameOnClick()
    {
        try
        {
            GameManager.BluetoothPlugin.SendData("exit");

            GameManager.BluetoothPlugin.Reset();

            gameManager.Settings = new GameSettings();

            if (lostPanel.activeSelf)
            {
                lostPanel.SetActive(false);
            }
            else if (winPanel.activeSelf)
            {
                winPanel.SetActive(false);
            }

            gameManager.OpenPage("ConnectionPage", false);
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error New Game Onclick";
        }
    }

    public void ReceiveTryAgain()
    {
        try
        {
            gameManager.Settings.AllReadySelected.Clear();
            gameManager.Settings.YourShotCoodinates.Clear();
            gameManager.Settings.SizeOfGrid = gameManager.Settings.SizeOfGrid;
            gameManager.Settings.YourTurn = false;

            if (lostPanel.activeSelf)
            {
                lostPanel.SetActive(false);
            }
            else if (winPanel.activeSelf)
            {
                winPanel.SetActive(false);
            }

            gameManager.OpenPage("SetupPage1", false);
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Receive Try Again";
        }
    }

    public void ReceiveExit()
    {
        try
        {
            GameManager.BluetoothPlugin.Reset();

            gameManager.Settings = new GameSettings();

            if (lostPanel.activeSelf)
            {
                lostPanel.SetActive(false);
            }
            else if (winPanel.activeSelf)
            {
                winPanel.SetActive(false);
            }

            gameManager.OpenPage("ConnectionPage", false);
        }
        catch (Exception ex)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Error Receive Exit";
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
