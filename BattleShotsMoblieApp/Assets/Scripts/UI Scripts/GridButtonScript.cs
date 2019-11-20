using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridButtonScript : MonoBehaviour
{
    [SerializeField]
    Image image;
    [SerializeField]
    private Sprite cross;
    [SerializeField]
    private Sprite shot;
    [SerializeField]
    private Sprite shotWithCross;

    private void OnEnable()
    {
        SetNoImage();
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
}
