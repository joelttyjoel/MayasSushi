using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicFillTimerController : MonoBehaviour
{
    //Made by Joel Håkansson, https://github.com/joelttyjoel
    
    [Header("Should timer hide when empty?")]
    public bool hideOnEmpty;
    [Header("0 = 0%, 1 = 100%")]
    public float startFillPercentage;
    [Header("True if timer is increasing, aka starting at 0% and going to 100%, false if opposite")]
    public bool timerIsGoingUp;

    private Image backgroundImage;
    private Image fillImage;
    private float currentFillPercentage;
    private float endFillPercentage;

    // Start is called before the first frame update
    void Start()
    {
        backgroundImage = GetComponent<Image>();
        fillImage = transform.GetChild(0).GetComponent<Image>();

        //basicly start at opposite side of start, if 100 -> 0, if 0 -> 100. 
        endFillPercentage = 1f - startFillPercentage;
        fillImage.fillAmount = endFillPercentage;

        //idc copy paste
        if (!hideOnEmpty) return;

        if (fillImage.fillAmount <= endFillPercentage)
        {
            fillImage.enabled = false;
        }
        else
        {
            fillImage.enabled = true;
        }
    }

    //set percentage level, send percentage of what shuold be here sir
    public void SetTimerPercentage(float percentage)
    {
        fillImage.fillAmount = percentage;

        if (!hideOnEmpty) return;

        //Debug.Log(percentage);
        //Debug.Log(endFillPercentage);
        if (percentage <= endFillPercentage)
        {
            fillImage.enabled = timerIsGoingUp;
            backgroundImage.enabled = timerIsGoingUp;
        }
        else
        {
            fillImage.enabled = !timerIsGoingUp;
            backgroundImage.enabled = !timerIsGoingUp;
        }
    }
}
