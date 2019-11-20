using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    GameObject[,] Grid;
    public CreateGrid(GameObject grid, int sizeOfGrid, GameObject buttonPrefab)
    {
        Grid = new GameObject[sizeOfGrid, sizeOfGrid];
        float offSet = grid.GetComponent<RectTransform>().rect.width / sizeOfGrid;
        float y = 0;
        float x = 0;
        for (int i = 0; i < sizeOfGrid; i++)
        {
            for (int j = 0; j < sizeOfGrid; j++)
            {
                GameObject btn = Instantiate(buttonPrefab, new Vector3(x, y), new Quaternion(0, 0, 0, 0), grid.transform);
                btn.name = i.ToString() + "," + j.ToString();
                RectTransform rectTransform = btn.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(offSet, offSet);
                Grid[i, j] = btn;
                x += offSet;
            }
            y += offSet;
        }
    }

    public GameObject[,] GetGridInstance()
    {
        return Grid;        
    }
}
