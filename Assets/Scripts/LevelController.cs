using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderStuff;
using Fungus;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    [System.Serializable]
    public class SceneField
    {
        [SerializeField]
        private Object m_SceneAsset;
        [SerializeField]
        private string m_SceneName = "";
        public string SceneName
        {
            get { return m_SceneName; }
        }
        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }
    }

    [Header("To calulate score, be in game scene and playing")]

    public float percentageOneStar;
    public float percentageTwoStars;
    public float percentageThreeStars;
    public int currentLevel;
    [Header("Scene References")]
    public string mainMenuName;
    public string levelSelectName;
    public LevelInformation levelInformation;

    [System.NonSerialized]
    public Flowchart thisFlowChart;

    private bool isShowingCredits;

    private static LevelController _instance;
    public static LevelController Instance { get { return _instance; } }

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

        CustomersCheck();
    }

    private void Start()
    {
        isShowingCredits = false;

        thisFlowChart = transform.GetComponentInChildren<Flowchart>();
    }

    private void CustomersCheck()
    {
        int levelIndex = 0; 
        foreach(Level a in levelInformation.levels)
        {
            int customerIndex = 0;
            foreach (Customer b in a.customers)
            {
                //wrong item count
                if (b.itemsInOrderIndexInBoardManager.Count < 1 || b.itemsInOrderIndexInBoardManager.Count > 4) Debug.LogError("Customer " + customerIndex + " in level: " + levelIndex + " has too many or too few items");

                //custiomer with no spawn time
                if (customerIndex > 3 && b.spawnTime < 5f) Debug.LogError("Customer " + customerIndex + " in level: " + levelIndex + " has a spawn timer set to start");

                customerIndex++;
            }

            levelIndex++;
        }
    }

    public void StartEndLevelSequence()
    {
        //set score and run
        GameObject.Find("Flowchart").GetComponent<Flowchart>().SetFloatVariable("Score", MyGameManager.Instance.GetScore());
        GameObject.Find("Flowchart").GetComponent<Flowchart>().ExecuteBlock("EndOfLevel");
    }

    public void LoadOtherSceneByIndex(int index)
    {
        switch(index)
        {
            case 0:
                StartCoroutine(LoadScene(mainMenuName));
                break;
            case 1:
                StartCoroutine(LoadScene(levelSelectName));
                break;
        }

        currentLevel = 9999;

        Time.timeScale = 1f;
    }

    public void LoadLevelByIndex(int index)
    {
        currentLevel = index;
        StartCoroutine(LoadScene(levelInformation.GameSceneName.ToString()));

        Time.timeScale = 1f;
    }

    private IEnumerator LoadScene(string SceneName)
    {
        thisFlowChart.ExecuteBlock("FadeIn");
        yield return new WaitForSeconds(0.3f);
        yield return SceneManager.LoadSceneAsync(SceneName);
        thisFlowChart.ExecuteBlock("FadeOut");
    }

    public void ResetCurrentLevel()
    {
        LoadLevelByIndex(currentLevel);
    }
    
    public void GoToLevelSelect()
    {
        LoadOtherSceneByIndex(1);
    }
    
    public void GoToLevelOneTest()
    {
        LoadLevelByIndex(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleCredits()
    {
        isShowingCredits = !isShowingCredits;

        if(isShowingCredits)
        {
            thisFlowChart.ExecuteBlock("ShowCredits");
        }
        else
        {
            thisFlowChart.ExecuteBlock("HideCredits");
        }
    }
}
