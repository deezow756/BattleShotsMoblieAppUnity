using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public string ButtonName;
    public ConnectionPage connectionPage;

    public void Clicked()
    {
        if(connectionPage != null)
            connectionPage.CompleteConnection(ButtonName);
    }
}
