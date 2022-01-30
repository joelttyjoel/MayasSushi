using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctionRedirector : MonoBehaviour
{
    public void LevelController_LoadLevelByIndex(int index)
    {
        LevelController.Instance.LoadLevelByIndex(index);
    }

    public void LevelController_LoadOtherLeveByIndex(int index)
    {
        LevelController.Instance.LoadOtherSceneByIndex(index);
    }

    public void LeveController_GoToLevelSelect()
    {
        LevelController.Instance.LoadOtherSceneByIndex(1);
    }
}
