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

    public List<string> AllReadySelected = new List<string>();

    public List<string> YourShotCoodinates = new List<string>();
    public bool Ready { get; set; }
    public bool EnemyReady { get; set; }

    public bool YourTurn { get; set; }

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
