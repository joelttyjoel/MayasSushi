using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickDoSomething : MonoBehaviour
{
    public bool GoToLevelOne;
    public bool ToggleCredits;
    public bool ExitGame;
    public bool SlideyMenuCheck;

    private SlideyMenuController slideyMenu;
    private bool doTheDo;

    private void Start()
    {
        if(SlideyMenuCheck)
        {
            slideyMenu = GameObject.Find("SlideyMenu").GetComponent<SlideyMenuController>();
        }

        doTheDo = false;
    }

    private void OnMouseUp()
    {
        doTheDo = true;
    }

    private void LateUpdate()
    {
        if (!doTheDo) return;

        if (SlideyMenuCheck)
        {
            Debug.Log(slideyMenu.willSlide);
            if (slideyMenu.willSlide)
            {
                doTheDo = false;
                return;
            }
            else
            {
                slideyMenu.willSlide = false;
            }
        }

        if (GoToLevelOne)
        {
            LevelController.Instance.LoadLevelByIndex(0);
        }
        else if (ToggleCredits)
        {
            LevelController.Instance.ToggleCredits();
        }
        else if (ExitGame)
        {
            LevelController.Instance.QuitGame();
        }

        doTheDo = false;
    }
}
