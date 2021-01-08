using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleSetUp2Page : MonoBehaviour
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
    private Text txtName;

    [SerializeField]
    private Text btnText;
    [SerializeField]
    private Button btnLetsPlay;

    [SerializeField]
    private GameObject errorText;

    [SerializeField]
    private GameObject LeaveGamePanel;

    [SerializeField]
    private GameObject helpPanel;

    private bool p1Done = false;

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
        SetGridInstanceToNull(gameManager.Settings.Player1Grid);
        SetGridInstanceToNull(gameManager.Settings.Player2Grid);
        CreateGrid createGrid = new CreateGrid();
        gridInstance = createGrid.GetGrid(this, grid, gameManager.Settings.SizeOfGrid, gridButtonPrefab);

        txtName.text = gameManager.Settings.Player1 + ": Place Your Shots";

        btnLetsPlay.interactable = false;
        btnText.text = "Next Player";

        txtShotsLeftToPlace.text = gameManager.Settings.NumOfShots.ToString();
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

    public void GridButtonOnClick(GameObject btn)
    {
        try
        {
            string[] split = btn.name.Split(',');
            int[,] gridTemp = null;
            List<string> coordTemp = null;
            if(!p1Done)
            {
                gridTemp = gameManager.Settings.Player1Grid;
                coordTemp = gameManager.Settings.Player1ShotCoodinates;
            }
            else
            {
                gridTemp = gameManager.Settings.Player2Grid;
                coordTemp = gameManager.Settings.Player2ShotCoodinates;
            }

            if (int.Parse(txtShotsLeftToPlace.text) > 0 && int.Parse(txtShotsLeftToPlace.text) < gameManager.Settings.NumOfShots)
            {
                if (gridTemp[int.Parse(split[0]), int.Parse(split[1])] == NULL_STATUS)
                {
                    gridTemp[int.Parse(split[0]), int.Parse(split[1])] = SHOT_STATUS;
                    btn.GetComponent<GridButtonScript>().SetShot();
                    coordTemp.Add(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) - 1).ToString();

                }
                else if (gridTemp[int.Parse(split[0]), int.Parse(split[1])] == SHOT_STATUS)
                {
                    gridTemp[int.Parse(split[0]), int.Parse(split[1])] = NULL_STATUS;
                    btn.GetComponent<GridButtonScript>().SetNoImage();
                    coordTemp.Remove(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) + 1).ToString();
                }
            }
            else if (int.Parse(txtShotsLeftToPlace.text) == 0)
            {
                if (gridTemp[int.Parse(split[0]), int.Parse(split[1])] == SHOT_STATUS)
                {
                    gridTemp[int.Parse(split[0]), int.Parse(split[1])] = NULL_STATUS;
                    btn.GetComponent<GridButtonScript>().SetNoImage();
                    coordTemp.Remove(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) + 1).ToString();
                }
            }
            else if (int.Parse(txtShotsLeftToPlace.text) == gameManager.Settings.NumOfShots)
            {
                if (gridTemp[int.Parse(split[0]), int.Parse(split[1])] == NULL_STATUS)
                {
                    gridTemp[int.Parse(split[0]), int.Parse(split[1])] = SHOT_STATUS;
                    btn.GetComponent<GridButtonScript>().SetShot();
                    coordTemp.Add(btn.name);
                    txtShotsLeftToPlace.text = (int.Parse(txtShotsLeftToPlace.text) - 1).ToString();
                }
            }

            if (txtShotsLeftToPlace.text == "0")
            {
                btnLetsPlay.interactable = true;
            }
            else
            {
                btnLetsPlay.interactable = false;
            }

            if (!p1Done)
            {
                gameManager.Settings.Player1Grid = gridTemp;
                gameManager.Settings.Player1ShotCoodinates = coordTemp;
            }
            else
            {
                gameManager.Settings.Player2Grid = gridTemp;
                gameManager.Settings.Player2ShotCoodinates = coordTemp;
            }
        }
        catch (Exception ex)
        {
            DisplayError("Error Grid Onclick");
        }
    }

    public void LetsGoOnClick()
    {
        if(!p1Done)
        {
            btnLetsPlay.interactable = false;

            foreach (GameObject btn in gridInstance)
            {
                btn.GetComponent<GridButtonScript>().SetNoImage();
            }

            txtShotsLeftToPlace.text = gameManager.Settings.NumOfShots.ToString();

            txtName.text = gameManager.Settings.Player2 + ": Place Your Shots";

            btnText.text = "Lets Play";

            p1Done = true;
        }
        else
        {
            gameManager.OpenPage("SinGamePage", true);
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
