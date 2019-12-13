using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateGrid : MonoBehaviour
{
    private const int SHOT_STATUS = 2;

    GameObject[,] Grid;
    public CreateGrid(SetupPage2 setupPage2, GameObject grid, int sizeOfGrid, GameObject buttonPrefab)
    {
        Grid = new GameObject[sizeOfGrid, sizeOfGrid];
        float offSet = grid.GetComponent<RectTransform>().rect.width / sizeOfGrid;
        float y = 0;
        float x = 0;
        for (int i = 0; i < sizeOfGrid; i++)
        {
            for (int j = 0; j < sizeOfGrid; j++)
            {
                GameObject btn = Instantiate(buttonPrefab, grid.transform);
                btn.name = i.ToString() + "," + j.ToString();
                RectTransform rectTransform = btn.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(offSet, offSet);
                rectTransform.anchoredPosition = new Vector2(x, y * -1);
                btn.GetComponent<GridButtonScript>().setupPage2 = setupPage2;
                Grid[i, j] = btn;
                x += offSet;
            }
            y += offSet;
            x = 0;
        }
    }

    public CreateGrid(GamePage gamePage, bool enemy, GameObject grid, int[,] gridStatus , int sizeOfGrid, GameObject buttonPrefab)
    {
        Grid = new GameObject[sizeOfGrid, sizeOfGrid];
        float offSet = grid.GetComponent<RectTransform>().rect.width / sizeOfGrid;
        float y = 0;
        float x = 0;
        for (int i = 0; i < sizeOfGrid; i++)
        {
            for (int j = 0; j < sizeOfGrid; j++)
            {
                GameObject btn = Instantiate(buttonPrefab, grid.transform);
                btn.name = i.ToString() + "," + j.ToString();
                RectTransform rectTransform = btn.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(x, y * -1);
                rectTransform.sizeDelta = new Vector2(offSet, offSet);
                GridButtonScript btnScript = btn.GetComponent<GridButtonScript>();
                btnScript.gamePage = gamePage;
                btnScript.enemy = enemy;
                if(!enemy)
                {
                    if(gridStatus[i,j] == SHOT_STATUS)
                    {
                        btnScript.SetShot();
                    }
                }
                Grid[i, j] = btn;
                x += offSet;
            }
            y += offSet;
            x = 0;
        }
    }
    
    public GameObject[,] GetGridInstance()
    {
        return Grid;
    }
}
