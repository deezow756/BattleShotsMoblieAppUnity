using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupPage2 : MonoBehaviour
{
    private const int NULL_STATUS = 0;
    private const int CROSS_STATUS = 1;
    private const int SHOT_STATUS = 2;
    private const int SHOT_CROSS_STATUS = 3;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject grid;
    [SerializeField]
    private GameObject gridButtonPrefab;
    private GameObject[,] gridInstance;

    [SerializeField]
    private Text txtShotsLeftToPlace;
    [SerializeField]
    private Text txtEnemyName;
    [SerializeField]
    private Text txtEnemyStatus;

    [SerializeField]
    private Button btnLetsPlay;

    [SerializeField]
    private GameObject errorText;

    [SerializeField]
    private GameObject LeaveGamePanel;

    [SerializeField]
    private GameObject helpPanel;



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
        try
        {
            SetYourGridInstanceToNull();
            CreateGrid createGrid = new CreateGrid(this, grid, gameManager.Settings.SizeOfGrid, gridButtonPrefab);
            gridInstance = createGrid.GetGridInstance();
        }
        catch (Exception ex)
        {
            DisplayError("Error With Grid Creation");
        }

        try
        {
            if (!gameManager.Settings.Master)
            {
                btnLetsPlay.gameObject.SetActive(false);
            }
            else
            {
                btnLetsPlay.interactable = false;
            }
            gameManager.Settings.Ready = false;
            gameManager.Settings.EnemyReady = false;
            txtShotsLeftToPlace.text = gameManager.Settings.NumOfShots.ToString();
            txtEnemyName.text = gameManager.Settings.EnemyName;
            txtEnemyStatus.text = "Not Ready";
            txtEnemyStatus.color = new Color(255, 0, 0, 255);
        }
        catch (Exception ex)
        {
            DisplayError("Error Onenable");
        }
    }

    private void SetYourGridInstanceToNull()
    {
        for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
        {
            for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
            {
                gameManager.Settings.YourGrid[i, j] = 0;
            }
        }
    }

    public void GridButtonOnClick(GameObject btn)
    {
        try
        {
            string[] split = btn.name.Split(',');
            if (int.Parse(txtShotsLeftToPlace.text) > 0 && int.Parse(txtShotsLeftToPlace.text) < gameManager.Settings.NumOfShots)
            {
                if (gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] == NULL_STATUS)
                {
                    gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] = SHOT_STATUS;
                    btn.GetComponent<GridButtonScript>().SetShot();
                    gameManager.Settings.YourShotCoodinates.Add(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) - 1).ToString();

                }
                else if (gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] == SHOT_STATUS)
                {
                    gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] = NULL_STATUS;
                    btn.GetComponent<GridButtonScript>().SetNoImage();
                    gameManager.Settings.YourShotCoodinates.Remove(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) + 1).ToString();
                }
            }
            else if (int.Parse(txtShotsLeftToPlace.text) == 0)
            {
                if (gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] == SHOT_STATUS)
                {
                    gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] = NULL_STATUS;
                    btn.GetComponent<GridButtonScript>().SetNoImage();
                    gameManager.Settings.YourShotCoodinates.Remove(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) + 1).ToString();
                }
            }
            else if (int.Parse(txtShotsLeftToPlace.text) == gameManager.Settings.NumOfShots)
            {
                if (gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] == NULL_STATUS)
                {
                    gameManager.Settings.YourGrid[int.Parse(split[0]), int.Parse(split[1])] = SHOT_STATUS;
                    btn.GetComponent<GridButtonScript>().SetShot();
                    gameManager.Settings.YourShotCoodinates.Add(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) - 1).ToString();
                }
            }

            if (txtShotsLeftToPlace.text == "0")
            {
                gameManager.Settings.Ready = true;
                btnLetsPlay.interactable = true;
                GameManager.BluetoothPlugin.SendData("ready");
            }
            else
            {
                if (gameManager.Settings.Ready)
                {
                    gameManager.Settings.Ready = false;
                    btnLetsPlay.interactable = false;
                    GameManager.BluetoothPlugin.SendData("notready");
                }
            }
        }
        catch (Exception ex)
        {
            DisplayError("Error Grid Onclick");
        }
    }

    public void SetEnemyStatus(bool status)
    {
        try
        {
            if (status)
            {
                gameManager.Settings.EnemyReady = true;
                txtEnemyStatus.text = "Ready";
                txtEnemyStatus.color = new Color(0, 255, 0, 255);
            }
            else
            {
                SetGridInteractable(true);
                gameManager.Settings.EnemyReady = false;
                txtEnemyStatus.text = "Not Ready";
                txtEnemyStatus.color = new Color(255, 0, 0, 255);
            }
        }
        catch (Exception ex)
        {
            DisplayError("Error Setting Enemy Status");
        }
    }

    public void DoubleCheckReceived()
    {
        try
        {
            if (gameManager.Settings.Ready)
            {
                SetGridInteractable(false);
                GameManager.BluetoothPlugin.SendData("enemyisready");
                Invoke("NextPage", 1);
            }
            else
            {
                GameManager.BluetoothPlugin.SendData("notready");
            }
        }
        catch (Exception ex)
        {
            DisplayError("Error Double Check Received");
        }
    }

    public void EnemyIsReady()
    {
        try
        {
            gameManager.Settings.EnemyReady = true;
            Invoke("NextPage", 0.5f);
        }
        catch (Exception ex)
        {
            DisplayError("Error EnemyIsReady");
        }
    }

    public void LetsPlayOnClick()
    {
        try
        {
            if (gameManager.Settings.EnemyReady && gameManager.Settings.Ready)
            {
                SetGridInteractable(false);
                GameManager.BluetoothPlugin.SendData("doublecheck");
            }
        }
        catch (Exception ex)
        {
            DisplayError("Error Lets Play Onclick");
        }
    }

    public void NextPage()
    {
        try
        {
            gameManager.OpenPage("GamePage", true);
        }
        catch (Exception ex)
        {
            DisplayError("Error With Next Page");
        }
    }

    private void SetGridInteractable(bool value)
    {
        try
        {
            for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
            {
                for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
                {
                    GameObject btn = gridInstance[i, j];
                    btn.GetComponent<Button>().interactable = value;
                }
            }
        }
        catch (Exception ex)
        {
            DisplayError("Error Changing Interaction With Grid");
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

}
