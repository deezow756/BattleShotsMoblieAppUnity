using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleGamePage : MonoBehaviour
{
    enum Turn { Player1, Player2}
    private const int NULL_STATUS = 0;
    private const int CROSS_STATUS = 1;
    private const int SHOT_STATUS = 2;
    private const int SHOT_CROSS_STATUS = 3;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject grid;
    [SerializeField]
    private GameObject[,] gridInstance;
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private Text txtP1Name;
    [SerializeField]
    private Text txtP2Name;
    [SerializeField]
    private Text txtP1Shotsleft;
    [SerializeField]
    private Text txtP2Shotsleft;
    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject winPanel;
    [SerializeField]
    private Text txtWinPanel;

    private GameObject curGridBtn;

    [SerializeField]
    private GameObject LeaveGamePanel;

    [SerializeField]
    private GameObject helpPanel;

    [SerializeField]
    GameObject hitAnimation;

    private Turn turn;

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
        gameManager.Settings = new GameSettings();
        gameManager.OpenPage("MenuPage", false);
    }

    public void LeaveNo()
    {
        LeaveGamePanel.SetActive(false);
    }

    private void OnEnable()
    {
        LeaveGamePanel.SetActive(false);
        winPanel.SetActive(false);

        txtP1Name.text = gameManager.Settings.Player1;
        txtP2Name.text = gameManager.Settings.Player2;

        CreateGrid EnemyGrid = new CreateGrid();
        gridInstance = EnemyGrid.GetGrid(this, true, grid, gameManager.Settings.EnemyGrid, gameManager.Settings.SizeOfGrid, buttonPrefab);

        txtP1Shotsleft.text = gameManager.Settings.NumOfShots.ToString();
        txtP2Shotsleft.text = gameManager.Settings.NumOfShots.ToString();

        SetGridInstanceToNull(gameManager.Settings.Player1Grid);
        SetGridInstanceToNull(gameManager.Settings.Player2Grid);

        CoinFlip();
    }

    private void OnDisable()
    {
        winPanel.SetActive(false);
        status.text = "";
        txtP1Shotsleft.text = "";
        txtP2Shotsleft.text = "";
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

        GameObject[] yourGridChildren = grid.GetComponentsInChildren<GameObject>();

        for (int i = 0; i < yourGridChildren.Length; i++)
        {
            Destroy(yourGridChildren[i]);
        }
    }

    public void EnemyGridButtonOnClick(GameObject btn)
    {
        if (turn == Turn.Player1)
        {
            if (gameManager.Settings.P1AllReadySelected.Contains(btn.name))
            {
                GameManager.BluetoothPlugin.Toast("Youve Already Tried This One");
            }
            else
            {
                SetGridInteraction(false);
                gameManager.Settings.P1AllReadySelected.Add(btn.name);
                curGridBtn = btn;

                string coordenates = btn.name;

                if (gameManager.Settings.Player2ShotCoodinates.Contains(coordenates))
                {
                    hitAnimation.SetActive(true);
                    string[] split = coordenates.Split(',');
                    txtP2Shotsleft.text = (int.Parse(txtP2Shotsleft.text) - 1).ToString();
                    gameManager.Settings.Player1Grid[int.Parse(split[0]), int.Parse(split[1])] = SHOT_CROSS_STATUS;
                    curGridBtn.GetComponent<GridButtonScript>().SetShotWithCross();
                    if (txtP2Shotsleft.text == "0")
                    {
                        winPanel.SetActive(true);
                        txtWinPanel.text = gameManager.Settings.Player1 + " Won\n" + gameManager.Settings.Player2 + " drinks the rest of the shots!";
                        return;
                    }
                    Invoke("NextTurn", 4);
                }
                else
                {
                    string[] split = coordenates.Split(',');
                    gameManager.Settings.Player1Grid[int.Parse(split[0]), int.Parse(split[1])] = CROSS_STATUS;
                    curGridBtn.GetComponent<GridButtonScript>().SetCross();
                    Invoke("NextTurn", 1.5f);
                }
            }
        }
        else
        {
            if (gameManager.Settings.P2AllReadySelected.Contains(btn.name))
            {
                GameManager.BluetoothPlugin.Toast("Youve Already Tried This One");
            }
            else
            {
                SetGridInteraction(false);
                gameManager.Settings.P2AllReadySelected.Add(btn.name);
                curGridBtn = btn;

                string coordenates = btn.name;

                if (gameManager.Settings.Player1ShotCoodinates.Contains(coordenates))
                {
                    hitAnimation.SetActive(true);
                    string[] split = coordenates.Split(',');
                    txtP1Shotsleft.text = (int.Parse(txtP1Shotsleft.text) - 1).ToString();
                    gameManager.Settings.Player2Grid[int.Parse(split[0]), int.Parse(split[1])] = SHOT_CROSS_STATUS;
                    curGridBtn.GetComponent<GridButtonScript>().SetShotWithCross();
                    if (txtP1Shotsleft.text == "0")
                    {
                        winPanel.SetActive(true);
                        txtWinPanel.text = gameManager.Settings.Player2 + " Won\n" + gameManager.Settings.Player1 + " drinks the rest of the shots!";
                        return;
                    }
                    Invoke("NextTurn", 4);
                }
                else
                {
                    string[] split = coordenates.Split(',');
                    gameManager.Settings.Player2Grid[int.Parse(split[0]), int.Parse(split[1])] = CROSS_STATUS;
                    curGridBtn.GetComponent<GridButtonScript>().SetCross();
                    Invoke("NextTurn", 1.5f);
                }               
            }
        }
    }

    public void NextTurn()
    {
        if (turn == Turn.Player1)
        {
            status.text = gameManager.Settings.Player2 + "'s Turn";
            turn = Turn.Player2;
        }
        else
        {
            status.text = gameManager.Settings.Player1 + "'s Turn";
            turn = Turn.Player1;
        }
        RefreshGrid();
        SetGridInteraction(true);
    }

    public void RefreshGrid()
    {
        if(turn == Turn.Player1)
        {
            for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
            {
                for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
                {
                    switch (gameManager.Settings.Player1Grid[i,j])
                    {
                        case 0:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetNoImage();
                            break;
                        case 1:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetCross();
                            break;
                        case 2:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetShot();
                            break;
                        case 3:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetShotWithCross();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
            {
                for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
                {
                    switch (gameManager.Settings.Player2Grid[i, j])
                    {
                        case 0:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetNoImage();
                            break;
                        case 1:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetCross();
                            break;
                        case 2:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetShot();
                            break;
                        case 3:
                            gridInstance[i, j].GetComponent<GridButtonScript>().SetShotWithCross();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    private void SetGridInstanceToNull(int[,] _grid)
    {
        for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
        {
            for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
            {
                _grid[i, j] = 0;
            }
        }
    }

    public void CoinFlip()
    {
        int ran = UnityEngine.Random.Range(0, 1);

        if (ran == 1)
        {
            turn = Turn.Player1;
            status.text = gameManager.Settings.Player1 + "'s Turn";
        }
        else
        {
            turn = Turn.Player2;
            status.text = gameManager.Settings.Player2 + "'s Turn";
        }
        SetGridInteraction(true);
    }

    public void SetGridInteraction(bool value)
    {
        for (int i = 0; i < gameManager.Settings.SizeOfGrid; i++)
        {
            for (int j = 0; j < gameManager.Settings.SizeOfGrid; j++)
            {
                gridInstance[i, j].GetComponent<Button>().interactable = value;
            }
        }
    }

    public void PlayAgainOnClick()
    {
        gameManager.Settings.P1AllReadySelected.Clear();
        gameManager.Settings.P2AllReadySelected.Clear();
        gameManager.Settings.Player1ShotCoodinates.Clear();
        gameManager.Settings.Player2ShotCoodinates.Clear();
        SetGridInstanceToNull(gameManager.Settings.Player1Grid);
        SetGridInstanceToNull(gameManager.Settings.Player2Grid);
        gameManager.Settings.YourTurn = false;

        if (winPanel.activeSelf)
        {
            winPanel.SetActive(false);
        }

        gameManager.OpenPage("SinSetupPage1", false);
    }

    public void NewGameOnClick()
    {
        gameManager.Settings = new GameSettings();

        if (winPanel.activeSelf)
        {
            winPanel.SetActive(false);
        }

        gameManager.OpenPage("MenuPage", false);
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
