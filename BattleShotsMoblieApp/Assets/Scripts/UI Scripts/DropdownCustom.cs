using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownCustom : MonoBehaviour
{
    [SerializeField]
    SetupPage1 setupPage1;
    [SerializeField]
    Text dropdownText;
    [SerializeField]
    private RectTransform container;
    private bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scale = container.localScale;
        scale.y = Mathf.Lerp(scale.y, isOpen ? 1 : 0, Time.deltaTime * 12);
        container.localScale = scale;
    }

    public void DropdownOnClick()
    {
        if(isOpen)
        {
            isOpen = false;
        }
        else
        {
            isOpen = true;
        }
    }

    public void DropdownButtonOnClick(int value)
    {
        dropdownText.text = value.ToString();
        isOpen = false;
        setupPage1.DropDownOptionChange(value);
    }
    
}
