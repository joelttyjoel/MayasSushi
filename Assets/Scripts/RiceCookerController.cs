using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RiceCookerController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject riceOnMouse;
    public Sprite riceCookerDone;
    public Sprite riceCookerWaiting;
    public float riceCookerTime;
    public BasicFillTimerController timerController;

    private float timer;
    private bool spriteSetToDone;
    private bool isUsingRiceCooker;
    private Image image;
    private GameObject riceOnMouseObject;

    // Start is called before the first frame update
    void Start()
    {
        timer = riceCookerTime;
        spriteSetToDone = false;
        isUsingRiceCooker = false;

        image = GetComponent<Image>();

        riceOnMouse.transform.position = new Vector3(-9999, -9999, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if(timer < 0f && !spriteSetToDone)
        {
            spriteSetToDone = true;

            image.sprite = riceCookerDone;
        }
        else if (timer > 0f && spriteSetToDone)
        {
            spriteSetToDone = false;

            image.sprite = riceCookerWaiting;
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timerController.SetTimerPercentage(1 - (timer / riceCookerTime));
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (spriteSetToDone && InteractionManager.Instance.canInteract)
        {
            isUsingRiceCooker = true;
            InteractionManager.Instance.canInteract = false;
        }
        else isUsingRiceCooker = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isUsingRiceCooker) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        riceOnMouse.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isUsingRiceCooker) return;
        isUsingRiceCooker = false;

        RaycastHit2D objectHitPerhaps = InteractionManager.Instance.CheckIfHitObject(InteractionManager.Instance.layerMaskTiles);
        if (objectHitPerhaps.collider != null)
        {
            //only reset if actually does something
            timer = riceCookerTime;

            //make list of selected objects
            List<GameObject> objectsToReplace = new List<GameObject>();

            objectsToReplace.Add(objectHitPerhaps.collider.gameObject);

            //now remove old objects
            BoardManager.Instance.ClearBoardOfSelected(objectsToReplace);

            //first fill, so can just copy existing gameobjects
            BoardManager.Instance.FillBoardOnlyRice(objectsToReplace);
        }

        riceOnMouse.transform.position = new Vector3(-9999, -9999, 0);

        InteractionManager.Instance.canInteract = true;
    }


    ////start hold piece
    //private void OnMouseDown()
    //{
    //    if (spriteSetToDone && InteractionManager.Instance.canInteract)
    //    {
    //        isUsingRiceCooker = true;
    //        InteractionManager.Instance.canInteract = false;
    //    }
    //    else isUsingRiceCooker = false;
    //}

    //private void OnMouseDrag()
    //{
    //    if (!isUsingRiceCooker) return;

    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

    //    riceOnMouse.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    //}

    ////drop piece if possible
    //private void OnMouseUp()
    //{
    //    if (!isUsingRiceCooker) return;
    //    isUsingRiceCooker = false;

    //    RaycastHit2D objectHitPerhaps = InteractionManager.Instance.CheckIfHitObject(InteractionManager.Instance.layerMaskTiles);
    //    if (objectHitPerhaps.collider != null)
    //    {
    //        //only reset if actually does something
    //        timer = riceCookerTime;

    //        //make list of selected objects
    //        List<GameObject> objectsToReplace = new List<GameObject>();

    //        objectsToReplace.Add(objectHitPerhaps.collider.gameObject);

    //        //now remove old objects
    //        BoardManager.Instance.ClearBoardOfSelected(objectsToReplace);

    //        //first fill, so can just copy existing gameobjects
    //        BoardManager.Instance.FillBoardOnlyRice(objectsToReplace);
    //    }

    //    riceOnMouse.transform.position = new Vector3(-9999, -9999, 0);

    //    InteractionManager.Instance.canInteract = true;
    //}
}
