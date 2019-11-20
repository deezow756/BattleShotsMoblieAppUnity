using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupPage2 : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject grid;
    [SerializeField]
    private GameObject gridButtonPrefab;

    [SerializeField]
    private Text txtShotsLeftToPlace;

    private void OnEnable()
    {
        CreateGrid createGrid = new CreateGrid(grid, gameManager.Settings.SizeOfGrid, gridButtonPrefab);
        gameManager.Settings.YourGrid = createGrid.GetGridInstance();
    }

}
