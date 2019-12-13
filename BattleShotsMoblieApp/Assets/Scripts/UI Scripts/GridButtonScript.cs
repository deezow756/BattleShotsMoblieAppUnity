using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridButtonScript : MonoBehaviour
{
    public SetupPage2 setupPage2;
    public GamePage gamePage;
    public bool enemy;
    [SerializeField]
    Image image;
    [SerializeField]
    private Sprite cross;
    [SerializeField]
    private Sprite shot;
    [SerializeField]
    private Sprite shotWithCross;

    private Button btn;

    private void OnEnable()
    {
        SetNoImage();
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void SetNoImage()
    {
        image.sprite = null;
        image.color = new Color32(255, 255, 225, 0);
    }
    public void SetCross()
    {
        image.sprite = cross;
        image.color = new Color32(255, 255, 225, 255);
    }
    public void SetShot()
    {
        image.sprite = shot;
        image.color = new Color32(255, 255, 225, 255);
    }
    public void SetShotWithCross()
    {
        image.sprite = shotWithCross;
        image.color = new Color32(255, 255, 225, 255);
    }

    public void OnClick()
    {
        if(setupPage2 != null)
        {
            setupPage2.GridButtonOnClick(this.gameObject);
        }
        else if(gamePage != null)
        {
            if(enemy)
            {
                gamePage.EnemyGridButtonOnClick(this.gameObject);
            }
        }
    }
}
