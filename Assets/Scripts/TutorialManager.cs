using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TutorialManager : MonoBehaviour
{
    public Flowchart tutorialFlowchart;
    public string hasShownFirstPlayInstructions;
    public string hasShownLevelSelectInstructions;
    public List<int> levelsWithTutorial;

    private bool timeIsOn;

    private static TutorialManager _instance;
    public static TutorialManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        timeIsOn = true;

        if (PlayerPrefs.GetInt(hasShownFirstPlayInstructions) == 0)
        {
            tutorialFlowchart.ExecuteBlock("StartTutorial");

            PlayerPrefs.SetInt(hasShownFirstPlayInstructions, 1);
        }
    }

    public void LoadLevelTutorialCheck(bool isLevelSelect, int currentLevel)
    {
        //spaghetti solution, but B) hey wassup baby

        if(isLevelSelect)
        {
            if (PlayerPrefs.GetInt(hasShownLevelSelectInstructions) == 0)
            {
                tutorialFlowchart.ExecuteBlock("LevelSelectInstructions");

                PlayerPrefs.SetInt(hasShownLevelSelectInstructions, 1);
            }
        }

        foreach(int a in levelsWithTutorial)
        {
            if(currentLevel == a)
            {
                tutorialFlowchart.ExecuteBlock("Level" + currentLevel + "Tutorial");
            }
        }
    }

    public void ToggleTime()
    {
        timeIsOn = !timeIsOn;

        if (timeIsOn)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0.2f;
        }
    }
}
