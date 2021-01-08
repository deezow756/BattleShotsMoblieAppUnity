using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public string ConnectedDeviceName { get; set; }
    public bool Master { get; set; }
    public string YourName { get; set; }
    public string EnemyName { get; set; }
    private int sizeOfGrid;
    public int SizeOfGrid
    {
        get
        {
            return sizeOfGrid;
        }
        set
        {
            if (value != 0)
            {
                Player1Grid = new int[value, value];
                Player2Grid = new int[value, value];
                YourGrid = new int[value, value];
                EnemyGrid = new int[value, value];
                sizeOfGrid = value;
            }
        }
    }
    public int NumOfShots { get; set; }

    public int MaxNumOfShots = 10;
    public int NumEnemyShots { get; set; }

    public int[,] EnemyGrid;

    public int[,] YourGrid;

    public int[,] Player1Grid;

    public int[,] Player2Grid;

    public List<string> AllReadySelected = new List<string>();

    public List<string> P1AllReadySelected = new List<string>();

    public List<string> P2AllReadySelected = new List<string>();

    public List<string> YourShotCoodinates = new List<string>();

    public List<string> Player1ShotCoodinates = new List<string>();

    public List<string> Player2ShotCoodinates = new List<string>();

    public bool Ready { get; set; }
    public bool EnemyReady { get; set; }

    public bool YourTurn { get; set; }

    public string Player1 { get; set; }
    public string Player2 { get; set; }

    public GameSettings()
    {
    }

    public void ResetSettings()
    {
        ConnectedDeviceName = "";
        Master = false;
        YourName = "";
        EnemyName = "";
        SizeOfGrid = 0;
        YourGrid = null;
        EnemyGrid = null;
        NumOfShots = 0;
        NumEnemyShots = 0;
        AllReadySelected.Clear();
        YourShotCoodinates.Clear();
        Ready = false;
        EnemyReady = false;
        YourTurn = false;
    }
}
