using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour
{
    private int thisButtonsLevel;

    private void Start()
    {
        thisButtonsLevel = int.Parse(transform.Find("Number").GetComponent<Text>().text);

        float thisLevelPercentage = PlayerPrefs.GetFloat(thisButtonsLevel.ToString());

        Debug.Log(thisLevelPercentage);
        Debug.Log(PlayerPrefs.GetFloat("0"));
        Debug.Log(PlayerPrefs.GetFloat("1"));
        Debug.Log(PlayerPrefs.GetFloat("2"));

        if (thisLevelPercentage > LevelController.Instance.percentageThreeStars)
        {
            transform.Find("Star (2)").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            transform.Find("Star (1)").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            transform.Find("Star").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            Debug.Log("3 stars");
        }
        else if (thisLevelPercentage > LevelController.Instance.percentageTwoStars)
        {
            transform.Find("Star (1)").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            transform.Find("Star").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            Debug.Log("2 stars");
        }
        else if (thisLevelPercentage > LevelController.Instance.percentageOneStar)
        {
            transform.Find("Star").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            Debug.Log("1 stars");
        }
    }
}
