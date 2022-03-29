using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonController : MonoBehaviour
{
    public bool unlockedFromStart;

    private int thisButtonsLevel;

    private void Start()
    {
        thisButtonsLevel = int.Parse(transform.Find("Number").GetComponent<Text>().text);

        float thisLevelPercentage = PlayerPrefs.GetFloat(thisButtonsLevel.ToString());

        //Debug.Log(thisLevelPercentage);
        //Debug.Log(PlayerPrefs.GetFloat("0"));
        //Debug.Log(PlayerPrefs.GetFloat("1"));
        //Debug.Log(PlayerPrefs.GetFloat("2"));

        if (thisLevelPercentage > LevelController.Instance.percentageThreeStars)
        {
            transform.Find("Star (2)").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            transform.Find("Star (1)").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            transform.Find("Star").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            return;
            //Debug.Log("3 stars");
        }
        else if (thisLevelPercentage > LevelController.Instance.percentageTwoStars)
        {
            transform.Find("Star (1)").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            transform.Find("Star").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            return;
            //Debug.Log("2 stars");
        }
        else if (thisLevelPercentage > LevelController.Instance.percentageOneStar)
        {
            transform.Find("Star").GetComponent<Image>().sprite = LevelController.Instance.starGood;
            return;
            //Debug.Log("1 stars");
        }

        //check if locked
        if (unlockedFromStart) return;

        int previousButtonsLevel = int.Parse(transform.Find("Number").GetComponent<Text>().text) - 1;

        float previousLevelPercentage = PlayerPrefs.GetFloat(previousButtonsLevel.ToString());

        if (previousLevelPercentage < LevelController.Instance.percentageOneStar)
        {
            GetComponent<Button>().interactable = false;
            GetComponent<Image>().color = LevelController.Instance.alphaLevelsNotUnlocked;
            transform.Find("Star (2)").GetComponent<Image>().color = LevelController.Instance.alphaLevelsNotUnlocked;
            transform.Find("Star (1)").GetComponent<Image>().color = LevelController.Instance.alphaLevelsNotUnlocked;
            transform.Find("Star").GetComponent<Image>().color = LevelController.Instance.alphaLevelsNotUnlocked;
            transform.Find("Number").GetComponent<Text>().color = new Color(0, 0, 0, LevelController.Instance.alphaLevelsNotUnlocked.a);
        }
    }
}
